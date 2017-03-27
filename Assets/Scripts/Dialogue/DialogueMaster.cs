using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class DialogueMaster : MonoBehaviour {

    private Dialogue dialogue;

    private GameObject window;
    private int selectedOption;

    private GameObject nodeText;
    private GameObject option01;
    private GameObject option02;
    private GameObject option03;

    public string pathToDialogueXML;

    public GameObject dialoguePreFab;

    private void Start()
    {
        //dialogue = Dialogue.loadDialogueFromXML(pathToDialogueXML/*Path.Combine(Application.dataPath, pathToDialogueXML)*/);
        //Debug.Log(Directory.GetCurrentDirectory() + pathToDialogueXML);
        //Debug.Log(dialogue.DialogueNodeList[0].getText());
        dialogue = new Dialogue();
        var n1 = new DialogueNode(0, "hello");
        n1.Add(new DialogueOption(-1, "I need to go"));
        n1.Add(new DialogueOption(1, "Give me a potion!"));
        n1.Add(new DialogueOption(2, "Would you kindly give me a potion?"));
        var n2 = new DialogueNode(1, "maybe");
        n2.Add(new DialogueOption(-1, "To hell with your maybe!"));
        n2.Add(new DialogueOption(-1, "I need to go now"));
        n2.Add(new DialogueOption(2, "Sorry, would you kindly give me a potion"));
        var n3 = new DialogueNode(2, "Here you go!");
        n3.Add(new DialogueOption(-1, "Thanks!"));
        n3.Add(new DialogueOption(-1, "I need to go now"));
        dialogue.Add(n1);
        dialogue.Add(n2);
        dialogue.Add(n3);
        

        var canvas = GameObject.Find("Canvas");
        window = Instantiate<GameObject>(dialoguePreFab);
        window.transform.parent = canvas.transform;
        window.transform.localPosition = new Vector3(0, 0, 0);

        nodeText = GameObject.Find("NodeText");
        option01 = GameObject.Find("Option01");
        option02 = GameObject.Find("Option02");
        option03 = GameObject.Find("Option03");

        window.SetActive(false);

    }

    private void runDialogue()
    {
        StartCoroutine("run");
        Debug.Log("Run start");
    }

    private void setSelectedOption(int i)
    {
        selectedOption = i;
        Debug.Log(i);
    }

    public IEnumerator run()
    {
        int nodeID = 0;
        Debug.Log(nodeID);
        window.SetActive(true);

        while(nodeID != -1)
        {
            displayNode(dialogue.DialogueNodeList[nodeID]);
            selectedOption = -2;
            while( selectedOption == -2)
            {
                yield return new WaitForSeconds(0.25f);
            }
            nodeID = selectedOption;
            Debug.Log("Node id updated");
            if (nodeID == 2)
                GameObject.Find("Player").GetComponent<PlayerHealth>().Heal(2);
                Debug.Log("Action");
        }
        window.SetActive(false);
        Debug.Log("Window setactive false");
    }

    private void displayNode(DialogueNode node)
    {
        Debug.Log("Setting buttons");
        nodeText.GetComponent<Text>().text = node.getText();


        option01.SetActive(false);
        option02.SetActive(false);
        option03.SetActive(false);
        Debug.Log("Entering display for");
        for(int i = 0; i<node.Options.Count || i < 2; i++)
        {
            switch (i)
            {
                case 0:
                    setOptionButton(option01, node.Options[i]);
                    Debug.Log(i);
                    break;
                case 1:
                    setOptionButton(option02, node.Options[i]);
                    Debug.Log(i);
                    break;
                case 2:
                    setOptionButton(option03, node.Options[i]);
                    Debug.Log(i);
                    break;
            }
        }
    }

    private void setOptionButton(GameObject button, DialogueOption opt)
    {
        button.SetActive(true);
        button.GetComponentInChildren<Text>().text = opt.getText();
        button.GetComponent<Button>().onClick.AddListener(delegate { setSelectedOption(opt.getID()); });
    }
}
