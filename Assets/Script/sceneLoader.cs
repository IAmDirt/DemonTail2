using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
public class sceneLoader : MonoBehaviour
{
    PlayerInputs input;
    clipboardUIManager _clipBoardManager;
    public int lvlToLoad;
    public GameObject ButtonPromet;

    public bool useRestriction;
    public void Start()
    {
    }
    private bool disableHitbox;
    public void OnTriggerEnter(Collider other)
    {
        if(disableHitbox)
        return;
        if (input == null)
        {
            var inputPlayer = other.GetComponent<InputPlayer>();
            if (inputPlayer)
            {
                _clipBoardManager = inputPlayer.ClipBoardManager;
                input = inputPlayer.input;
            }
        }

        if (other.gameObject.layer == 10)
        {
            if (input != null)
            {
                Invoke("setInput", 1.5f);
                gameManager.Instance.startDialogue();
                openUI();

            }
        }
    }

    #region input
    //refrence https://forum.unity.com/threads/how-to-remove-and-inputaction-phase-callback.894601/
    public void setInput()
    {
        input.Dialogue.Select.performed += loadSceneAction;
        input.Dialogue.Back.performed += deselectAction;

        ButtonPromet.transform.localScale = Vector3.one * 0.1f;
        ButtonPromet.SetActive(true);
        LeanTween.scale(ButtonPromet, Vector3.one, 0.3f).setEaseOutBack();
    }

    private void loadSceneAction(InputAction.CallbackContext ctx)//need this to add and move actions to input
    {
        // do the thing
        deselect();
        loadScene();
    }
    private void deselectAction(InputAction.CallbackContext ctx)//need this to add and move actions to input
    {
        // do the thing
        deselect();
    }
    #endregion

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            disableHitbox = false;
        }
    }

    public void loadScene()
    {
        if (useRestriction )
        {
            if (!_clipBoardManager.ClueRestrictionsMet())   
            {
                disableHitbox = true;
                deselect();
                return;
            }
        }

        input.Dialogue.Select.performed -= loadSceneAction;
        input.Dialogue.Back.performed -= deselectAction;
        gameManager.Instance.loadScene(lvlToLoad);
        gameManager.Instance.returnToGameplay();

        LeanTween.scale(ButtonPromet, Vector3.one * 1.5f, 0.5f).setEasePunch();
    }

    public void openUI()
    {
        if(useRestriction) _clipBoardManager.OpenUI();
    }
    public void deselect()
    {
        input.Dialogue.Select.performed -= loadSceneAction;
        input.Dialogue.Back.performed -= deselectAction;

        ButtonPromet.SetActive(false);
        gameManager.Instance.returnToGameplay();
       if(useRestriction) _clipBoardManager.hideMenu();
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {


        }
    }


}