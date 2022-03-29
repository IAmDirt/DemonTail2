using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using UnityEngine.EventSystems;

//origional https://github.com/RisingArtist/Ink-with-Unity-2019.3/blob/master/Assets/DialogueManager.cs
public class DialogueManager : Singleton<DialogueManager>
{
    public TextAsset inkFile;
    public GameObject customButton;
    public Animator PoleyTestAnim;
    public bool isTalking = false;

    static Story currentStory;

    [Header("UI refrence")]
    [SerializeField] TextMeshProUGUI dialogueName;
    [SerializeField] TextMeshProUGUI dialogueText;
    public GameObject optionPanel;
    public GameObject textBox;

    static Choice choiceSelected;
    public EventSystem eventSystem;


    //tags
    private const string SPEAKER_TAG = "Speaker";
    private const string PORTRAIT_TAG = "Portrait";
    private const string ADVANCE_TAG = "Advance";
    private const string HIDE_TAG = "Hide";
    private const string Clue_TAG = "Clue";

    void Start()
    {
        // story = new Story(inkFile.text);
        choiceSelected = null;
        textBox.SetActive(false);
    }
    public void StartDialogue(TextAsset newInkFile)
    {
        if (currentStory)
            if (currentStory.canContinue)
            {
                debugConsole("currentStory");
                showDialogueUI(true);
                inputAdvanceDialogue();
                return;
            }

        currentStory = new Story(newInkFile.text);
        showDialogueUI(true);
        inputAdvanceDialogue();
        gameManager.Instance.startDialogue();
    }
    public void showDialogueUI(bool bo)
    {
        isHidden = !bo;
        textBox.SetActive(bo);
        optionPanel.SetActive(bo);
    }
    private bool isHidden;
    public void hideDialogue()
    {
        isHidden = true;
        showDialogueUI(false);
    }
    public void inputAdvanceDialogue()
    {
        showPromt(false);

        if (isHidden)
            return;
        //Is there more to the story?
        if (currentStory.canContinue)
        {
            AdvanceDialogue();


            Debug.Log("1");

        }
        else if (currentStory.currentChoices.Count != 0) //Are there any choices?
        {

            Debug.Log("a");
            StartCoroutine(ShowChoices());
        }
        else if (!_choice)
        {
            FinishDialogue();
            gameManager.Instance.returnToGameplay();
            Debug.Log("b");
        }
    }
    private void FinishDialogue()
    {
        debugConsole("End of Dialogue!");
        showDialogueUI(false);
    }
    void AdvanceDialogue()
    {
        string currentSentence = currentStory.Continue();
        HandleTags(currentStory.currentTags);
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
    }
    public GameObject promt;
    // Type out the sentence letter by letter and make character idle if they were talking

    public void showPromt(bool bo)
    {
        if(bo)
        {
            promt.SetActive(true);
            promt.transform.localScale = Vector3.one * 0.01f;
            LeanTween.scale(promt, Vector3.one , 0.1f).setEaseInOutBack();
        }
        else
        {
            LeanTween.scale(promt, Vector3.one * 0.01f, 0.1f).setEaseInOutBack().setOnComplete(disablePromt);
        }

    }
    private void disablePromt()
    {
        promt.SetActive(false);
    }
    IEnumerator TypeSentence(string sentence)
    {

        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        showPromt(true);
         yield return null;
    }

    // Create then show the choices on the screen until one got selected
    private bool _choice = false;
    IEnumerator ShowChoices()
    {
        debugConsole("There are choices need to be made here!");
        List<Choice> _choices = currentStory.currentChoices;
        _choice = true;
        Button firstButton = null;
        for (int i = 0; i < _choices.Count; i++)
        {

            GameObject temp = Instantiate(customButton, optionPanel.transform);
            temp.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _choices[i].text;
            temp.AddComponent<Selectable>();
            temp.GetComponent<Selectable>().element = _choices[i];
            temp.GetComponent<Button>().onClick.AddListener(() => { temp.GetComponent<Selectable>().Decide(); });
            if (i == 0)
            {
                firstButton = temp.GetComponent<Button>();
            }

        }

        optionPanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        if (firstButton) firstButton.GetComponent<Button>().Select();

        yield return new WaitUntil(() => { return choiceSelected != null; });

        AdvanceFromDecision();
    }

    // Tells the story which branch to go to
    public static void SetDecision(object element)
    {
        choiceSelected = (Choice)element;
        currentStory.ChooseChoiceIndex(choiceSelected.index);
    }

    // After a choice was made, turn off the panel and advance from that choice
    void AdvanceFromDecision()
    {
        _choice = false;
        optionPanel.SetActive(false);
        for (int i = 0; i < optionPanel.transform.childCount; i++)
        {
            Destroy(optionPanel.transform.GetChild(i).gameObject);
        }
        choiceSelected = null; // Forgot to reset the choiceSelected. Otherwise, it would select an option without player intervention.
        AdvanceDialogue();
    }

    /*** Tag Parser ***/
    /// In Inky, you can use tags which can be used to cue stuff in a game.
    /// This is just one way of doing it. Not the only method on how to trigger events. 
    /// 
    public ClipBoardBooleans clipboardBools;
    void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            //parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length < 2)
            {
                Debug.LogError("Tag could not be appropriatly parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            //handle the tag

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    debugConsole("speaker=" + tagValue);
                    dialogueName.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    debugConsole("Portrait=" + tagValue);
                    PoleyTestAnim.Play(tagValue);
                    break;
                case ADVANCE_TAG:
                    debugConsole("advance=" + tagValue);
                    gameManager.Instance.ResumeTimeline();
                    gameManager.Instance.skipNextPause();
                   // inputAdvanceDialogue();
                    break;
                case HIDE_TAG:
                    debugConsole("Hide=" + tagValue);
                    hideDialogue();
                    break;
                case Clue_TAG:

                    //split dialogue 
                    //
                    //  string[] splitValue = tagValue.Split(',');

                    // string clueKey = splitValue[0].Trim();
                    // string clueValue = splitValue[1].Trim();
                    string ClueValue = splitTag[2].Trim(); // Location, Boss, ETC...

                     clipboardBools.setBool(int.Parse(tagValue), ClueValue);

                    //change blackboard 
                    debugConsole("Clue=" + tagValue);
                    break;
            }
        }
    }
    public bool DialogueDebug = false;

    public void debugConsole(string output)
    {
        if (DialogueDebug)
            Debug.Log(output);
    }



    void SetAnimation(string _name)
    {
        //   CharacterScript cs = GameObject.FindObjectOfType<CharacterScript>();
        //   cs.PlayAnimation(_name);
    }
    /* void SetTextColor(string _color)
     {
         switch (_color)
         {
             case "red":
                 message.color = Color.red;
                 break;
             case "blue":
                 message.color = Color.cyan;
                 break;
             case "green":
                 message.color = Color.green;
                 break;
             case "white":
                 message.color = Color.white;
                 break;
             default:
                 Debug.Log($"{_color} is not available as a text color");
                 break;
         }
     }*/
}