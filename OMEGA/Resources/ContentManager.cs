using System;
using System.Collections.Generic;

namespace OMEGA
{
    public class ContentManager
    {
        private readonly Dictionary<string, Resource> _loaded_resources;

        private readonly List<Resource> _runtime_resources;

        public ContentManager(GameInfo info)
        {
            ResourceLoader.SetRootPath(info.ResourcesFolder);

            _loaded_resources = new Dictionary<string, Resource>();
            _runtime_resources = new List<Resource>();

            LoadContentPack("base");

            if (info.PreloadPaks != null)
            {
                foreach (var pak_name in info.PreloadPaks)
                {
                    LoadContentPack(pak_name);
                }
            }
        }

        public T Get<T>(string resource_id) where T : Resource
        {
            if (_loaded_resources.TryGetValue(resource_id, out Resource resource))
            {
                return (T)resource;
            }

            throw new Exception($"Can't find resource with ID: {resource_id}");
        }



        public void LoadContentPack(string pak_name)
        {
            ResourcePak pak = ResourceLoader.LoadPak(pak_name);

            if (pak.Images != null)
            {
                foreach (var image_res in pak.Images)
                {
                    Texture2D texture = ResourceLoader.LoadTexture(image_res.Value);

                    _loaded_resources.Add(texture.Id, texture);
                }
            }

            if (pak.Fonts != null)
            {
                foreach (var font_res in pak.Fonts)
                {
                    Font font = ResourceLoader.LoadFont(font_res.Value);

                    _loaded_resources.Add(font.Id, font);
                }
            }

            if (pak.Shaders != null)
            {
                foreach (var shader_res in pak.Shaders)
                {
                    ShaderProgram shader = ResourceLoader.LoadShader(shader_res.Value);

                    _loaded_resources.Add(shader.Id, shader);
                }
            }

            //foreach (var sfx_res in pak.Sfx)
            //{
            //    Effect effect = _loader.LoadEffect(sfx_res.Value);

            //    _loaded_resources.Add(effect.Id, effect);
            //}

            //foreach (var song_res in pak.Songs)
            //{
            //    Song song = _loader.LoadSong(song_res.Value);

            //    _loaded_resources.Add(song.Id, song);
            //}

            if (pak.TextFiles != null)
            {
                foreach (var txt_res in pak.TextFiles)
                {
                    TextFile text_file = ResourceLoader.LoadTextFile(txt_res.Value);

                    _loaded_resources.Add(text_file.Id, text_file);
                }
            }


        }

        internal void RegisterRuntimeLoaded(Resource resource)
        {
            _runtime_resources.Add(resource);
        }

        internal void DisposeRuntimeLoaded(Resource resource)
        {
            _runtime_resources.Remove(resource);

            resource.Dispose();
        }

        internal void FreeEverything()
        {
            Console.WriteLine($" > Diposing {_loaded_resources.Count.ToString()} loaded resources.");

            foreach (var resource in _loaded_resources)
            {
                Console.WriteLine($" > Diposing {resource.Key}.");
                resource.Value.Dispose();
            }

            Console.WriteLine($" > Disposing {_runtime_resources.Count.ToString()} runtime resources.");

            foreach (var resource in _runtime_resources)
            {
                Console.WriteLine($" > Diposing {resource.Id}.");
                resource.Dispose();
            }

            _loaded_resources.Clear();
            _runtime_resources.Clear();
        }
    }
}
