namespace Launcher.Core.Games;

public class GameEntry
{
    public string Title { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    // Optional but future-proof
    public string? EmulatorId { get; set; }
    public string? Platform { get; set; }
}