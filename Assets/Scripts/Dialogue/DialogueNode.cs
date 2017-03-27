using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;


public class DialogueNode {

    int nodeID;

    string text;

    public List<DialogueOption> Options;

    public DialogueNode() //Parameterless used for XML Serialization
    {

        Options = new List<DialogueOption>();

    }

    public string getText()
    {
        return text;
    }

    public DialogueNode(int id, string text)
    {
        Options = new List<DialogueOption>();
        nodeID = id;
        this.text = text;
    }

    public void Add(DialogueOption option)
    {

        if(Options.Count < 3)
        {
            Options.Add(option);
        }

    }

    
    
	
}
