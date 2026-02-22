using ASCzee;

if (args.Length == 0)
{
    Console.WriteLine("ASCzee - ASCII markdown presentation app");
    Console.WriteLine("Usage: asczee <presentation.md>");
    return 1;
}

var filePath = args[0];
if (!File.Exists(filePath))
{
    Console.Error.WriteLine($"Error: File not found: {filePath}");
    return 1;
}

if (!filePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
{
    Console.Error.WriteLine("Error: Input file must be a markdown file (.md).");
    return 1;
}

var notesService = new NotesArtifactService();
var notesPath = notesService.GetNotesPath(filePath);
var legacyNotesPath = $"{filePath}.notes.md";

if (!File.Exists(notesPath) && File.Exists(legacyNotesPath))
{
    try
    {
        File.Copy(legacyNotesPath, notesPath, overwrite: false);
    }
    catch
    {
        // If migration copy fails, continue and attempt legacy read below.
    }
}

string content;
if (File.Exists(notesPath))
{
    content = File.ReadAllText(notesPath);
}
else if (File.Exists(legacyNotesPath))
{
    content = File.ReadAllText(legacyNotesPath);
}
else
{
    content = File.ReadAllText(filePath);
}

var presentation = PresentationParser.Parse(content, filePath, notesPath);
var style = PresentationStyleLoader.Load(filePath);
var viewer = new PresentationViewer(presentation, notesService, new SongPromptGenerator(), style);
viewer.Run();
return 0;
