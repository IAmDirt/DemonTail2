using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using UnityEngine.EventSystems;

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
                Debug.Log("currentStory");
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
        if (isHidden)
            return;
        //Is there more to the story?
        if (currentStory.canContinue)
        {
            AdvanceDialogue();


            //Are there any choices?
            if (currentStory.currentChoices.Count != 0)
            {
                StartCoroutine(ShowChoices());
            }
        }
        else if (!_choice)
        {
            FinishDialogue();
            gameManager.Instance.returnToGameplay();
        }
    }
    private void FinishDialogue()
    {
        Debug.Log("End of Dialogue!");
        showDialogueUI(false);
    }
    void AdvanceDialogue()
    {
        string currentSentence = currentStory.Continue();
        HandleTags(currentStory.currentTags);
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
    }

    // Type out the sentence letter by letter and make character idle if they were talking
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        //  CharacterScript tempSpeaker = GameObject.FindObjectOfType<CharacterScript>();
        //if (tempSpeaker.isTalking)
        // {
        //    SetAnimation("idle");
        // }
        yield return null;
    }

    // Create then show the choices on the screen until one got selected
    private bool _choice = false;
    IEnumerator ShowChoices()
    {
        Debug.Log("There are choices need to be made here!");
        List<Choice> _choices = currentStory.currentChoices;
        _choice = true;
        Button firstButton = null;
        for (int i = 0; i < _choices.Count; i++)
        {

            GameObject temp = Instantiate(customButton, optionPanel.transform);
            temp.transform.GetChild(0).GetComponent<Text>().text = _choices[i].text;
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
    void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            //parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)

            {
                Debug.LogError("Tag could not be appropriatly parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            //handle the tag

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    Debug.Log("speaker=" + tagValue);
                    dialogueName.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    Debug.Log("Portrait=" + tagValue);
                    PoleyTestAnim.Play(tagValue);
                    break;
                case ADVANCE_TAG:
                    Debug.Log("advance=" + tagValue);
                    gameManager.Instance.ResumeTimeline();
                    gameManager.Instance.skipNextPause();
                   // inputAdvanceDialogue();
                    break;
                case HIDE_TAG:
                    Debug.Log("Hide=" + tagValue);
                    hideDialogue();
                    break;

                    
            }
        }
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