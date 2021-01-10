using System;
using System.Collections.Generic;

namespace OMEGA
{
    public class ContentManager
    {
        private readonly Dictionary<string, Resource> m_loaded_resources;
        private readonly Dictionary<string, string[]> m_pak_res_map;
        private readonly List<Resource> m_runtime_resources;

        public ContentManager(GameInfo info)
        {
            ResourceLoader.SetRootPath(info.ResourcesFolder);

            m_loaded_resources = new Dictionary<string, Resource>();
            m_runtime_resources = new List<Resource>();
            m_pak_res_map = new Dictionary<string, string[]>();

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
            if (m_loaded_resources.TryGetValue(resource_id, out Resource resource))
            {
                return (T)resource;
            }

            throw new Exception($"Can't find resource with ID: {resource_id}");
        }
        public void LoadContentPack(string pak_name)
        {
            ResourcePak pak = ResourceLoader.LoadPak(pak_name);

            if (pak.TotalResourcesCount == 0)
            {
                return;
            }

            int res_name_map_idx = 0;

            m_pak_res_map.Add(pak_name, new string[pak.TotalResourcesCount]);

            if (pak.Images != null)
            {
                foreach (var image_res in pak.Images)
                {
                    Texture2D texture = ResourceLoader.LoadTexture(image_res.Value);
                    m_loaded_resources.Add(texture.Id, texture);
                    m_pak_res_map[pak_name][res_name_map_idx++] = image_res.Key;
                }
            }

            if (pak.Atlases != null)
            {
                foreach (var atlas_res in pak.Atlases)
                {
                    TextureAtlas atlas = ResourceLoader.LoadAtlas(atlas_res.Value);
                    m_loaded_resources.Add(atlas.Id, atlas);
                    m_pak_res_map[pak_name][res_name_map_idx++] = atlas_res.Key;
                }
            }

            if (pak.Fonts != null)
            {
                foreach (var font_res in pak.Fonts)
                {
                    Font font = ResourceLoader.LoadFont(font_res.Value);
                    m_loaded_resources.Add(font.Id, font);
                    m_pak_res_map[pak_name][res_name_map_idx++] = font_res.Key;
                }
            }

            if (pak.Shaders != null)
            {
                foreach (var shader_res in pak.Shaders)
                {
                    ShaderProgram shader = ResourceLoader.LoadShader(shader_res.Value);
                    m_loaded_resources.Add(shader.Id, shader);
                    m_pak_res_map[pak_name][res_name_map_idx++] = shader_res.Key;
                }
            }

            if (pak.TextFiles != null)
            {
                foreach (var txt_res in pak.TextFiles)
                {
                    TextFile text_file = ResourceLoader.LoadTextFile(txt_res.Value);
                    m_loaded_resources.Add(text_file.Id, text_file);
                    m_pak_res_map[pak_name][res_name_map_idx++] = txt_res.Key;
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
        }

        internal int RegisterRuntimeLoaded(Resource resource)
        {
            m_runtime_resources.Add(resource);
            return m_runtime_resources.Count;
        }

        public void FreePack(string pack_name)
        {
            Console.WriteLine($" > Diposing resources from Pack: {pack_name}");

            var res_ids = m_pak_res_map[pack_name];

            for (int i = 0; i < res_ids.Length; ++i)
            {
                var res_id = res_ids[i];

                m_loaded_resources[res_id].Dispose();
                Console.WriteLine($" > Disposed resource: {res_id}");
            }
        }

        internal void FreeEverything()
        {
            Console.WriteLine($" > Diposing {m_loaded_resources.Count} loaded resources.");

            foreach (var resource in m_loaded_resources)
            {
                Console.WriteLine($" > Diposing {resource.Key}.");
                resource.Value.Dispose();
            }

            Console.WriteLine($" > Disposing {m_runtime_resources.Count} runtime resources.");

            foreach (var resource in m_runtime_resources)
            {
                Console.WriteLine($" > Diposing {resource.Id}.");
                resource.Dispose();
            }

            m_loaded_resources.Clear();
            m_runtime_resources.Clear();

            GC.Collect();
        }
    }
}
