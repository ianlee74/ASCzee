namespace ASCzee;

/// <summary>
/// Represents an ASCzee presentation composed of one or more slides.
/// </summary>
public class Presentation
{
    public string SourcePath { get; init; } = string.Empty;
    public string NotesPath { get; init; } = string.Empty;
    public List<Slide> Slides { get; init; } = [];

    public string Title => Slides.FirstOrDefault(s => s.SlideType is not SlideType.NotesSlide)?.Title ?? string.Empty;
}
