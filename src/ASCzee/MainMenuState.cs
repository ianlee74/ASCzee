namespace ASCzee;

public enum MainMenuAction
{
    Exit,
    StartNew,
    CreateSong
}

public class MainMenuState
{
    private static readonly MainMenuAction[] ActionOrder =
    [
        MainMenuAction.Exit,
        MainMenuAction.StartNew,
        MainMenuAction.CreateSong
    ];

    public MainMenuAction FocusedAction { get; set; } = MainMenuAction.Exit;

    public IReadOnlyList<MainMenuAction> Actions => ActionOrder;

    public void MoveUp()
    {
        var currentIndex = Array.IndexOf(ActionOrder, FocusedAction);
        if (currentIndex <= 0)
        {
            FocusedAction = ActionOrder[^1];
            return;
        }

        FocusedAction = ActionOrder[currentIndex - 1];
    }

    public void MoveDown()
    {
        var currentIndex = Array.IndexOf(ActionOrder, FocusedAction);
        if (currentIndex == -1 || currentIndex >= ActionOrder.Length - 1)
        {
            FocusedAction = ActionOrder[0];
            return;
        }

        FocusedAction = ActionOrder[currentIndex + 1];
    }
}
