using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour {

    private Dialogue dialogue;
    private DialogueNodeUI node;
    private VerticalLayoutGroup layout;

    public DialogueUI(Dialogue dialogue)
    {
        this.dialogue = dialogue;
    }

    public void setActiveNode(DialogueNode UInode)
    {
        node = new DialogueNodeUI(UInode);
    }

    public int getID()
    {
        return node.getID();
    }



}
