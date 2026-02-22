namespace ASCzee;

public class InputCapability
{
    public bool KeyboardEnabled { get; init; } = true;
    public bool MouseEnabled { get; init; }
}

public class PresentationSession
{
    public int CurrentSlideIndex { get; set; }
    public int? PreviousSlideIndexBeforeNotesJump { get; set; }
    public bool IsMainMenuOpen { get; set; }
    public MainMenuState MainMenuState { get; set; } = new();
    public List<string> Notes { get; set; } = [];
    public InputCapability InputCapability { get; init; } = new()
    {
        MouseEnabled = !Console.IsInputRedirected
            && (Environment.GetEnvironmentVariable("TERM")?.Contains("xterm", StringComparison.OrdinalIgnoreCase) ?? false)
    };
    public Dictionary<int, int> FocusBySlideIndex { get; } = new();

    public int GetFocusIndex(int slideIndex)
    {
        return FocusBySlideIndex.TryGetValue(slideIndex, out var index) ? index : 0;
    }

    public void SetFocusIndex(int slideIndex, int focusIndex)
    {
        FocusBySlideIndex[slideIndex] = focusIndex;
    }
}
