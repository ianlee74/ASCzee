namespace ASCzee;

public class SongPromptGenerator
{
    public string GeneratePrompt(Presentation presentation, IReadOnlyList<string> notes, string genre, string? customization = null)
    {
        var contentSlides = presentation.Slides.Where(s => s.SlideType is SlideType.TitleSlide or SlideType.StandardSlide).ToList();
        var selectedItems = contentSlides.SelectMany(s => s.OptionItems).Where(o => o.IsSelected).Select(o => o.Text).Take(6).ToList();
        var highlights = contentSlides
            .SelectMany(s => s.BodyLines)
            .Where(line => !string.IsNullOrWhiteSpace(line) && !line.TrimStart().StartsWith("- [", StringComparison.Ordinal))
            .Take(6)
            .ToList();

        var noteHighlights = notes.Where(n => !string.IsNullOrWhiteSpace(n)).Take(6).ToList();

        var lines = new List<string>
        {
            "Write a motivational meeting summary song with a clear chorus and two short verses.",
            "Use a positive, collaborative tone suitable for a team recap.",
            $"Genre preference: {genre}.",
            "Incorporate the following presentation highlights:"
        };

        foreach (var item in highlights)
        {
            lines.Add($"- {item}");
        }

        if (selectedItems.Count > 0)
        {
            lines.Add("Include these chosen priorities:");
            foreach (var item in selectedItems)
            {
                lines.Add($"- {item}");
            }
        }

        if (noteHighlights.Count > 0)
        {
            lines.Add("Reflect these presenter notes:");
            foreach (var note in noteHighlights)
            {
                lines.Add($"- {note}");
            }
        }

        if (!string.IsNullOrWhiteSpace(customization))
        {
            lines.Add("Additional customization from presenter:");
            lines.Add($"- {customization.Trim()}");
        }

        lines.Add("Keep the output concise and suitable for pasting into suno.com.");

        return string.Join(Environment.NewLine, lines);
    }
}
