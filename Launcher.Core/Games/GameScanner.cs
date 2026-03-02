using Launcher.Core.Emulation;

namespace Launcher.Core.Games;

public sealed class GameScanner
{
    public EmulatorManager EmulatorManager { get; }

    public GameScanner(EmulatorManager emulators)
    {
        EmulatorManager = emulators;
    }
    public IEnumerable<GameEntry> Scan(string folder)
    {
        if (!Directory.Exists(folder))
            yield break;

        foreach (var file in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories))
        {
            if (!EmulatorManager.IsSupportedRom(file))
                continue;

            var emulator = EmulatorManager.FindForRom(file);
            if (emulator == null)
                continue;
            
            var game = new GameEntry
            {
                Title = Path.GetFileNameWithoutExtension(file),
                FilePath = file,
                System = emulator.Manifest.System,
                EmulatorId = emulator.Manifest.Id,
                BoxArtPath = "avares://Launcher.App/Assets/placeholder.png" // optional
            };

            yield return game;
        }
    }
}