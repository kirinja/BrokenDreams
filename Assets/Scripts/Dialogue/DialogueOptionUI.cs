using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueOptionUI : MonoBehaviour {

    private DialogueOption option;
    private bool picked;
    private string text;
    
   
    

    public DialogueOptionUI(DialogueOption option)
    {
        this.option = option;
        text = option.getText();
        picked = false;
        
        
    }

    public void pick()
    {
        picked = true;
    }

    public bool getPicked()
    {
        return picked;
    }

    public int getID()
    {
        return option.getID();
    }

    
}
