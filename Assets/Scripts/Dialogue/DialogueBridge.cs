using System;

public static class DialogueBridge
{
    public static event Action OnDialogueClosed;

    public static void CloseDialogue()
    {
        OnDialogueClosed?.Invoke();
    }
}