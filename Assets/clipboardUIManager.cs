using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class clipboardUIManager : MonoBehaviour
{
    public enum DictionaryKey { Location, Boss}
    [System.Serializable]
    public class clue
    {

        public DictionaryKey dictionaryKey;
        public bool isEnabled;
        [TextArea]
        public string ClueText;
    }

    public ClipBoardBooleans clipBoardsBooleans;
    public GameObject cluePrefab;
    public GameObject clueLayoutParent;
    public GameObject clueparent;
    public GameObject ClueParent_Hidden;
    public clue[] clueList;

    public void Start()
    {
        updateUI();
        clipBoardsBooleans.manager = this;

        clueparent.transform.localScale = Vector3.one * 0.1f;
        clueLayoutParent.SetActive(false);

        hiddenStartPos = ClueParent_Hidden.transform.localPosition;
    }

    #region UI animation
    private bool isOpen = false;


    private Vector3 hiddenStartPos;
    public void inputClipBoard()
    {
        if (isOpen)
        {
            close();
        }
        else
        {
            open();
        }

    }
    public void open()
    {
        Promt.SetActive(false);
        isOpen = true;

        LeanTween.moveLocal(ClueParent_Hidden, hiddenStartPos - Vector3.up * 70, 0.3f).setEaseInOutBack();

        LeanTween.moveLocal(clueparent, Vector3.zero, 1f).setEaseOutElastic().setDelay(0.15f);
        LeanTween.scale(clueparent, Vector3.one, 0.45f).setEaseOutBack().setDelay(0.15f);
        LeanTween.rotateZ(clueparent, 12, 0.4f).setEasePunch().setDelay(0.25f);


        clueLayoutParent.SetActive(true);
    }

    public void close()
    {
        isOpen = false;
        LeanTween.moveLocal(ClueParent_Hidden, hiddenStartPos, 0.7f).setEaseInOutElastic();

        LeanTween.moveLocal(clueparent, new Vector2(400, -400), 0.7f).setEaseOutElastic();
        LeanTween.scale(clueparent, Vector3.one* 0.1f, 0.3f).setEaseOutBack();
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

       Invoke( "newClue", 1.5f);
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
}