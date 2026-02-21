namespace ASCzee;

/// <summary>
/// Interactive console viewer for an ASCzee <see cref="Presentation"/>.
///
/// Navigation:
///   Right arrow / Space / Enter  – next slide
///   Left arrow / Backspace       – previous slide
///   Q / Escape                   – quit
/// </summary>
public class PresentationViewer(Presentation presentation)
{
    private readonly Presentation _presentation = presentation;
    private int _currentIndex;

    public void Run()
    {
        if (_presentation.Slides.Count == 0)
        {
            Console.WriteLine("No slides to display.");
            return;
        }

        Console.CursorVisible = false;
        RenderSlide();

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key is ConsoleKey.Q or ConsoleKey.Escape)
                break;

            if (key.Key is ConsoleKey.RightArrow or ConsoleKey.Spacebar or ConsoleKey.Enter)
            {
                if (_currentIndex < _presentation.Slides.Count - 1)
                {
                    _currentIndex++;
                    RenderSlide();
                }
            }
            else if (key.Key is ConsoleKey.LeftArrow or ConsoleKey.Backspace)
            {
                if (_currentIndex > 0)
                {
                    _currentIndex--;
                    RenderSlide();
                }
            }
        }

        Console.CursorVisible = true;
        Console.Clear();
    }

    private void RenderSlide()
    {
        Console.Clear();
        var slide = _presentation.Slides[_currentIndex];
        var width = Console.WindowWidth;

        // Title bar
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(Center(slide.Title, width));
        Console.ResetColor();
        Console.WriteLine(new string('─', width));

        // Content
        Console.WriteLine();
        Console.WriteLine(slide.Content);

        // Status bar
        var status = $" Slide {_currentIndex + 1}/{_presentation.Slides.Count}  [← →] Navigate  [Q] Quit ";
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(status.PadRight(width));
        Console.ResetColor();
    }

    private static string Center(string text, int width)
    {
        if (text.Length >= width) return text;
        var padding = (width - text.Length) / 2;
        return text.PadLeft(text.Length + padding);
    }
}
