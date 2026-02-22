using System.Text.RegularExpressions;

namespace ASCzee;

/// <summary>
/// Parses markdown into a <see cref="Presentation"/>.
///
/// Rules:
///   - `#` creates title slides.
///   - `##` creates standard slides.
///   - `###` and deeper remain in-slide body content.
///   - GitHub-style task list items are parsed as option boxes.
/// </summary>
public static class PresentationParser
{
    private static readonly Regex SlideHeadingRegex = new(@"^(#{1,2})\s+(.+)$", RegexOptions.Compiled);
    private static readonly Regex TaskListRegex = new(@"^\s*[-*]\s\[( |x|X)\]\s+(.+)$", RegexOptions.Compiled);

    public static Presentation Parse(string markup, string sourcePath, string notesPath)
    {
        ArgumentNullException.ThrowIfNull(markup);

        var normalized = markup.ReplaceLineEndings("\n");
        var lines = normalized.Split('\n');

        var sections = new List<(string Heading, int Level, List<string> BodyLines)>();
        string currentHeading = string.Empty;
        var currentLevel = 0;
        var currentBody = new List<string>();

        foreach (var line in lines)
        {
            var headingMatch = SlideHeadingRegex.Match(line);
            if (headingMatch.Success)
            {
                var level = headingMatch.Groups[1].Value.Length;
                if (level <= 2)
                {
                    if (currentLevel > 0 || currentBody.Count > 0)
                    {
                        sections.Add((currentHeading, currentLevel, [.. currentBody]));
                    }

                    currentHeading = headingMatch.Groups[2].Value.Trim();
                    currentLevel = level;
                    currentBody.Clear();
                    continue;
                }
            }

            currentBody.Add(line);
        }

        if (currentLevel > 0 || currentBody.Count > 0)
        {
            sections.Add((currentHeading, currentLevel, currentBody));
        }

        if (sections.Count == 0)
        {
            sections.Add((string.Empty, 2, []));
        }

        var slides = sections.Select(ParseSection).ToList();

        foreach (var slide in slides)
        {
            if (string.Equals(slide.Title, NotesArtifactService.NotesHeading, StringComparison.OrdinalIgnoreCase))
            {
                slide.SlideType = SlideType.NotesSlide;
            }
        }

        if (slides.Count == 0)
        {
            slides.Add(new Slide { Title = "Untitled", SlideType = SlideType.StandardSlide });
        }

        return new Presentation
        {
            SourcePath = sourcePath,
            NotesPath = notesPath,
            Slides = slides
        };
    }

    private static Slide ParseSection((string Heading, int Level, List<string> BodyLines) section)
    {
        var optionItems = new List<OptionBoxItem>();
        var bodyLines = section.BodyLines.ToList();

        for (var index = 0; index < bodyLines.Count; index++)
        {
            var match = TaskListRegex.Match(bodyLines[index]);
            if (!match.Success)
            {
                continue;
            }

            optionItems.Add(new OptionBoxItem
            {
                LineIndex = index,
                Text = match.Groups[2].Value.Trim(),
                IsSelected = !string.Equals(match.Groups[1].Value, " ", StringComparison.Ordinal)
            });

            bodyLines[index] = BuildTaskLine(optionItems[^1]);
        }

        TrimOuterBlankLines(bodyLines, optionItems);

        return new Slide
        {
            Title = section.Heading,
            SlideType = section.Level == 1 ? SlideType.TitleSlide : SlideType.StandardSlide,
            BodyLines = bodyLines,
            OptionItems = optionItems
        };
    }

    private static void TrimOuterBlankLines(List<string> lines, List<OptionBoxItem> options)
    {
        while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[0]))
        {
            lines.RemoveAt(0);
            foreach (var option in options)
            {
                option.LineIndex--;
            }
        }

        while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1]))
        {
            lines.RemoveAt(lines.Count - 1);
        }
    }

    public static string BuildTaskLine(OptionBoxItem option)
    {
        var marker = option.IsSelected ? "X" : " ";
        return $"- [{marker}] {option.Text}";
    }
}
