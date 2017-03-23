using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class Dialogue {

    List<DialogueNode> DialogueNodeList;


    public static Dialogue loadDialogueFromXML(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Dialogue));
        StreamReader reader = new StreamReader(path);

        Dialogue dialogue = (Dialogue)serializer.Deserialize(reader);

        return dialogue;

    }


}
