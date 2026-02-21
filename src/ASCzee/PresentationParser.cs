namespace ASCzee;

/// <summary>
/// Parses ASCzee markup into a <see cref="Presentation"/>.
///
/// Format:
///   - Slides are separated by lines containing only "---"
///   - The first line of a slide starting with "# " is the slide title
///   - Remaining lines form the slide content
/// </summary>
public static class PresentationParser
{
    private const string SlideSeparator = "---";
    private const string TitlePrefix = "# ";

    public static Presentation Parse(string markup)
    {
        ArgumentNullException.ThrowIfNull(markup);

        var blocks = markup
            .ReplaceLineEndings("\n")
            .Split($"\n{SlideSeparator}\n", StringSplitOptions.RemoveEmptyEntries);

        var slides = blocks
            .Select(ParseSlide)
            .ToList();

        var title = slides.FirstOrDefault()?.Title ?? string.Empty;

        return new Presentation { Title = title, Slides = slides };
    }

    private static Slide ParseSlide(string block)
    {
        var lines = block.Split('\n');
        var title = string.Empty;
        var contentLines = new List<string>();

        foreach (var line in lines)
        {
            if (title.Length == 0 && line.StartsWith(TitlePrefix, StringComparison.Ordinal))
            {
                title = line[TitlePrefix.Length..].Trim();
            }
            else
            {
                contentLines.Add(line);
            }
        }

        // Remove leading/trailing blank lines from content
        while (contentLines.Count > 0 && string.IsNullOrWhiteSpace(contentLines[0]))
            contentLines.RemoveAt(0);
        while (contentLines.Count > 0 && string.IsNullOrWhiteSpace(contentLines[^1]))
            contentLines.RemoveAt(contentLines.Count - 1);

        return new Slide
        {
            Title = title,
            Content = string.Join(Environment.NewLine, contentLines)
        };
    }
}
