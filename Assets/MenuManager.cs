using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public EventSystem MenuEventSystem;


    public MenuButton currentButton;

    void Start()
    {

    }

    void Update()
    {
        if (currentButton)
        {

            if (currentButton.gameObject != MenuEventSystem.currentSelectedGameObject)
            {
                //select new button
                currentButton.deselect();

                currentButton = MenuEventSystem.currentSelectedGameObject.GetComponent<MenuButton>();
                currentButton.selelected();
            }
        }
        else
            currentButton = MenuEventSystem.currentSelectedGameObject.GetComponent<MenuButton>();
    }
}
