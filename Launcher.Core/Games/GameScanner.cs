using System.IO;

namespace Launcher.Core.Games;

public class GameScanner
{
    public IEnumerable<GameEntry> Scan(string folder)
    {
        foreach (var file in Directory.EnumerateFiles(folder))
        {
            yield return new GameEntry
            {
                Title = Path.GetFileNameWithoutExtension(file),
                FilePath = file
            };
        }
    }
}