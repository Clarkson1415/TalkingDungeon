using UnityEngine;

/// <summary>
/// Represents a class that shows the battle text progress at the top of the screen.
/// </summary>
public class BattleTextBox : Textbox
{
    private void Update()
    {
        switch (State)
        {
            case BoxState.INACTIVE:
                break;
            case BoxState.WRITINGSLIDE:
                if (this.InteractFlag)
                {
                    this.SkipToEnd();
                    this.State = BoxState.WAITINGONSLIDE;
                }
                break;
            case BoxState.WAITINGONSLIDE:
                if (this.InteractFlag)
                {
                    this.State = BoxState.INACTIVE;
                }
                break;
            default:
                State = BoxState.INACTIVE;
                break;
        }

        ResetAllFlags();
    }
}