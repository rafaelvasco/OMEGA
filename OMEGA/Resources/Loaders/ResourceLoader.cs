
using System.IO;
using System.Text.Json;


namespace OMEGA
{
    public static partial class ResourceLoader
    {
        private static string _rootPath;

        public static string GetFullResourcePath(string relativeResPath)
        {
            if (_rootPath == null)
            {
                return relativeResPath;
            }

            string full_path = Path.Combine(_rootPath, relativeResPath);

            if (Platform.RunningPlatform == RunningPlatform.Windows)
            {
                full_path = full_path.Replace('\\', '/');
            }

            return full_path;
        }

        public static void SetRootPath(string path)
        {
            _rootPath = path;
        }

        /// <summary>
        /// Used by Game to Load Game Assets Pak
        /// </summary>
        /// <param name="pak_name"></param>
        /// <returns></returns>
        public static ResourcePak LoadPak(string pakName)
        {
            var path = Path.Combine(_rootPath,
                !pakName.Contains(".pak") ? pakName + ".pak" : pakName);

            return PakLoader.Load(path);
        }

        /// <summary>
        /// Used by Game to Load Game Properties
        /// </summary>
        /// <returns>GameInfo</returns>
        public static GameInfo LoadGameInfo()
        {
            var json_game_info_file = File.ReadAllText("project.json");

            GameInfo game_info_file = JsonSerializer.Deserialize<GameInfo>(json_game_info_file);

            return game_info_file;
        }

        /// <summary>
        /// User by Assets Builder
        /// </summary>
        /// <param name="resources_folder"></param>
        /// <returns>GameAssetsManifest</returns>
        public static GameAssetsManifest LoadGameAssetsManifest(string resourcesFolder)
        {
            var json_game_assets_manifest = File.ReadAllText(Path.Combine(resourcesFolder, "manifest.json"));

            GameAssetsManifest manifest = JsonSerializer.Deserialize<GameAssetsManifest>(json_game_assets_manifest);

            return manifest;
        }
    }
}
