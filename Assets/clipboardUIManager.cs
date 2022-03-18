using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[System.Serializable]    //used for tutorial information when going into bossfight
public class clueVisualization
{
    public Sprite sprite1;
    public Sprite sprite2;
    [TextArea]
    public string clueDescription;
}
public class clipboardUIManager : MonoBehaviour
{
    public enum DictionaryKey { Location, Boss }
    [System.Serializable]
    public class clue
    {

        public DictionaryKey dictionaryKey;
        public bool isEnabled;
        [TextArea]
        public string ClueText;

        public bool AlreadyDisplayed;
        [Header("restrictions")]
        public clueVisualization visualization;
    }



    public ClipBoardBooleans clipBoardsBooleans;
    public GameObject cluePrefab;
    public GameObject clueLayoutParent;
    public GameObject clueparent;
    public GameObject ClueParent_Hidden;
    public clue[] clueList;

    [Header("restricitons")]
    public Transform restrictionDropdown;
    public Transform restrictionDropdownPositions;

    public void Start()
    {
        updateUI();
        clipBoardsBooleans.manager = this;

        clueparent.transform.localScale = Vector3.one * 0.1f;
        clueLayoutParent.SetActive(false);

        hiddenStartPos = ClueParent_Hidden.transform.localPosition;

        //tutorial menu
        tutorialStartPos = tutorialVisualization.transform.localPosition;
        hideMenu();
    }

    #region UI animation
    private bool isOpen = false;

    private bool waitInput;
    public void InputDelay()
    {
        waitInput = false;
    }

    private Vector3 hiddenStartPos;
    public void inputClipBoard()
    {
        if (waitInput)
            return;
        if (isOpen)
        {
            closeClipBoard();
        }
        else
        {
            openClipBoard();
        }
        waitInput = true;
        Invoke("InputDelay", 0.35f);
    }
    public void openClipBoard()
    {
        Promt.SetActive(false);
        isOpen = true;

        LeanTween.moveLocal(ClueParent_Hidden, hiddenStartPos - Vector3.up * 70, 0.3f).setEaseInOutBack();

        LeanTween.moveLocal(clueparent, Vector3.zero, 0.35f).setEaseOutBack().setDelay(0.15f);
        LeanTween.scale(clueparent, Vector3.one, 0.45f).setEaseOutBack().setDelay(0.15f);
        LeanTween.rotateZ(clueparent, 12, 0.4f).setEasePunch().setDelay(0.25f);


        clueLayoutParent.SetActive(true);
    }

    public void closeClipBoard()
    {
        isOpen = false;
        LeanTween.moveLocal(ClueParent_Hidden, hiddenStartPos, 0.7f).setEaseInOutElastic();

        LeanTween.moveLocal(clueparent, new Vector2(400, -400), 0.7f).setEaseOutElastic();
        LeanTween.scale(clueparent, Vector3.one * 0.1f, 0.3f).setEaseOutBack();
        LeanTween.rotateZ(clueparent, 15, 0.2f).setEasePunch();

        clueLayoutParent.SetActive(false);

    }
    public GameObject Promt;
    public RandomAudioPlayer newClueSound;
    public void newClue()
    {
        newClueSound.PlayRandomClip();
        Promt.SetActive(true);
        LeanTween.moveLocalY(ClueParent_Hidden, hiddenStartPos.y + 20, 1.4f).setEaseInOutElastic();
    }
    #endregion

    #region updating UI info
    public void updateUI()
    {
        foreach (var clue in clueList)
        {
            var key = clue.dictionaryKey.ToString().Trim();

            bool newValue = false;
            newValue = clipBoardsBooleans.clueCollection1.boolHolder[key];

            setClue(clue, newValue);
        }
    }

    public void setClue(clue Clue, bool bo)
    {
        /* if (i > clueList.Length)
         {
             Debug.LogWarning("clue has exeded list " + i);
             return;
         }*/
        if (!bo || Clue.isEnabled)
            return;

        Invoke("newClue", 1.5f);
        var wantedClue = Clue;

        spawnPrefab(wantedClue, bo);
    }

