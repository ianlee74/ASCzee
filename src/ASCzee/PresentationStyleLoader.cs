namespace ASCzee;

public static class PresentationStyleLoader
{
    public static PresentationStyle Load(string presentationPath)
    {
        var presentationStylePath = Path.ChangeExtension(presentationPath, ".style");
        var defaultStylePathInPresentationDirectory = Path.Combine(
            Path.GetDirectoryName(presentationPath) ?? string.Empty,
            "default.style");
        var defaultStylePathInCurrentDirectory = Path.Combine(Environment.CurrentDirectory, "default.style");

        if (File.Exists(presentationStylePath))
        {
            return LoadFromFile(presentationStylePath, PresentationStyle.Default);
        }

        if (File.Exists(defaultStylePathInPresentationDirectory))
        {
            return LoadFromFile(defaultStylePathInPresentationDirectory, PresentationStyle.Default);
        }

        if (File.Exists(defaultStylePathInCurrentDirectory))
        {
            return LoadFromFile(defaultStylePathInCurrentDirectory, PresentationStyle.Default);
        }

        return PresentationStyle.Default;
    }

    public static PresentationStyle LoadFromFile(string stylePath, PresentationStyle baseStyle)
    {
        var map = new Dictionary<string, RgbColor>(StringComparer.OrdinalIgnoreCase)
        {
            ["# Header"] = baseStyle.Header1,
            ["## Header"] = baseStyle.Header2,
            ["Normal Text"] = baseStyle.NormalText,
            ["Hyperlink Text"] = baseStyle.HyperlinkText,
            ["Selector Color"] = baseStyle.SelectorColor,
            ["Selection Color"] = baseStyle.SelectionColor
        };

        foreach (var rawLine in File.ReadLines(stylePath))
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var separatorIndex = line.IndexOf(':');
            if (separatorIndex <= 0 || separatorIndex == line.Length - 1)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();
            if (!map.ContainsKey(key))
            {
                continue;
            }

            if (TryParseHex(value, out var color))
            {
                map[key] = color;
            }
        }

        return new PresentationStyle
        {
            Header1 = map["# Header"],
            Header2 = map["## Header"],
            NormalText = map["Normal Text"],
            HyperlinkText = map["Hyperlink Text"],
            SelectorColor = map["Selector Color"],
            SelectionColor = map["Selection Color"]
        };
    }

    private static bool TryParseHex(string value, out RgbColor color)
    {
        color = default;
        var hex = value.Trim();
        if (!hex.StartsWith('#'))
        {
            return false;
        }

        hex = hex[1..];

        if (hex.Length != 6)
        {
            return false;
        }

        if (!byte.TryParse(hex[..2], System.Globalization.NumberStyles.HexNumber, null, out var r)
            || !byte.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out var g)
            || !byte.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out var b))
        {
            return false;
        }

        color = new RgbColor(r, g, b);
        return true;
    }
}
