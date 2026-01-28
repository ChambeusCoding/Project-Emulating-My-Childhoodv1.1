using Launcher.Core.Emulation;

namespace Launcher.Core.Games;

public sealed class GameScanner
{
    public EmulatorManager EmulatorManager { get; }  // expose for ViewModel

    public GameScanner(EmulatorManager emulators)
    {
        EmulatorManager = emulators;
    }

    /// <summary>
    /// Scans the folder for ROM files supported by any registered emulator.
    /// </summary>
    /// <param name="folder">Folder to scan.</param>
    /// <returns>Enumerable of GameEntry objects.</returns>
    public IEnumerable<GameEntry> Scan(string folder)
    {
        if (!Directory.Exists(folder))
            yield break;

        foreach (var file in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories))
        {
            // Skip unsupported files
            if (!EmulatorManager.IsSupportedRom(file))
                continue;

            var emulator = EmulatorManager.FindForRom(file);
            if (emulator == null)
                continue;

            // Create game entry
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