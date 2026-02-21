namespace ASCzee.Tests;

public class PresentationParserTests
{
    [Fact]
    public void Parse_EmptyString_ReturnsEmptyPresentation()
    {
        var result = PresentationParser.Parse(string.Empty);
        Assert.Empty(result.Slides);
    }

    [Fact]
    public void Parse_SingleSlideWithTitle_ExtractsTitle()
    {
        var markup = "# Hello World\n\nSome content here.";
        var result = PresentationParser.Parse(markup);

        Assert.Single(result.Slides);
        Assert.Equal("Hello World", result.Slides[0].Title);
        Assert.Equal("Some content here.", result.Slides[0].Content);
    }

    [Fact]
    public void Parse_MultipleSlides_ReturnsCorrectCount()
    {
        var markup = "# Slide 1\n\nContent 1\n\n---\n# Slide 2\n\nContent 2";
        var result = PresentationParser.Parse(markup);

        Assert.Equal(2, result.Slides.Count);
    }

    [Fact]
    public void Parse_UsesFirstSlideTitleAsPresentationTitle()
    {
        var markup = "# My Presentation\n\nIntro\n\n---\n# Second Slide\n\nBody";
        var result = PresentationParser.Parse(markup);

        Assert.Equal("My Presentation", result.Title);
    }

    [Fact]
    public void Parse_SlideWithNoTitle_HasEmptyTitle()
    {
        var markup = "Just some content without a title.";
        var result = PresentationParser.Parse(markup);

        Assert.Single(result.Slides);
        Assert.Equal(string.Empty, result.Slides[0].Title);
    }

    [Fact]
    public void Parse_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PresentationParser.Parse(null!));
    }
}

