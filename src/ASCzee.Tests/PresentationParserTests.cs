namespace ASCzee.Tests;

public class PresentationParserTests
{
    [Fact]
    public void Parse_EmptyString_ReturnsEmptyPresentation()
    {
        var result = PresentationParser.Parse(string.Empty, "deck.md", "deck.notes.md");
        Assert.Single(result.Slides);
    }

    [Fact]
    public void Parse_TitleAndStandardHeading_CreatesTwoSlides()
    {
        var markup = "# Hello World\nIntro\n\n## Agenda\nPoint";
        var result = PresentationParser.Parse(markup, "deck.md", "deck.notes.md");

        Assert.Equal(2, result.Slides.Count);
        Assert.Equal("Hello World", result.Slides[0].Title);
        Assert.Equal(SlideType.TitleSlide, result.Slides[0].SlideType);
        Assert.Equal("Agenda", result.Slides[1].Title);
        Assert.Equal(SlideType.StandardSlide, result.Slides[1].SlideType);
    }

    [Fact]
    public void Parse_ThirdLevelHeading_StaysInBody()
    {
        var markup = "## Slide\n### Subheading\nBody";
        var result = PresentationParser.Parse(markup, "deck.md", "deck.notes.md");

        Assert.Single(result.Slides);
        Assert.Contains("### Subheading", result.Slides[0].BodyLines);
    }

    [Fact]
    public void Parse_TaskListItems_ExtractsOptionItems()
    {
        var markup = "## Slide\n- [ ] First\n- [x] Second";
        var result = PresentationParser.Parse(markup, "deck.md", "deck.notes.md");

        var options = result.Slides[0].OptionItems;
        Assert.Equal(2, options.Count);
        Assert.False(options[0].IsSelected);
        Assert.True(options[1].IsSelected);
    }

    [Fact]
    public void Parse_NotesHeading_SetsNotesSlideType()
    {
        var markup = "## Presentation Notes\n- one";
        var result = PresentationParser.Parse(markup, "deck.md", "deck.notes.md");

        Assert.Single(result.Slides);
        Assert.Equal(SlideType.NotesSlide, result.Slides[0].SlideType);
    }

    [Fact]
    public void Parse_NullInput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PresentationParser.Parse(null!, "deck.md", "deck.notes.md"));
    }
}

