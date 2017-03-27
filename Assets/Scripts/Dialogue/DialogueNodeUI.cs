using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNodeUI : MonoBehaviour {

    private DialogueNode node;
    private string text;

    private List<DialogueOptionUI> options;

	public DialogueNodeUI(DialogueNode node)
    {
        this.node = node;
        text = node.getText();
       // fillOptions(node.getOptions());
        
    }

    private void fillOptions(List<DialogueOption> options)
    {
        
        for(int i = 0; i<options.Count; i++)
        {
            new DialogueOptionUI(options[i]);
        }

    }

    public int getID()
    {
        int ID = -2;
        for(int i = 0; i<options.Count; i++)
        {
            if (options[i].getPicked())
            {
                ID = options[i].getID();
            }
        }
        return ID;
    }


}
