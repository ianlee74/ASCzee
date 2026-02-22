using System.Text;

namespace ASCzee;

public static class AsciiBannerRenderer
{
    private const int GlyphHeight = 5;

    private static readonly Dictionary<char, string[]> Glyphs = new()
    {
        ['A'] = [" ### ", "#   #", "#####", "#   #", "#   #"],
        ['B'] = ["#### ", "#   #", "#### ", "#   #", "#### "],
        ['C'] = [" ####", "#    ", "#    ", "#    ", " ####"],
        ['D'] = ["#### ", "#   #", "#   #", "#   #", "#### "],
        ['E'] = ["#####", "#    ", "#### ", "#    ", "#####"],
        ['F'] = ["#####", "#    ", "#### ", "#    ", "#    "],
        ['G'] = [" ####", "#    ", "# ###", "#   #", " ####"],
        ['H'] = ["#   #", "#   #", "#####", "#   #", "#   #"],
        ['I'] = ["#####", "  #  ", "  #  ", "  #  ", "#####"],
        ['J'] = ["#####", "   # ", "   # ", "#  # ", " ##  "],
        ['K'] = ["#   #", "#  # ", "###  ", "#  # ", "#   #"],
        ['L'] = ["#    ", "#    ", "#    ", "#    ", "#####"],
        ['M'] = ["#   #", "## ##", "# # #", "#   #", "#   #"],
        ['N'] = ["#   #", "##  #", "# # #", "#  ##", "#   #"],
        ['O'] = [" ### ", "#   #", "#   #", "#   #", " ### "],
        ['P'] = ["#### ", "#   #", "#### ", "#    ", "#    "],
        ['Q'] = [" ### ", "#   #", "#   #", "#  ##", " ####"],
        ['R'] = ["#### ", "#   #", "#### ", "#  # ", "#   #"],
        ['S'] = [" ####", "#    ", " ### ", "    #", "#### "],
        ['T'] = ["#####", "  #  ", "  #  ", "  #  ", "  #  "],
        ['U'] = ["#   #", "#   #", "#   #", "#   #", " ### "],
        ['V'] = ["#   #", "#   #", "#   #", " # # ", "  #  "],
        ['W'] = ["#   #", "#   #", "# # #", "## ##", "#   #"],
        ['X'] = ["#   #", " # # ", "  #  ", " # # ", "#   #"],
        ['Y'] = ["#   #", " # # ", "  #  ", "  #  ", "  #  "],
        ['Z'] = ["#####", "   # ", "  #  ", " #   ", "#####"],
        ['0'] = [" ### ", "#  ##", "# # #", "##  #", " ### "],
        ['1'] = ["  #  ", " ##  ", "  #  ", "  #  ", " ### "],
        ['2'] = [" ### ", "#   #", "   # ", "  #  ", "#####"],
        ['3'] = ["#####", "   # ", "  ## ", "   # ", "#####"],
        ['4'] = ["#   #", "#   #", "#####", "    #", "    #"],
        ['5'] = ["#####", "#    ", "#### ", "    #", "#### "],
        ['6'] = [" ### ", "#    ", "#### ", "#   #", " ### "],
        ['7'] = ["#####", "   # ", "  #  ", " #   ", "#    "],
        ['8'] = [" ### ", "#   #", " ### ", "#   #", " ### "],
        ['9'] = [" ### ", "#   #", " ####", "    #", " ### "],
        ['-'] = ["     ", "     ", "#####", "     ", "     "],
        [' '] = ["   ", "   ", "   ", "   ", "   "]
    };

    public static IEnumerable<string> Render(string text, int width, bool includeBorder = true, int borderHeight = 1)
    {
        var safe = string.IsNullOrWhiteSpace(text) ? "UNTITLED" : text.Trim().ToUpperInvariant();
        var chunks = WrapByWord(safe, Math.Max(1, (width - 2) / 6));
        var output = new List<string>();
        borderHeight = Math.Max(1, borderHeight);

        if (includeBorder)
        {
            for (var i = 0; i < borderHeight; i++)
            {
                output.Add(new string('═', Math.Max(10, width)));
            }
        }

        for (var index = 0; index < chunks.Count; index++)
        {
            var chunk = chunks[index];
            for (var row = 0; row < GlyphHeight; row++)
            {
                var builder = new StringBuilder();
                for (var c = 0; c < chunk.Length; c++)
                {
                    if (c > 0)
                    {
                        builder.Append(' ');
                    }

                    builder.Append(GetGlyph(chunk[c])[row]);
                }

                output.Add(Center(builder.ToString(), width));
            }

            if (index < chunks.Count - 1)
            {
                output.Add(string.Empty);
            }
        }

        if (includeBorder)
        {
            for (var i = 0; i < borderHeight; i++)
            {
                output.Add(new string('═', Math.Max(10, width)));
            }
        }

        return output;
    }

    private static IReadOnlyList<string> GetGlyph(char value)
    {
        return Glyphs.TryGetValue(value, out var glyph)
            ? glyph
            : ["#####", "# ? #", "# ? #", "# ? #", "#####"];
    }

    private static List<string> WrapByWord(string text, int maxCharsPerLine)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var chunks = new List<string>();
        var current = string.Empty;

        foreach (var word in words)
        {
            if (word.Length > maxCharsPerLine)
            {
                if (!string.IsNullOrWhiteSpace(current))
                {
                    chunks.Add(current);
                    current = string.Empty;
                }

                for (var i = 0; i < word.Length; i += maxCharsPerLine)
                {
                    chunks.Add(word.Substring(i, Math.Min(maxCharsPerLine, word.Length - i)));
                }

                continue;
            }

            var candidate = string.IsNullOrWhiteSpace(current) ? word : $"{current} {word}";
            if (candidate.Length <= maxCharsPerLine)
            {
                current = candidate;
            }
            else
            {
                chunks.Add(current);
                current = word;
            }
        }

        if (!string.IsNullOrWhiteSpace(current))
        {
            chunks.Add(current);
        }

        if (chunks.Count == 0)
        {
            chunks.Add("UNTITLED");
        }

        return chunks;
    }

    private static string Center(string text, int width)
    {
        if (text.Length >= width)
        {
            return text;
        }

        var padding = (width - text.Length) / 2;
        return new string(' ', padding) + text;
    }
}
