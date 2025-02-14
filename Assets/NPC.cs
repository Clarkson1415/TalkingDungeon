using UnityEngine;

public class NPC : MonoBehaviour, IHasDialogue
{
    private string[] dialogue;
    private string[][] dialogueOptions;

    public string[] GetDialogue()
    {
        return this.dialogue;
    }

    public string[][] GetDialogueOptions()
    {
        return this.dialogueOptions;
    }

    // Start is called before the first frame update
    void Start()
    {
        dialogue = new string[1] { "HEY" };
        dialogueOptions = new string[][]
        {
            new string[] {"bye", "what?"},
        };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
