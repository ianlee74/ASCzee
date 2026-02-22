namespace ASCzee;

public readonly record struct RgbColor(byte R, byte G, byte B)
{
    public string ToAnsiForegroundCode() => $"\u001b[38;2;{R};{G};{B}m";
}

public class PresentationStyle
{
    public static PresentationStyle Default { get; } = new()
    {
        Header1 = new RgbColor(0, 255, 255),
        Header2 = new RgbColor(0, 128, 192),
        NormalText = new RgbColor(255, 255, 255),
        HyperlinkText = new RgbColor(0, 0, 255),
        SelectorColor = new RgbColor(255, 0, 0),
        SelectionColor = new RgbColor(0, 255, 0)
    };

    public RgbColor Header1 { get; init; }
    public RgbColor Header2 { get; init; }
    public RgbColor NormalText { get; init; }
    public RgbColor HyperlinkText { get; init; }
    public RgbColor SelectorColor { get; init; }
    public RgbColor SelectionColor { get; init; }
}
