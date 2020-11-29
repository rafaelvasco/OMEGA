using OMEGA;
using PowerArgs;
using System;
using System.IO;
using System.Text.Json;

namespace OMEGACLI.Build
{
    public class BuildActionArgs
    {
        [ArgRequired(PromptIfMissing = true), ArgDescription("Game Folder"), ArgPosition(1), ArgShortcut("-p")]
        public string GameFolder { get; set; }
    }

    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class BuilderExecutor
    {
        [HelpHook, ArgShortcut("-?"), ArgDescription("Shows Usage")]
        public bool Help { get; set; }

        [ArgActionMethod, ArgDescription("Builds App Assets")]
        public void Build(BuildActionArgs args)
        {
            try
            {
                var json_game_info = File.ReadAllText(Path.Combine(args.GameFolder, "project.json"));

                GameInfo game_info = JsonSerializer.Deserialize<GameInfo>(json_game_info);

                var resources_folder = Path.Combine(args.GameFolder, game_info.ResourcesFolder);

                GameAssetsManifest assets_manifest = ResourceLoader.LoadGameAssetsManifest(resources_folder);

                Builder.BuildGame(resources_folder, assets_manifest);
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
