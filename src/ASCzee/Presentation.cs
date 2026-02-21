namespace ASCzee;

/// <summary>
/// Represents an ASCzee presentation composed of one or more slides.
/// </summary>
public class Presentation
{
    public IReadOnlyList<Slide> Slides { get; init; } = [];
    public string Title { get; init; } = string.Empty;
}
