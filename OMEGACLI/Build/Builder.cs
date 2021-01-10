using OMEGA;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;

namespace OMEGACLI
{
    public static class Builder
    {
        public static void BuildGame(string resources_folder, GameAssetsManifest manifest)
        {
            Console.WriteLine("Building Game Asssets...");

            ResourceLoader.SetRootPath(resources_folder);

            List<ResourcePak> resource_paks =
                BuildProjectResources(manifest);

            foreach (var pak in resource_paks)
            {
                using var pak_file = File.Create(Path.Combine(resources_folder, pak.Name + ".pak"));

                Serializer.Serialize(pak_file, pak);
            }

            Console.WriteLine("Project Built Successfully");
        }

        //private static void BuildAppConfigFile(string root_path, AppProject project)
        //{
        //    GameProperties props = new GameProperties()
        //    {
        //        Title = project.Title,
        //        FrameRate = project.FrameRate,
        //        CanvasWidth = project.CanvasWidth,
        //        CanvasHeight = project.CanvasHeight,
        //        Fullscreen = project.StartFullscreen,
        //        PreloadResourcePaks = project.PreloadPaks
        //    };

        //    File.WriteAllBytes(Path.Combine(root_path, "Config.json"), 
        //        JsonSerializer.PrettyPrintByteArray(JsonSerializer.Serialize(props)));

        //}

        private static List<ResourcePak> BuildProjectResources(GameAssetsManifest manifest)
        {
            var resource_groups = manifest.Resources;

            var results = new List<ResourcePak>();

            foreach (var resource_group in resource_groups)
            {
                var pak = new ResourcePak(resource_group.Key);

                Console.WriteLine($"Creating resource Pak: {pak.Name}");

                if (resource_group.Value.Images != null)
                {
                    foreach (var image_info in resource_group.Value.Images)
                    {
                        var pixmap_data = ImageBuilder.Build(image_info.Id, image_info.Path);

                        pak.Images.Add(image_info.Id, pixmap_data);

                        pak.TotalResourcesCount++;

                        Console.WriteLine($"Added Image: {pixmap_data.Id}");
                    }
                }

                if (resource_group.Value.Shaders != null)
                {
                    foreach (var shader_info in resource_group.Value.Shaders)
                    {
                        var shader_data = ShaderBuilder.Build(shader_info.Id, shader_info.VsPath, shader_info.FsPath);

                        pak.Shaders.Add(shader_info.Id, shader_data);

                        pak.TotalResourcesCount++;

                        Console.WriteLine($"Added Shader: {shader_data.Id}");
                    }
                }

                if (resource_group.Value.Fonts != null)
                {
                    foreach (var font_info in resource_group.Value.Fonts)
                    {
                        var build_params = new FontBuildParams()
                        {
                            Id = font_info.Id,
                            Path = font_info.Path,
                            Size = font_info.Size,
                            CharRangeLevel = font_info.CharRangeLevel,
                            PaddingLeft = font_info.Padding != null ? font_info.Padding[0] : 0,
                            PaddingRight = font_info.Padding != null ? font_info.Padding[1] : 0,
                            PaddingUp = font_info.Padding != null ? font_info.Padding[2] : 0,
                            PaddingDown = font_info.Padding != null ? font_info.Padding[3] : 0,
                            DropShadow = font_info.DropShadow,
                            ShadowOffsetX = font_info.ShadowOffsetX,
                            ShadowOffsetY = font_info.ShadowOffsetY,
                            ShadowColor = font_info.ShadowColor != null ? Color.FromHex(font_info.ShadowColor) : Color.Black
                        };

                        var font_data = FontBuilder.Build(build_params);

                        pak.Fonts.Add(font_info.Id, font_data);

                        pak.TotalResourcesCount++;

                        Console.WriteLine($"Added Font: {font_data.Id}");

                    }
                }

                if (resource_group.Value.Atlases != null)
                {
                    foreach (var atlas_info in resource_group.Value.Atlases)
                    {
                        var atlas_data = AtlasBuilder.Build(atlas_info.Id, atlas_info.Path, atlas_info.Regions);

                        pak.Atlases.Add(atlas_data.Id, atlas_data);

                        pak.TotalResourcesCount++;

                        Console.WriteLine($"Added Atlas: {atlas_data.Id}");
                    }
                }

                if (resource_group.Value.TextFiles != null)
                {
                    foreach (var text_file_info in resource_group.Value.TextFiles)
                    {
                        var text_file_data = TextBuilder.Build(text_file_info.Id, text_file_info.Path);
                        pak.TextFiles.Add(text_file_info.Id, text_file_data);

                        pak.TotalResourcesCount++;

                        Console.WriteLine($"Added TextFile: {text_file_data.Id}");
                    }
                }

                results.Add(pak);
                Console.WriteLine($"Built PAK with {pak.TotalResourcesCount} resources.");
            }

            return results;
        }
    }
}
