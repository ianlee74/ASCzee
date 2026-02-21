using ASCzee;

if (args.Length == 0)
{
    Console.WriteLine("ASCzee - ASCII markup driven presentation app");
    Console.WriteLine("Usage: ASCzee <presentation.ascz>");
    return 1;
}

var filePath = args[0];
if (!File.Exists(filePath))
{
    Console.Error.WriteLine($"Error: File not found: {filePath}");
    return 1;
}

var content = File.ReadAllText(filePath);
var presentation = PresentationParser.Parse(content);
var viewer = new PresentationViewer(presentation);
viewer.Run();
return 0;
