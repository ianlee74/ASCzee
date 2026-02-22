namespace ASCzee.Tests;

public class PresentationStyleLoaderTests
{
    [Fact]
    public void Load_UsesPresentationStyle_WhenPresentationStyleExists()
    {
        using var sandbox = new TestSandbox();
        var presentationPath = Path.Combine(sandbox.Path, "deck.md");
        File.WriteAllText(presentationPath, "# demo");
        File.WriteAllText(Path.Combine(sandbox.Path, "deck.style"), "Selector Color: #010203");
        File.WriteAllText(Path.Combine(sandbox.Path, "default.style"), "Selector Color: #090909");

        var style = PresentationStyleLoader.Load(presentationPath);

        Assert.Equal(new RgbColor(1, 2, 3), style.SelectorColor);
    }

    [Fact]
    public void Load_UsesDefaultStyle_WhenPresentationStyleDoesNotExist()
    {
        using var sandbox = new TestSandbox();
        var presentationPath = Path.Combine(sandbox.Path, "deck.md");
        File.WriteAllText(presentationPath, "# demo");
        File.WriteAllText(Path.Combine(sandbox.Path, "default.style"), "Hyperlink Text: #0A141E");

        var style = PresentationStyleLoader.Load(presentationPath);

        Assert.Equal(new RgbColor(10, 20, 30), style.HyperlinkText);
    }

    [Fact]
    public void LoadFromFile_IgnoresInvalidValues_AndKeepsBaseDefaults()
    {
        using var sandbox = new TestSandbox();
        var stylePath = Path.Combine(sandbox.Path, "sample.style");
        File.WriteAllText(stylePath, """
            # Header: #112233
            ## Header: not-a-color
            Normal Text: #08090A
            Hyperlink Text: 1,2
            Selector Color: #FF00FF
            Selection Color: 1000,0,0
            """);

        var style = PresentationStyleLoader.LoadFromFile(stylePath, PresentationStyle.Default);

        Assert.Equal(new RgbColor(0x11, 0x22, 0x33), style.Header1);
        Assert.Equal(PresentationStyle.Default.Header2, style.Header2);
        Assert.Equal(new RgbColor(8, 9, 10), style.NormalText);
        Assert.Equal(PresentationStyle.Default.HyperlinkText, style.HyperlinkText);
        Assert.Equal(new RgbColor(255, 0, 255), style.SelectorColor);
        Assert.Equal(PresentationStyle.Default.SelectionColor, style.SelectionColor);
    }

    private sealed class TestSandbox : IDisposable
    {
        public string Path { get; }

        public TestSandbox()
        {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"ASCzee.Tests.{Guid.NewGuid():N}");
            Directory.CreateDirectory(Path);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(Path, recursive: true);
            }
            catch
            {
            }
        }
    }
}
