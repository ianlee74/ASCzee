namespace ASCzee;

public class NotesArtifactService
{
    public const string NotesHeading = "Presentation Notes";

    public string GetNotesPath(string sourcePath)
    {
        var directory = Path.GetDirectoryName(sourcePath) ?? string.Empty;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourcePath);
        var extension = Path.GetExtension(sourcePath);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".md";
        }

        var notesFileName = $"{fileNameWithoutExtension}.notes{extension}";
        return string.IsNullOrEmpty(directory)
            ? notesFileName
            : Path.Combine(directory, notesFileName);
    }

    public void Save(Presentation presentation, IReadOnlyList<string> notes)
    {
        var markdown = BuildAnnotatedMarkdown(presentation, notes);
        File.WriteAllText(presentation.NotesPath, markdown);
    }

    public void Delete(string notesPath)
    {
        if (File.Exists(notesPath))
        {
            File.Delete(notesPath);
        }
    }

    public string BuildAnnotatedMarkdown(Presentation presentation, IReadOnlyList<string> notes)
    {
        var lines = new List<string>();

        foreach (var slide in presentation.Slides.Where(s => s.SlideType is not SlideType.MainMenuSlide and not SlideType.NotesSlide))
        {
            var headingPrefix = slide.SlideType == SlideType.TitleSlide ? "#" : "##";
            lines.Add($"{headingPrefix} {slide.Title}".TrimEnd());

            foreach (var line in RenderBodyLines(slide))
            {
                lines.Add(line);
            }

            lines.Add(string.Empty);
        }

        lines.Add($"## {NotesHeading}");
        if (notes.Count == 0)
        {
            lines.Add("- No notes captured yet.");
        }
        else
        {
            foreach (var note in notes)
            {
                lines.Add($"- {note}");
            }
        }

        return string.Join(Environment.NewLine, lines);
    }

    public List<string> ExtractNotesFromPresentation(Presentation presentation)
    {
        var notesSlide = presentation.Slides.FirstOrDefault(s => s.SlideType == SlideType.NotesSlide || string.Equals(s.Title, NotesHeading, StringComparison.OrdinalIgnoreCase));
        if (notesSlide is null)
        {
            return [];
        }

        return notesSlide.BodyLines
            .Select(line => line.Trim())
            .Where(line => line.StartsWith("- ", StringComparison.Ordinal))
            .Select(line => line[2..].Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line) && !string.Equals(line, "No notes captured yet.", StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static IEnumerable<string> RenderBodyLines(Slide slide)
    {
        if (slide.OptionItems.Count == 0)
        {
            return slide.BodyLines;
        }

        var map = slide.OptionItems.ToDictionary(o => o.LineIndex);
        return slide.BodyLines.Select((line, index) => map.TryGetValue(index, out var option) ? PresentationParser.BuildTaskLine(option) : line);
    }
}
