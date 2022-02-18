using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneLoader : MonoBehaviour
{
    PlayerInputs input;
    public int lvlToLoad;
    public GameObject ButtonPromet;
    public void OnTriggerEnter(Collider other)
    {
        if (input == null)
        {
            {
                var inputPlayer = other.GetComponent<InputPlayer>();
                if (inputPlayer)
                {
                    input = inputPlayer.input;
                }
            }

            if (other.gameObject.layer == 10)
            {
                if (input != null)
                {
                    input.Dialogue.Select.performed += ctx => loadScene();
                    input.Dialogue.Back.performed += ctx => deselect();
                    ButtonPromet.SetActive(true);
                    gameManager.Instance.startDialogue();

                }
            }
        }
    }

    public void loadScene()
    {
        input.Dialogue.Select.performed -= ctx => loadScene();
        input.Dialogue.Back.performed -= ctx => deselect();
        gameManager.Instance.loadScene(lvlToLoad);
            gameManager.Instance.returnToGameplay();

        LeanTween.scale(ButtonPromet, Vector3.one * 1.5f, 0.5f).setEasePunch();
    }

    public void deselect()
    {
        input.Dialogue.Select.performed -= ctx => loadScene();
        input.Dialogue.Back.performed -= ctx => deselect();

            ButtonPromet.SetActive(false);
            gameManager.Instance.returnToGameplay();
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 10)
        {


        }
    }
}
