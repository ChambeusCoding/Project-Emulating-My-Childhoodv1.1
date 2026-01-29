namespace Launcher.Core.Games;

public sealed class GameEntry
{
    public string Title { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;

    public string? System { get; set; }
    public string? EmulatorId { get; set; }
    public string? BoxArtPath { get; set; }
}
