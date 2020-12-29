using ProtoBuf;
using System.IO;
using System.Text.Json;

namespace OMEGA
{
    public static partial class ResourceLoader
    {
        private static string root_path;

        public static string GetFullResourcePath(string relative_res_path)
        {
            if (root_path == null)
            {
                return relative_res_path;
            }

            string full_path = Path.Combine(root_path, relative_res_path);

            if (Platform.RunningPlatform == RunningPlatform.Windows)
            {
                full_path = full_path.Replace('\\', '/');
            }

            return full_path;
        }

        public static void SetRootPath(string path)
        {
            root_path = path;
        }
        public static ResourcePak LoadPak(string pak_name)
        {
            var path = Path.Combine(root_path,
                !pak_name.Contains(".pak") ? pak_name + ".pak" : pak_name);

            using var file = File.OpenRead(path);

            ResourcePak pak = Serializer.Deserialize<ResourcePak>(file);

            return pak;
        }

        public static GameInfo LoadGameInfo()
        {
            var json_game_info_file = File.ReadAllText("project.json");

            GameInfo game_info_file = JsonSerializer.Deserialize<GameInfo>(json_game_info_file);

            return game_info_file;
        }

        public static GameAssetsManifest LoadGameAssetsManifest(string resources_folder)
        {
            var json_game_assets_manifest = File.ReadAllText(Path.Combine(resources_folder, "manifest.json"));

            GameAssetsManifest manifest = JsonSerializer.Deserialize<GameAssetsManifest>(json_game_assets_manifest);

            return manifest;
        }
    }
}
