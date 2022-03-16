using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   [ CreateAssetMenu(fileName = "ClipBoardBooleans", menuName = "ScriptableObjects/ClipBoardBooleans", order = 1)]
public class ClipBoardBooleans : ScriptableObject
{
    public firstBoss clueCollection;

}
[System.Serializable]
public class firstBoss
{
    public bool clue1;
    public bool clue2;
}