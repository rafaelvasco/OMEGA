using OMEGA;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;

namespace OMEGACLI.Build
{
    public static class Builder
    {

        public static void BuildGame(string resources_folder, GameAssetsManifest manifest)
        {
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

                var resource_defs = resource_group.Value;


                foreach (var resource_def in resource_defs)
                {
                    switch (resource_def.ResourceType)
                    {
                        case ResourceType.Image:

                            var pixmap_data = ResourceLoader.LoadImageData(resource_def.ResourceId, resource_def.ResourcePath);

                            pak.Images.Add(resource_def.ResourceId, pixmap_data);

                            break;

                        case ResourceType.Font:

                            var font_data = ResourceLoader.LoadFontData(resource_def.ResourceId, resource_def.ResourcePath, resource_def.SecondaryResourcePath);

                            pak.Fonts.Add(resource_def.ResourceId, font_data);

                            break;
                        case ResourceType.Shader:

                            var shader_data = ResourceLoader.LoadShaderProgramData(resource_def.ResourceId, resource_def.ResourcePath, resource_def.SecondaryResourcePath);

                            pak.Shaders.Add(resource_def.ResourceId, shader_data);

                            break;

                        //case ResourceType.Sfx:

                        //    var sfx_data = Loader.LoadSfxData(res_file_info.FullPath);

                        //    pak.Sfx.Add(res_file_info.FileName, sfx_data);

                        //    break;
                        //case ResourceType.Song:

                        //    var song_data = Loader.LoadSongData(res_file_info.FullPath);

                        //    pak.Songs.Add(res_file_info.FileName, song_data);

                        //    break;

                        case ResourceType.TextFile:

                            var text_file_data = ResourceLoader.LoadTextFileData(resource_def.ResourcePath);
                            pak.TextFiles.Add(resource_def.ResourceId, text_file_data);

                            break;
                    }
                }

                results.Add(pak);
            }

            return results;
        }
    }
}
