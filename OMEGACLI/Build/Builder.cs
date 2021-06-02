﻿using MessagePack;
using OMEGA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OMEGACLI
{
    public static class Builder
    {
        public static void BuildGame(string resourcesFolder, GameAssetsManifest manifest)
        {
            Console.WriteLine("Building Game Asssets...");

            ResourceLoader.SetRootPath(resourcesFolder);

            List<ResourcePak> resource_paks =
                BuildProjectResources(manifest);

            foreach (var pak in resource_paks)
            {
                using var pak_file = File.Create(Path.Combine(resourcesFolder, pak.Name + ".pak"));

                MessagePackSerializer.Serialize(pak_file, pak);
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

            foreach (var (groupKey, group) in resource_groups)
            {
                var pak = new ResourcePak(groupKey);

                Console.WriteLine($"Creating resource Pak: {pak.Name}");

                if (group.Images != null)
                {
                    foreach (var image_info in group.Images)
                    {
                        var pixmap_data = ImageBuilder.Build(image_info.Id, image_info.Path);

                        pak.Images.Add(image_info.Id, pixmap_data);

                        pak.TotalResourcesCount++;

                        Console.WriteLine($"Added Image: {pixmap_data.Id}");
                    }
                }

                if (group.Shaders != null)
                {
                    foreach (var shader_info in group.Shaders)
                    {
                        var shader_data = ShaderBuilder.Build(shader_info.Id, shader_info.VsPath, shader_info.FsPath);

                        pak.Shaders.Add(shader_info.Id, shader_data);

                        pak.TotalResourcesCount++;

                        Console.WriteLine($"Added Shader: {shader_data.Id}");
                    }
                }

                if (group.Fonts != null)
                {
                    foreach (var font_info in group.Fonts)
                    {
                        var build_params = new FontBuildParams()
                        {
                            Id = font_info.Id,
                            LineSpacing = font_info.LineSpacing,
                            Spacing = font_info.Spacing,
                            DefaultChar = font_info.DefaultChar,
                            Faces = font_info.Faces.Select(f => new FontFace()
                            {
                                CharRanges = f.CharRanges.Select(CharRange.GetFromKey).ToList(),
                                Path = f.Path,
                                Size = f.Size,
                            }).ToList()
                        };

                        var font_data = FontBuilder.Build(build_params);

                        pak.Fonts.Add(font_info.Id, font_data);

                        pak.TotalResourcesCount++;

                        Console.WriteLine($"Added Font: {font_data.Id}");

                    }
                }

                if (group.Atlases != null)
                {
                    foreach (var atlas_info in group.Atlases)
                    {
                        var atlas_data = AtlasBuilder.Build(atlas_info.Id, atlas_info.Path, atlas_info.Regions);

                        pak.Atlases.Add(atlas_data.Id, atlas_data);

                        pak.TotalResourcesCount++;

                        Console.WriteLine($"Added Atlas: {atlas_data.Id}");
                    }
                }

                if (group.TextFiles != null)
                {
                    foreach (var text_file_info in group.TextFiles)
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
