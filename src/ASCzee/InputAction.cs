namespace ASCzee;

public enum InputAction
{
    None,
    NextSlide,
    PreviousSlide,
    MoveOptionFocusUp,
    MoveOptionFocusDown,
    ToggleOption,
    AddNote,
    JumpToNotes,
    Escape,
    Confirm
}

public static class InputActionMapper
{
    public static InputAction Map(ConsoleKeyInfo keyInfo)
    {
        return keyInfo.Key switch
        {
            ConsoleKey.RightArrow => InputAction.NextSlide,
            ConsoleKey.LeftArrow => InputAction.PreviousSlide,
            ConsoleKey.UpArrow => InputAction.MoveOptionFocusUp,
            ConsoleKey.DownArrow => InputAction.MoveOptionFocusDown,
            ConsoleKey.Spacebar => InputAction.ToggleOption,
            ConsoleKey.Insert => InputAction.AddNote,
            ConsoleKey.F1 => InputAction.JumpToNotes,
            ConsoleKey.Escape => InputAction.Escape,
            ConsoleKey.Enter => InputAction.Confirm,
            _ => InputAction.None
        };
    }
}
