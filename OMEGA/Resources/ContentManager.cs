using System;
using System.Collections.Generic;

namespace OMEGA
{
    public class ContentManager
    {
        private readonly Dictionary<string, Resource> _loadedResources;
        private readonly Dictionary<string, string[]> _pakMap;
        private readonly Dictionary<string, Resource> _runtimeResources;

        public ContentManager(GameInfo info)
        {
            ResourceLoader.SetRootPath(info.ResourcesFolder);

            _loadedResources = new Dictionary<string, Resource>();
            _runtimeResources = new Dictionary<string, Resource>();
            _pakMap = new Dictionary<string, string[]>();

            LoadContentPack("base");

            if (info.PreloadPaks != null)
            {
                foreach (var pak_name in info.PreloadPaks)
                {
                    LoadContentPack(pak_name);
                }
            }
        }

        public T Get<T>(string resourceId) where T : Resource
        {
            if (_loadedResources.TryGetValue(resourceId, out var resource))
            {
                return (T)resource;
            }

            throw new Exception($"Can't find resource with ID: {resourceId}");
        }
        public void LoadContentPack(string pakName)
        {
            ResourcePak pak = ResourceLoader.LoadPak(pakName);

            if (pak.TotalResourcesCount == 0)
            {
                return;
            }

            int res_name_map_idx = 0;

            _pakMap.Add(pakName, new string[pak.TotalResourcesCount]);

            if (pak.Images != null)
            {
                foreach (var (imageKey, imageData) in pak.Images)
                {
                    Texture2D texture = ResourceLoader.LoadTexture(imageData);
                    _loadedResources.Add(texture.Id, texture);
                    _pakMap[pakName][res_name_map_idx++] = imageKey;
                }
            }

            if (pak.Atlases != null)
            {
                foreach (var (atlasKey, atlasData) in pak.Atlases)
                {
                    TextureAtlas atlas = ResourceLoader.LoadAtlas(atlasData);
                    _loadedResources.Add(atlas.Id, atlas);
                    _pakMap[pakName][res_name_map_idx++] = atlasKey;
                }
            }

            if (pak.Fonts != null)
            {
                foreach (var (fontKey, fontData) in pak.Fonts)
                {
                    TextureFont font = ResourceLoader.LoadFont(fontData);
                    _loadedResources.Add(font.Id, font);
                    _pakMap[pakName][res_name_map_idx++] = fontKey;
                }
            }

            if (pak.Shaders != null)
            {
                foreach (var (shaderKey, shaderProgramData) in pak.Shaders)
                {
                    ShaderProgram shader = ResourceLoader.LoadShader(shaderProgramData);
                    _loadedResources.Add(shader.Id, shader);
                    _pakMap[pakName][res_name_map_idx++] = shaderKey;
                }
            }

            if (pak.TextFiles != null)
            {
                foreach (var (txtKey, textFileData) in pak.TextFiles)
                {
                    TextFile text_file = ResourceLoader.LoadTextFile(textFileData);
                    _loadedResources.Add(text_file.Id, text_file);
                    _pakMap[pakName][res_name_map_idx++] = txtKey;
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

        internal void RegisterRuntimeLoaded(Resource resource)
        {
            resource.Id = Guid.NewGuid().ToString();
            _runtimeResources.Add(resource.Id, resource);
        }

        public void FreePack(string packName)
        {
            Console.WriteLine($" > Diposing resources from Pack: {packName}");

            var res_ids = _pakMap[packName];

            for (int i = 0; i < res_ids.Length; ++i)
            {
                var res_id = res_ids[i];
                _loadedResources[res_id].Dispose();
                _loadedResources.Remove(res_id);
                Console.WriteLine($" > Disposed resource: {res_id}");
            }
        }

        public void Free(Resource resource)
        {
            if (_loadedResources.TryGetValue(resource.Id, out var asset))
            {
                asset.Dispose();

                _loadedResources.Remove(resource.Id);
            }
            else if (_runtimeResources.TryGetValue(resource.Id, out var asset2))
            {
                asset2.Dispose();

                _runtimeResources.Remove(resource.Id);
            }
        }

        internal void FreeEverything()
        {
            Console.WriteLine($" > Diposing {_loadedResources.Count} loaded resources.");

            foreach (var (resKey, resource) in _loadedResources)
            {
                Console.WriteLine($" > Diposing {resKey}.");
                resource.Dispose();
            }

            Console.WriteLine($" > Disposing {_runtimeResources.Count} runtime resources.");

            foreach (var (resKey, resource) in _runtimeResources)
            {
                Console.WriteLine($" > Diposing {resKey}.");
                resource.Dispose();
            }

            _loadedResources.Clear();
            _runtimeResources.Clear();
        }
    }
}
