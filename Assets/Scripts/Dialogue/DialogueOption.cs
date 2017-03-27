using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;


public class DialogueOption {

    int destID;
    string text;

    public DialogueOption() { } //Parameterless for XML Serialization
	
    public string getText()
    {
        return text;
    }

    public int getID()
    {
        return destID;
    }

    public DialogueOption(int id, string text)
    {
        destID = id;
        this.text = text;
    }
}
