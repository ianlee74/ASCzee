using System.Text.RegularExpressions;

namespace ASCzee;

/// <summary>
/// Interactive console viewer for an ASCzee <see cref="Presentation"/>.
/// </summary>
public class PresentationViewer(Presentation presentation, NotesArtifactService notesArtifactService, SongPromptGenerator songPromptGenerator, PresentationStyle style)
{
    private const int SunoPromptCharacterLimit = 1000;

    private enum SongPromptAction
    {
        OpenInNotepad,
        CopyToClipboard,
        OpenSunoCreate
    }

    private static readonly SongPromptAction[] SongPromptActions =
    [
        SongPromptAction.OpenInNotepad,
        SongPromptAction.CopyToClipboard,
        SongPromptAction.OpenSunoCreate
    ];

    private static readonly string[] PopularGenres =
    [
        "Pop",
        "Rock",
        "Hip-Hop / Rap",
        "R&B",
        "Country",
        "Jazz",
        "Blues",
        "Classical",
        "Electronic",
        "EDM / Dance",
        "Reggae",
        "Latin",
        "Folk",
        "Soul",
        "Funk",
        "Metal",
        "Punk",
        "Indie",
        "Alternative",
        "K-Pop",
        "Afrobeats",
        "House",
        "Techno",
        "Trance",
        "Lo-fi",
        "Ambient",
        "Other (custom)"
    ];

    private readonly Presentation _presentation = presentation;
    private readonly NotesArtifactService _notesArtifactService = notesArtifactService;
    private readonly SongPromptGenerator _songPromptGenerator = songPromptGenerator;
    private readonly PresentationStyle _style = style;
    private readonly PresentationSession _session = new();

    private string? _songPrompt;
    private string? _songPromptPath;
    private string _songGenre = "any genre";
    private bool _isSongPromptOpen;
    private int _songPromptFocusIndex;
    private string? _songPromptStatusMessage;
    private string? _slideStatusMessage;
    private readonly Dictionary<int, int> _rowToOptionIndex = [];
    private static readonly Regex HyperlinkRegex = new(@"\[(?<text>[^\]]+)\]\((?<url>[^)\s]+)\)", RegexOptions.Compiled);

    public void Run()
    {
        Console.Clear();

        if (_presentation.Slides.Count == 0)
        {
            Console.WriteLine("No slides to display.");
            return;
        }

        _session.Notes = _notesArtifactService.ExtractNotesFromPresentation(_presentation);
        EnsureNotesSlide();
        EnableMouseTracking();

        Console.CursorVisible = false;
        RenderCurrentView();

        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            var action = InputActionMapper.Map(key);

            if (_session.IsMainMenuOpen)
            {
                if (HandleMainMenuAction(action))
                {
                    break;
                }

                RenderCurrentView();
                continue;
            }

            if (HandlePresentationAction(action))
            {
                break;
            }

            RenderCurrentView();
        }

