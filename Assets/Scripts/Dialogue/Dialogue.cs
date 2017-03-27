using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class Dialogue{

    public List<DialogueNode> DialogueNodeList;

   // private DialogueUI dialogueWindow;


    public static Dialogue loadDialogueFromXML(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Dialogue));
        StreamReader reader = new StreamReader(path);

        Dialogue dialogue = (Dialogue)serializer.Deserialize(reader);

        return dialogue;

    }

    public Dialogue()
    {

        DialogueNodeList = new List<DialogueNode>();
        //dialogueWindow = new DialogueUI(this);

    } //Parameterless for XML Serialization

    public void Add(DialogueNode node)
    {
        DialogueNodeList.Add(node);
    }

    

    /*public void Update()
    {
        dialogueWindow.getID();
    }*/


}
