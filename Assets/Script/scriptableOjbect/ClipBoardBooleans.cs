using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   [ CreateAssetMenu(fileName = "ClipBoardBooleans", menuName = "ScriptableObjects/ClipBoardBooleans", order = 1)]
public class ClipBoardBooleans : ScriptableObject
{
    public clipboardUIManager manager;
    public firstBoss clueCollection1;







    public void setBool(int key, string value)
    {
        foreach (var item in clueCollection1.boolHolder)
        {
          //  Debug.Log(1+item.Key + "   " + item.Value);
        }
        clueCollection1.boolHolder[value] = true;

        manager.updateUI();

        foreach (var item in clueCollection1.boolHolder)
        {
           // Debug.Log(2+item.Key + "   " + item.Value);
        }
    }
}
[System.Serializable]
public class firstBoss
{

    //location
    //
    public Dictionary<string, bool> boolHolder = new Dictionary<string, bool>()
    {
        {"Location", false },
        {"Boss", false }
    };



    public bool clue1;
    public bool clue2;
}