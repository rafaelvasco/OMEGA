using System.IO;
using MessagePack;
using System.Buffers;
using System.Collections.Generic;

namespace OMEGA
{
    public static class PakLoader
    {
        public static ResourcePak Load(string path)
        {
            var fileBytes = File.ReadAllBytes(path);

            var pakReader = new MessagePackReader(fileBytes);

            // Read Into Nested Object
            pakReader.ReadArrayHeader();

            var pakeName = pakReader.ReadString();

            var pak = new ResourcePak(pakeName);

            int numberImages = pakReader.ReadMapHeader();

            if (numberImages > 0)
            {
                pak.Images = new Dictionary<string, ImageData>();

                for (int i = 0; i < numberImages; ++i)
                {
                    var key = pakReader.ReadString();

                    pakReader.ReadArrayHeader();

                    var imageId = pakReader.ReadString();

                    var imageData = pakReader.ReadBytes();
                    int imageWidth = pakReader.ReadInt32();
                    int imageHeight = pakReader.ReadInt32();

                    pak.Images.Add(key, new ImageData()
                    {
                        Id = imageId,
                        Data = imageData?.ToArray(),
                        Width = imageWidth,
                        Height = imageHeight

                    });
                }
            }

            int numberAtlases = pakReader.ReadMapHeader();

            if (numberAtlases > 0)
            {
                pak.Atlases = new Dictionary<string, TextureAtlasData>();

                for (int i = 0; i < numberAtlases; i++)
                {
                    var key = pakReader.ReadString();
                    pakReader.ReadArrayHeader();
                    var atlasId = pakReader.ReadString();
                    var atlasImageData = pakReader.ReadBytes();
                    int imageWidth = pakReader.ReadInt32();
                    int imageHeight = pakReader.ReadInt32();

                    var atlasData = new TextureAtlasData()
                    {
                        Id = atlasId,
                        Data = atlasImageData?.ToArray(),
                        Atlas = new Dictionary<string, SRect>(),
                        Width = imageWidth,
                        Height = imageHeight,
                    };

                    int rectsCount = pakReader.ReadMapHeader();

                    if (rectsCount > 0)
                    {
                        for (int j = 0; j < rectsCount; ++j)
                        {
                            var rectKey = pakReader.ReadString();
                            
                            pakReader.ReadArrayHeader();
                            var rectX = pakReader.ReadInt32();
                            var rectY = pakReader.ReadInt32();
                            var rectW = pakReader.ReadInt32();
                            var rectH = pakReader.ReadInt32();

                            atlasData.Atlas.Add(rectKey, new SRect(rectX, rectY, rectW, rectH));
                        }
                    }

                    atlasData.RuntimeUpdatable = pakReader.ReadBoolean();


                    pak.Atlases.Add(key, atlasData);
                }
            }

            int shadersCount = pakReader.ReadMapHeader();

            if (shadersCount > 0)
            {
                pak.Shaders = new Dictionary<string, ShaderProgramData>();

                for (int i = 0; i < shadersCount; ++i)
                {
                    var shaderKey = pakReader.ReadString();

                    pakReader.ReadArrayHeader();

                    var shaderId = pakReader.ReadString();
                    var vertexShaderData = pakReader.ReadBytes();
                    var fragShaderData = pakReader.ReadBytes();
                    string[] samplers = null;
                    string[] @params = null;

                    int samplersCount = pakReader.ReadArrayHeader();

                    if (samplersCount > 0)
                    {
                        samplers = new string[samplersCount];

                        for (int j = 0; j < samplersCount; ++j)
                        {
                            samplers[j] = pakReader.ReadString();
                        }
                    }

                    int paramsCount = pakReader.ReadArrayHeader();

                    if (paramsCount > 0)
                    {
                        @params = new string[paramsCount];

                        for (int k = 0; k < paramsCount; ++k)
                        {
                            @params[k] = pakReader.ReadString();
                        }
                    }

                    pak.Shaders.Add(shaderKey, new ShaderProgramData()
                    {
                        Id = shaderId,
                        VertexShader = vertexShaderData?.ToArray(),
                        FragmentShader = fragShaderData?.ToArray(),
                        Samplers = samplers,
                        Params = @params
                    });

                }
            }

            int fontsSize = pakReader.ReadMapHeader();

            if (fontsSize > 0)
            {
                pak.Fonts = new Dictionary<string, FontData>();

                for (int i = 0; i < fontsSize; ++i)
                {
                    var fontKey = pakReader.ReadString();

                    pakReader.ReadArrayHeader();

                    var fontId = pakReader.ReadString();

                    var fontData = new FontData()
                    {
                        Id = fontId
                    };

                    pakReader.ReadArrayHeader();

                    var fontImageDataId = pakReader.ReadString();
                    var fontImageData = pakReader.ReadBytes();
                    int fontImageWidth = pakReader.ReadInt32();
                    int fontImageHeight = pakReader.ReadInt32();

                    fontData.FontSheet = new ImageData()
                    {
                        Id = fontImageDataId,
                        Width = fontImageWidth,
                        Height = fontImageHeight,
                        Data = fontImageData?.ToArray()
                    };

                    int charCount = pakReader.ReadArrayHeader();

                    if (charCount > 0)
                    {
                        fontData.Chars = new char[charCount];

                        for (int j = 0; j < charCount; ++j)
                        {
                            fontData.Chars[j] = pakReader.ReadChar();
                        }
                    }

                    int glyphRectCount = pakReader.ReadArrayHeader();

                    if (glyphRectCount > 0)
                    {
                        fontData.GlyphRects = new SRect[glyphRectCount];

                        for (int k = 0; k < glyphRectCount; ++k)
                        {
                            pakReader.ReadArrayHeader();

                            int x = pakReader.ReadInt32();
                            int y = pakReader.ReadInt32();
                            int w = pakReader.ReadInt32();
                            int h = pakReader.ReadInt32();

                            fontData.GlyphRects[k] = new SRect(x, y, w, h);
                        }
                    }

                    int glyphCroppingsCount = pakReader.ReadArrayHeader();

                    if (glyphCroppingsCount > 0)
                    {
                        fontData.GlyphCroppings = new SRect[glyphCroppingsCount];

                        for (int l = 0; l < glyphCroppingsCount; ++l)
                        {
                            pakReader.ReadArrayHeader();

                            int x = pakReader.ReadInt32();
                            int y = pakReader.ReadInt32();
                            int w = pakReader.ReadInt32();
                            int h = pakReader.ReadInt32();

                            fontData.GlyphCroppings[l] = new SRect(x, y, w, h);
                        }
                    }

                    int glyphKerningsCount = pakReader.ReadArrayHeader();

                    if (glyphKerningsCount > 0)
                    {
                        fontData.GlyphKernings = new SVec3[glyphKerningsCount];

                        for (int k = 0; k < glyphKerningsCount; ++k)
                        {
                            pakReader.ReadArrayHeader();

                            float x = (float) pakReader.ReadDouble();
                            float y = (float)pakReader.ReadDouble();
                            float z = (float)pakReader.ReadDouble();

                            fontData.GlyphKernings[k] = new SVec3(x, y, z);
                        }
                    }

                    fontData.LineSpacing = pakReader.ReadInt32();
                    fontData.Spacing = pakReader.ReadInt32();
                    fontData.DefaultChar = pakReader.ReadChar();

                    pak.Fonts.Add(fontKey, fontData);
                }
            }

            int textCount = pakReader.ReadMapHeader();

            if (textCount > 0)
            {
                pak.TextFiles = new Dictionary<string, TextFileData>();

                var textKey = pakReader.ReadString();

                pakReader.ReadArrayHeader();

                var textId = pakReader.ReadString();

                int textRowCount = pakReader.ReadArrayHeader();

                var textData = new TextFileData()
                {
                    Id = textId,
                    TextData = new byte[textRowCount][]
                };

                for (int i = 0; i < textRowCount; ++i)
                {
                    textData.TextData[i] = pakReader.ReadBytes().Value.ToArray();
                }

                pak.TextFiles.Add(textKey, textData);
            }

            pak.TotalResourcesCount = pakReader.ReadInt32();

            return pak;
        }
    }
}
