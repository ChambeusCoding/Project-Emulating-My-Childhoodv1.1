namespace Launcher.Core.Games;

public class GameEntry
{
    public string Title { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string System { get; set; } = string.Empty;
    public string? EmulatorId { get; set; }
    public string? BoxArtPath { get; set; } // optional for your Phase 3 UI
}