using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;


public class DialogueMaster : MonoBehaviour {

    public GameObject dialoguePreFab;
    public int maxInteractionCount;
    public GameObject Itemtogive;

    private int currentInteractionCount;
    private Dialogue dialogue;
    private GameObject window;
    private int selectedOption;
    private GameObject nodeText;
    private GameObject option01;
    private GameObject option02;
    private GameObject option03;
    private int option01ID, option02ID, option03ID;

    //public string pathToDialogueXML;

    

    private void Start()
    {
        //dialogue = Dialogue.loadDialogueFromXML(pathToDialogueXML/*Path.Combine(Application.dataPath, pathToDialogueXML)*/);
        //Debug.Log(Directory.GetCurrentDirectory() + pathToDialogueXML);
        //Debug.Log(dialogue.DialogueNodeList[0].getText());

        // TODO add external loading of dialog text
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
        

        var canvas = GameObject.Find("DialogueCanvas");
        window = Instantiate<GameObject>(dialoguePreFab);
        window.transform.SetParent(canvas.transform);
        window.transform.localPosition = new Vector3(0, 0, 0);

        nodeText = GameObject.Find("NodeText");
        option01 = GameObject.Find("Option01");
        option02 = GameObject.Find("Option02");
        option03 = GameObject.Find("Option03");
        //EventSystem.current.SetSelectedGameObject(option01, new BaseEventData(EventSystem.current));
        

        window.SetActive(false);
    }

    private void Update()
    {
        /*if (Input.GetButtonDown("Option1"))
        {
            setSelectedOption(option01ID);
        }
        else if (Input.GetButtonDown("Option2"))
        {
            setSelectedOption(option02ID);
        }
        else if (Input.GetButtonDown("Option3"))
        {
            setSelectedOption(option03ID);
        }*/
    }

    public void runDialogue()
    {
        if (currentInteractionCount >= maxInteractionCount)
            return;
        //GameObject.FindGameObjectWithTag("Player").GetComponent<Input2D>().enabled = false;
        StartCoroutine("run");
    }

    public void setSelectedOption(int i)
    {
        selectedOption = i;
    }

    public IEnumerator run()
    {
        var gm = GameManager.Instance;
        gm.Paused = true;
        int nodeID = 0;
        window.SetActive(true);
        

        while(nodeID != -1)
        {
            displayNode(dialogue.DialogueNodeList[nodeID]);
            selectedOption = -2;
            while( selectedOption == -2)
            {
                yield return StartCoroutine(WaitForRealSeconds(0.25f));
            }
            nodeID = selectedOption;
            if (nodeID == 2)
                spawnHealth(); // TODO spawn health potions instead
        }
        currentInteractionCount++;
        
        gm.Paused = false;
        window.SetActive(false);
    }

    public IEnumerator WaitForRealSeconds(float f)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + f)
        {
            yield return null;
        }
    }

    private void displayNode(DialogueNode node)
    {
        nodeText.GetComponent<Text>().text = node.getText();
        // rescale the window depending on how many options we have (also reposition text correctly)
        window.GetComponent<RectTransform>().sizeDelta = new Vector2(window.GetComponent<RectTransform>().sizeDelta.x, node.Options.Count * 100);
        nodeText.GetComponent<RectTransform>().anchoredPosition = new Vector2(nodeText.GetComponent<RectTransform>().anchoredPosition.x, -(nodeText.GetComponent<RectTransform>().rect.height / 2 + 25));

        option01.SetActive(false);
        option02.SetActive(false);
        option03.SetActive(false);
        for(int i = 0; i<node.Options.Count || i < 2; i++)
        {
            switch (i)
            {
                case 0:
                    setOptionButton(option01, node.Options[i]);
                    option01ID = node.Options[i].getID();
                    break;
                case 1:
                    setOptionButton(option02, node.Options[i]);
                    option02ID = node.Options[i].getID();
                    break;
                case 2:
                    setOptionButton(option03, node.Options[i]);
                    option03ID = node.Options[i].getID();
                    break;
            }
        }
        EventSystem.current.SetSelectedGameObject(option01);
    }

    private void setOptionButton(GameObject button, DialogueOption opt)
    {
        button.SetActive(true);
        button.GetComponentInChildren<Text>().text = opt.getText();
        button.GetComponent<Button>().onClick.AddListener(delegate { setSelectedOption(opt.getID()); });
    }

    private void spawnHealth()
    {
        var copy = Instantiate(Itemtogive, (transform.position + Vector3.right*5), Quaternion.identity);
        

    }
}
