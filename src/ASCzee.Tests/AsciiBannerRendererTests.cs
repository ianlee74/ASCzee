namespace ASCzee.Tests;

public class AsciiBannerRendererTests
{
    [Fact]
    public void Render_QuestionMarkAndParens_DoesNotUseFallbackGlyph()
    {
        var lines = AsciiBannerRenderer.Render("?( )", 120, includeBorder: false).ToList();

        Assert.Equal(5, lines.Count);
        Assert.DoesNotContain(lines, line => line.Contains("?"));
    }

    [Fact]
    public void Render_BasicPunctuation_DoesNotUseFallbackGlyph()
    {
        var lines = AsciiBannerRenderer.Render("!.,:;'\"+-=/\\", 160, includeBorder: false).ToList();

        Assert.Equal(5, lines.Count);
        Assert.DoesNotContain(lines, line => line.Contains("?"));
    }
}