    public void spawnPrefab(clue newClue, bool bo)
    {
        var spawned = Instantiate(cluePrefab, clueLayoutParent.transform);

        newClue.isEnabled = true;

        var mainText = spawned.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        mainText.text = newClue.ClueText;

        var checkBox = spawned.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        checkBox.text = bo ? "V" : "O";
    }
    #endregion

    #region tutorial Visualisation
    [Header("ClueRestriciton")]

    //not enough clues ui popup
    public TextAsset inkFile;
    public DialogueManager dialogueManager;

    public bool ClueRestrictionsMet()
    {
        if (_currentCount >= 1)
        {
            return true;
        }

        //gameManager.Instance.returnToGameplay();
        Invoke("DisplayRequirement", 0.01f);
        return false;
    }
    public void DisplayRequirement()
    {
        dialogueManager.StartDialogue(inkFile);
        //maybe I should gather more clues
    }

    public GameObject tutorialPrefab;

    //display all clues on screen popup
    public GameObject tutorialVisualization;
    private Vector3 tutorialStartPos;

    public TextMeshProUGUI ClueCount;
    private int _currentCount;

    public void OpenUI()
    {
        openClipBoard();
        LeanTween.moveLocal(clueparent, new Vector3(250, 0, 0), 0.4f).setEaseOutBack().setDelay(0.7f);
        LeanTween.moveLocal(tutorialVisualization, tutorialStartPos, 0.4f).setEaseOutBack().setDelay(0.7f);
        StartCoroutine(checkCurrentClues());
    }

    public IEnumerator checkCurrentClues()
    {
        yield return new WaitForSeconds(1.25f);
        //show ui right

        foreach (var Clue in clueList)
        {
             if (!Clue.isEnabled || Clue.AlreadyDisplayed)
             yield return null;


            Clue.AlreadyDisplayed = true;

            //set prefab information
            var newPrefab = tutorialPrefab;
            newPrefab.GetComponent<cluePrefabBehavior>().setVisuals(Clue.visualization);

            displayClue(tutorialPrefab);

            //update count;
            _currentCount++;
            ClueCount.text = _currentCount.ToString() + "/ 3";


            yield return new WaitForSeconds(0.35f);
        }
        //is clue already displayed on right
        //if not
        //animate get clues into tips


        //enable "enter casino" button when enough clues
    }
    public void displayClue(GameObject CluePrefab)
    {
        //animate clueParent
        LeanTween.rotateZ(clueparent, 8, 0.3f).setEasePunch().setDelay(0.1f);

        
        var spawned = Instantiate(CluePrefab, ClueParent_Hidden.transform.position, CluePrefab.transform.rotation, restrictionDropdown.parent.parent);
        var Position = restrictionDropdownPositions.GetChild(_currentCount);

        var spawnedChild = spawned.transform.GetChild(0).gameObject;

        LeanTween.moveY(spawnedChild, Position.position.y, 2).setEaseOutElastic().setOnComplete(() => setParent(spawned, restrictionDropdown));
        LeanTween.moveX(spawned, Position.position.x, 0.5f).setEaseOutBack().setDelay(0.15f);

        spawnedChild.transform.localScale = Vector3.one * 0.1f;
        LeanTween.scale(spawnedChild, Vector3.one, 0.3f).setEaseOutBack();
        LeanTween.rotateZ(spawnedChild, 15, 0.3f).setEasePunch().setDelay(0.2f);
    }

    public void setParent(GameObject go, Transform parent)
    {
        go.transform.parent = parent;
        go.transform.GetChild(0).localPosition = Vector3.zero;
    }

    public void hideMenu()
    {
        closeClipBoard();
        LeanTween.moveLocal(tutorialVisualization, new Vector3(-850, 0, 0), 1f).setEaseOutElastic().setDelay(0.25f);
    }

    #endregion
}