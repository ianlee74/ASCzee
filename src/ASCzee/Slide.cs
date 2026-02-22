namespace ASCzee;

public enum SlideType
{
    TitleSlide,
    StandardSlide,
    NotesSlide,
    MainMenuSlide
}

public class OptionBoxItem
{
    public int LineIndex { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}

/// <summary>
/// Represents a single slide in an ASCzee presentation.
/// </summary>
public class Slide
{
    public string Title { get; set; } = string.Empty;
    public SlideType SlideType { get; set; } = SlideType.StandardSlide;
    public List<string> BodyLines { get; set; } = [];
    public List<OptionBoxItem> OptionItems { get; set; } = [];

    public string Content => string.Join(Environment.NewLine, BodyLines);
}