        DisableMouseTracking();
        Console.CursorVisible = true;
        Console.Clear();
    }

    private bool HandlePresentationAction(InputAction action)
    {
        if (_isSongPromptOpen)
        {
            return HandleSongPromptAction(action);
        }

        switch (action)
        {
            case InputAction.NextSlide:
                if (_session.CurrentSlideIndex < _presentation.Slides.Count - 1)
                {
                    _session.CurrentSlideIndex++;
                    _songPrompt = null;
                    _slideStatusMessage = null;
                }
                return false;

            case InputAction.PreviousSlide:
                if (_session.CurrentSlideIndex > 0)
                {
                    _session.CurrentSlideIndex--;
                    _slideStatusMessage = null;
                }
                return false;

            case InputAction.MoveOptionFocusUp:
                MoveFocus(-1);
                return false;

            case InputAction.MoveOptionFocusDown:
                MoveFocus(1);
                return false;

            case InputAction.ToggleOption:
                ToggleFocusedOption();
                return false;

            case InputAction.AddNote:
                CaptureNote();
                return false;

            case InputAction.JumpToNotes:
                JumpToNotes();
                return false;

            case InputAction.Escape:
                if (TryHandleMouseClickSequence())
                {
                    return false;
                }

                if (CurrentSlide().SlideType == SlideType.NotesSlide && _session.PreviousSlideIndexBeforeNotesJump.HasValue)
                {
                    _session.CurrentSlideIndex = _session.PreviousSlideIndexBeforeNotesJump.Value;
                    _session.PreviousSlideIndexBeforeNotesJump = null;
                }
                else
                {
                    _session.IsMainMenuOpen = true;
                }

                return false;

            case InputAction.Confirm:
                OpenFocusedHyperlink();
                return false;

            case InputAction.None:
            default:
                return false;
        }
    }

    private bool HandleMainMenuAction(InputAction action)
    {
        switch (action)
        {
            case InputAction.MoveOptionFocusUp:
                _session.MainMenuState.MoveUp();
                return false;

            case InputAction.MoveOptionFocusDown:
                _session.MainMenuState.MoveDown();
                return false;

            case InputAction.Confirm:
                return ExecuteMainMenuAction(_session.MainMenuState.FocusedAction);

            case InputAction.Escape:
                _session.IsMainMenuOpen = false;
                _songPrompt = null;
                return false;

            default:
                return false;
        }
    }

    private bool ExecuteMainMenuAction(MainMenuAction action)
    {
        switch (action)
        {
            case MainMenuAction.Exit:
                _session.IsMainMenuOpen = false;
                return true;

            case MainMenuAction.StartNew:
                DeleteArtifactIfExists(_presentation.NotesPath);
                DeleteArtifactIfExists(_songPromptPath ?? BuildSongPromptPath());
                _session.Notes.Clear();
                _songPrompt = null;
                _songPromptPath = null;
                _isSongPromptOpen = false;
                foreach (var slide in _presentation.Slides)
                {
                    foreach (var option in slide.OptionItems)
                    {
                        option.IsSelected = false;
                    }
                }

                RemoveNotesSlide();
                EnsureNotesSlide();
                _session.CurrentSlideIndex = 0;
                _session.PreviousSlideIndexBeforeNotesJump = null;
                _session.IsMainMenuOpen = false;
                PersistState();
                return false;

            case MainMenuAction.CreateSong:
                if (StartSongPromptFlow())
                {
                    _session.IsMainMenuOpen = false;
                }
                return false;

            default:
                return false;
        }
    }

    private void CaptureNote()
    {
        Console.CursorVisible = true;
        Console.SetCursorPosition(0, Math.Max(0, Console.WindowHeight - 2));
        Console.Write(new string(' ', Math.Max(10, Console.WindowWidth - 1)));
        Console.SetCursorPosition(0, Math.Max(0, Console.WindowHeight - 2));
        Console.Write("Note: ");
        var note = Console.ReadLine()?.Trim();
        Console.CursorVisible = false;

        if (string.IsNullOrWhiteSpace(note))
        {
            return;
        }

        _session.Notes.Add(note);
        EnsureNotesSlide();
        PersistState();
    }

    private void JumpToNotes()
    {
        var notesIndex = EnsureNotesSlide();
        _session.PreviousSlideIndexBeforeNotesJump = _session.CurrentSlideIndex;
        _session.CurrentSlideIndex = notesIndex;
    }

    private int EnsureNotesSlide()
    {
        var notesSlide = _presentation.Slides.FirstOrDefault(s => s.SlideType == SlideType.NotesSlide);
        if (notesSlide is null)
        {
            notesSlide = new Slide
            {
                Title = NotesArtifactService.NotesHeading,
                SlideType = SlideType.NotesSlide
            };
            _presentation.Slides.Add(notesSlide);
        }

        notesSlide.BodyLines = _session.Notes.Count == 0
            ? ["- No notes captured yet."]
            : _session.Notes.Select(note => $"- {note}").ToList();

        return _presentation.Slides.IndexOf(notesSlide);
    }

    private void RemoveNotesSlide()
    {
        _presentation.Slides.RemoveAll(s => s.SlideType == SlideType.NotesSlide);
    }

    private void MoveFocus(int delta)
    {
        var slide = CurrentSlide();
        var interactiveCount = slide.OptionItems.Count + slide.Hyperlinks.Count;
        if (interactiveCount == 0)
        {
            return;
        }

        var current = _session.GetFocusIndex(_session.CurrentSlideIndex);
        var next = (current + delta) % interactiveCount;
        if (next < 0)
        {
            next += interactiveCount;
        }

        _session.SetFocusIndex(_session.CurrentSlideIndex, next);
    }

    private void ToggleFocusedOption()
    {
        var slide = CurrentSlide();
        if (slide.OptionItems.Count == 0)
        {
            return;
        }

        var focus = _session.GetFocusIndex(_session.CurrentSlideIndex);
        if (focus < 0 || focus >= slide.OptionItems.Count)
        {
            return;
        }

        slide.OptionItems[focus].IsSelected = !slide.OptionItems[focus].IsSelected;
        PersistState();
    }

    private void OpenFocusedHyperlink()
    {
        var slide = CurrentSlide();
        if (slide.Hyperlinks.Count == 0)
        {
            return;
        }

        var focus = _session.GetFocusIndex(_session.CurrentSlideIndex);
        var linkIndex = focus - slide.OptionItems.Count;
        if (linkIndex < 0 || linkIndex >= slide.Hyperlinks.Count)
        {
            return;
        }

        var link = slide.Hyperlinks[linkIndex];
        DesktopActions.TryOpenUrl(link.Url, out var message);
        _slideStatusMessage = message;
    }

    private void PersistState()
    {
        try
        {
            _notesArtifactService.Save(_presentation, _session.Notes);
        }
        catch
        {
            // Runtime presentation should continue even if file persistence fails.
        }
    }

    private Slide CurrentSlide() => _presentation.Slides[_session.CurrentSlideIndex];

    private bool HandleSongPromptAction(InputAction action)
    {
        switch (action)
        {
            case InputAction.MoveOptionFocusUp:
                _songPromptFocusIndex = (_songPromptFocusIndex - 1 + SongPromptActions.Length) % SongPromptActions.Length;
                return false;

            case InputAction.MoveOptionFocusDown:
                _songPromptFocusIndex = (_songPromptFocusIndex + 1) % SongPromptActions.Length;
                return false;

            case InputAction.Confirm:
                ExecuteSongPromptAction(SongPromptActions[_songPromptFocusIndex]);
                return false;

            case InputAction.Escape:
                _isSongPromptOpen = false;
                _songPromptStatusMessage = null;
                return false;

            default:
                return false;
        }
    }

    private bool StartSongPromptFlow()
    {
        var genreSelection = PromptForGenreSelection();
        if (string.IsNullOrWhiteSpace(genreSelection))
        {
            _songPromptStatusMessage = "Song prompt creation canceled.";
            return false;
        }

        _songGenre = genreSelection;

        _songPrompt = _songPromptGenerator.GeneratePrompt(_presentation, _session.Notes, _songGenre);
        SaveSongPromptToFile();
        _songPromptFocusIndex = 0;
        _songPromptStatusMessage = null;
        _isSongPromptOpen = true;
        return true;
    }

    private string? PromptForGenreSelection()
    {
        var selectedIndex = 0;

        while (true)
        {
            Console.Clear();
            var width = Math.Max(20, Console.WindowWidth);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('═', width));
            Console.WriteLine(Center("SELECT SONG GENRE", width));
            Console.WriteLine(new string('═', width));
            Console.ResetColor();
            Console.WriteLine();

            for (var index = 0; index < PopularGenres.Length; index++)
            {
                var prefix = index == selectedIndex ? ">" : " ";
                if (index == selectedIndex)
                {
                    SetForeground(_style.SelectorColor);
                    Console.Write(prefix);
                    ResetForeground();
                    WriteLineNormal($" {PopularGenres[index]}");
                }
                else
                {
                    WriteLineNormal($"{prefix} {PopularGenres[index]}");
                }
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Use ↑/↓ and Enter to select. Press Esc to cancel.");
            Console.ResetColor();

            var action = InputActionMapper.Map(Console.ReadKey(intercept: true));
            switch (action)
            {
                case InputAction.MoveOptionFocusUp:
                    selectedIndex = (selectedIndex - 1 + PopularGenres.Length) % PopularGenres.Length;
                    break;

                case InputAction.MoveOptionFocusDown:
                    selectedIndex = (selectedIndex + 1) % PopularGenres.Length;
                    break;

                case InputAction.Confirm:
                    var selectedGenre = PopularGenres[selectedIndex];
                    if (selectedGenre.StartsWith("Other", StringComparison.OrdinalIgnoreCase))
                    {
                        var customGenre = PromptForInput("Enter custom genre: ");
                        return string.IsNullOrWhiteSpace(customGenre) ? "Pop" : customGenre.Trim();
                    }

                    return selectedGenre;

                case InputAction.Escape:
                    return null;
            }
        }
    }

    private void ExecuteSongPromptAction(SongPromptAction action)
    {
        switch (action)
        {
            case SongPromptAction.OpenInNotepad:
                SaveSongPromptToFile();
                if (string.IsNullOrWhiteSpace(_songPromptPath))
                {
                    _songPromptStatusMessage = "Unable to determine prompt file path.";
                    break;
                }

                DesktopActions.TryOpenFileInNotepad(_songPromptPath, out var notepadMessage);
                _songPromptStatusMessage = notepadMessage;
                break;

            case SongPromptAction.CopyToClipboard:
                DesktopActions.TryCopyToClipboard(_songPrompt ?? string.Empty, out var clipboardMessage);
                _songPromptStatusMessage = clipboardMessage;
                break;

            case SongPromptAction.OpenSunoCreate:
                DesktopActions.TryOpenUrl("https://suno.com/create", out var browserMessage);
                _songPromptStatusMessage = browserMessage;
                break;
        }
    }

    private string BuildSongPromptPath()
    {
        var sourcePath = _presentation.SourcePath;
        var directory = Path.GetDirectoryName(sourcePath) ?? string.Empty;
        var baseName = Path.GetFileNameWithoutExtension(sourcePath);
        var fileName = $"{baseName}.songprompt.txt";
        return string.IsNullOrWhiteSpace(directory) ? fileName : Path.Combine(directory, fileName);
    }

    private void SaveSongPromptToFile()
    {
        try
        {
            _songPromptPath = BuildSongPromptPath();
            File.WriteAllText(_songPromptPath, _songPrompt ?? string.Empty);
        }
        catch
        {
            _songPromptStatusMessage = "Unable to save prompt file.";
        }
    }

    private static void DeleteArtifactIfExists(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Ignore artifact deletion failures during session reset.
        }
    }

    private string BuildPromptLengthStatus(string prefix)
    {
        var length = (_songPrompt ?? string.Empty).Length;
        if (length > SunoPromptCharacterLimit)
        {
            return $"{prefix} Current length: {length}/{SunoPromptCharacterLimit} (over limit).";
        }

        return $"{prefix} Current length: {length}/{SunoPromptCharacterLimit}.";
    }

    private static string? PromptForInput(string prompt)
    {
        Console.Clear();
        Console.CursorVisible = true;
        Console.Write(prompt);
        var value = Console.ReadLine();
        Console.CursorVisible = false;
        return value;
    }

    private void RenderCurrentView()
    {
        Console.Clear();
        var width = Math.Max(20, Console.WindowWidth);

        if (_isSongPromptOpen)
        {
            RenderSongPromptPage(width);
            return;
        }

        if (_session.IsMainMenuOpen)
        {
            RenderMainMenu(width);
            return;
        }

        var slide = CurrentSlide();
        RenderSlide(slide, width);

        RenderStatusBar(width);
    }

    private void RenderSongPromptPage(int width)
    {
        var currentLength = (_songPrompt ?? string.Empty).Length;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('═', width));
        Console.WriteLine(Center("CREATE A SONG", width));
        Console.WriteLine(new string('═', width));
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine($"Genre: {_songGenre}");
        Console.WriteLine($"Suno prompt limit: {SunoPromptCharacterLimit} characters");
        if (currentLength > SunoPromptCharacterLimit)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Current length: {currentLength}/{SunoPromptCharacterLimit} (over limit)");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine($"Current length: {currentLength}/{SunoPromptCharacterLimit}");
        }
        Console.WriteLine($"Prompt file: {_songPromptPath ?? BuildSongPromptPath()}");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Prompt:");
        Console.ResetColor();
        WriteLineNormal(_songPrompt ?? string.Empty);
        Console.WriteLine();

        for (var index = 0; index < SongPromptActions.Length; index++)
        {
            var prefix = index == _songPromptFocusIndex ? ">" : " ";
            if (index == _songPromptFocusIndex)
            {
                SetForeground(_style.SelectorColor);
                Console.Write(prefix);
                ResetForeground();
                WriteLineNormal($" {FormatSongPromptAction(SongPromptActions[index])}");
            }
            else
            {
                WriteLineNormal($"{prefix} {FormatSongPromptAction(SongPromptActions[index])}");
            }
        }

        if (!string.IsNullOrWhiteSpace(_songPromptStatusMessage))
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(_songPromptStatusMessage);
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Use ↑/↓ and Enter. Press Esc to return.");
        Console.ResetColor();
    }

    private static string FormatSongPromptAction(SongPromptAction action)
    {
        return action switch
        {
            SongPromptAction.OpenInNotepad => "Open Prompt in Notepad",
            SongPromptAction.CopyToClipboard => "Copy Prompt to Clipboard",
            SongPromptAction.OpenSunoCreate => "Open suno.com/create",
            _ => action.ToString()
        };
    }

    private void RenderSlide(Slide slide, int width)
    {
        _rowToOptionIndex.Clear();

        var headerColor = slide.SlideType == SlideType.TitleSlide ? _style.Header1 : _style.Header2;
        SetForeground(headerColor);
        var borderHeight = slide.SlideType == SlideType.TitleSlide ? 2 : 1;
        var titleLines = AsciiBannerRenderer.Render(slide.Title, width, includeBorder: true, borderHeight: borderHeight).ToList();

        foreach (var line in titleLines)
        {
            Console.WriteLine(line);
        }

        ResetForeground();
        Console.WriteLine();

    var currentRow = Console.CursorTop;

        var optionMap = slide.OptionItems.ToDictionary(o => o.LineIndex);
        var focusIndex = _session.GetFocusIndex(_session.CurrentSlideIndex);

        for (var index = 0; index < slide.BodyLines.Count; index++)
        {
            if (!optionMap.TryGetValue(index, out var option))
            {
                WriteBodyLineWithHyperlinks(slide.BodyLines[index]);
                currentRow++;
                continue;
            }

            var marker = option.IsSelected ? "X" : " ";
            var optionIndex = slide.OptionItems.IndexOf(option);
            var prefix = optionIndex == focusIndex ? ">" : " ";
            WriteOptionLine(prefix, optionIndex == focusIndex, marker, option.Text);
            _rowToOptionIndex[currentRow + 1] = optionIndex;
            currentRow++;
        }

        if (slide.Hyperlinks.Count > 0)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Links:");
            Console.ResetColor();

            var focusIndexValue = _session.GetFocusIndex(_session.CurrentSlideIndex);
            for (var linkIndex = 0; linkIndex < slide.Hyperlinks.Count; linkIndex++)
            {
                var absoluteFocusIndex = slide.OptionItems.Count + linkIndex;
                var isFocused = absoluteFocusIndex == focusIndexValue;
                var link = slide.Hyperlinks[linkIndex];

                if (isFocused)
                {
                    SetForeground(_style.SelectorColor);
                    Console.Write(">");
                    ResetForeground();
                    WriteNormal($" [{linkIndex + 1}] ");
                    SetForeground(_style.HyperlinkText);
                    Console.Write(link.Text);
                    ResetForeground();
                    WriteNormal($" -> {link.Url}");
                    Console.WriteLine();
                }
                else
                {
                    WriteNormal($"  [{linkIndex + 1}] ");
                    SetForeground(_style.HyperlinkText);
                    Console.Write(link.Text);
                    ResetForeground();
                    WriteLineNormal($" -> {link.Url}");
                }
            }

            if (!string.IsNullOrWhiteSpace(_slideStatusMessage))
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(_slideStatusMessage);
                Console.ResetColor();
            }
        }
    }

    private void WriteBodyLineWithHyperlinks(string line)
    {
        var matches = HyperlinkRegex.Matches(line);
        if (matches.Count == 0)
        {
            WriteLineNormal(line);
            return;
        }

        var lastIndex = 0;
        foreach (Match match in matches)
        {
            if (match.Index > lastIndex)
            {
                WriteNormal(line[lastIndex..match.Index]);
            }

            var linkText = match.Groups["text"].Value;
            SetForeground(_style.HyperlinkText);
            Console.Write(linkText);
            ResetForeground();

            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < line.Length)
        {
            WriteNormal(line[lastIndex..]);
        }

        Console.WriteLine();
    }

    private bool TryHandleMouseClickSequence()
    {
        if (!_session.InputCapability.MouseEnabled)
        {
            return false;
        }

        if (!Console.KeyAvailable)
        {
            return false;
        }

        var sequence = new List<char>();
        while (Console.KeyAvailable)
        {
            sequence.Add(Console.ReadKey(intercept: true).KeyChar);
            var last = sequence[^1];
            if (last is 'm' or 'M')
            {
                break;
            }
        }

        var text = new string(sequence.ToArray());
        if (!text.StartsWith("[<", StringComparison.Ordinal))
        {
            return false;
        }

        var payload = text[2..].TrimEnd('m', 'M');
        var parts = payload.Split(';', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 3)
        {
            return false;
        }

        if (!int.TryParse(parts[2], out var row))
        {
            return false;
        }

        if (!_rowToOptionIndex.TryGetValue(row, out var optionIndex))
        {
            return false;
        }

        var slide = CurrentSlide();
        if (optionIndex < 0 || optionIndex >= slide.OptionItems.Count)
        {
            return false;
        }

        slide.OptionItems[optionIndex].IsSelected = !slide.OptionItems[optionIndex].IsSelected;
        _session.SetFocusIndex(_session.CurrentSlideIndex, optionIndex);
        PersistState();
        return true;
    }

    private void EnableMouseTracking()
    {
        if (_session.InputCapability.MouseEnabled)
        {
            Console.Write("\u001b[?1000h\u001b[?1006h");
        }
    }

    private void DisableMouseTracking()
    {
        if (_session.InputCapability.MouseEnabled)
        {
            Console.Write("\u001b[?1000l\u001b[?1006l");
        }
    }

    private void RenderMainMenu(int width)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string('═', width));
        Console.WriteLine(Center("MAIN MENU", width));
        Console.WriteLine(new string('═', width));
        Console.ResetColor();
        Console.WriteLine();

        foreach (var action in _session.MainMenuState.Actions)
        {
            var focused = action == _session.MainMenuState.FocusedAction;
            var prefix = focused ? ">" : " ";
            if (focused)
            {
                SetForeground(_style.SelectorColor);
                Console.Write(prefix);
                ResetForeground();
                WriteLineNormal($" {FormatAction(action)}");
            }
            else
            {
                WriteLineNormal($"{prefix} {FormatAction(action)}");
            }
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Use ↑/↓ and Enter. Press Esc to return.");
        Console.ResetColor();
    }

    private static string FormatAction(MainMenuAction action)
    {
        return action switch
        {
            MainMenuAction.Exit => "Exit",
            MainMenuAction.StartNew => "Start New",
            MainMenuAction.CreateSong => "Create a Song",
            _ => action.ToString()
        };
    }

    private void RenderStatusBar(int width)
    {
        var slidePosition = Math.Clamp(_session.CurrentSlideIndex + 1, 1, _presentation.Slides.Count);
        var status = $" Slide {slidePosition}/{_presentation.Slides.Count}  [← →] Navigate  [↑ ↓ Space] Options  [Ins] Note  [F1] Notes  [Esc] Menu ";
        if (CurrentSlide().Hyperlinks.Count > 0)
        {
            status = $" Slide {slidePosition}/{_presentation.Slides.Count}  [← →] Navigate  [↑ ↓] Focus  [Enter] Open Link  [Space] Toggle Option  [Ins] Note  [F1] Notes  [Esc] Menu ";
        }

        var row = Math.Max(0, Console.WindowHeight - 1);
        Console.SetCursorPosition(0, row);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(status.PadRight(Math.Max(width, status.Length)));
        Console.ResetColor();
    }

    private static string Center(string text, int width)
    {
        if (text.Length >= width) return text;
        var padding = (width - text.Length) / 2;
        return text.PadLeft(text.Length + padding);
    }

    private void SetForeground(RgbColor color)
    {
        Console.Write(color.ToAnsiForegroundCode());
    }

    private static void ResetForeground()
    {
        Console.Write("\u001b[39m");
    }

    private void WriteNormal(string text)
    {
        SetForeground(_style.NormalText);
        Console.Write(text);
        ResetForeground();
    }

    private void WriteLineNormal(string text)
    {
        SetForeground(_style.NormalText);
        Console.WriteLine(text);
        ResetForeground();
    }

    private void WriteOptionLine(string prefix, bool isFocused, string marker, string optionText)
    {
        if (isFocused)
        {
            SetForeground(_style.SelectorColor);
            Console.Write(prefix);
            ResetForeground();
        }
        else
        {
            WriteNormal(prefix);
        }

        WriteNormal(" [");
        if (string.Equals(marker, "X", StringComparison.Ordinal))
        {
            SetForeground(_style.SelectionColor);
            Console.Write("X");
            ResetForeground();
        }
        else
        {
            WriteNormal(" ");
        }

        WriteNormal($"] {optionText}");
        Console.WriteLine();
    }
}
