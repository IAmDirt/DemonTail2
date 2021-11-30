using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialRoom
    {
        public GameObject TutorialDoor;
        public Transform RoomCenter; //cameraTarget;
    }

    public TutorialRoom[] tutorialRooms;
    public int CurrentRoomNr = 1;
    void Update()
    {
        switch (CurrentRoomNr)
        {
            case 1:
                RoomOne();
                break;

            case 2:
                roomTwo();
                break;

            case 3:
                roomThree();
                break;
            case 4:
                roomFour();
                break;

            case 5:
                roomFive();
                break;
            default:
                //not added this room
                break;
        }
    }
    //WASD tutorial
    private int directionsMoved;
    public void RoomOne()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            directionsMoved++;
        }

        if (directionsMoved >= 3)
        {
            //next room
            unlockNextRoom();
        }
    }

    private float aimTargetsComplete = 0;
    public void aimTargetReached()
    {
        //target grows in size when aimed at
        aimTargetsComplete++;
    }

    Ray ray;
    RaycastHit hit;
    public void roomTwo()
    {
        //aim at 2-3 targets 

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            var TutorialTarget =hit.collider.GetComponent<tutorailTarget>();

            if(TutorialTarget)
            {
                TutorialTarget.hover(this);
            }
        }

        if (aimTargetsComplete >= 3)
        {
            unlockNextRoom();
        }
    }

    public void roomThree()
    {
        //shoot ball to hit target
    }
    public void roomFour()
    {
        //dash Throught bullet wave
        if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space))
        {
            Invoke( "unlockNextRoom", 1);
        }

    }
    public void roomFive()
    {
        //reflect black ball
    }
    private bool reflectUnlocked;
    public void ReflectUnlock()
    {
        if(!reflectUnlocked)
        {
            unlockNextRoom();
            reflectUnlocked = true;
        }
    }
    public void unlockNextRoom()
    {
        //open next door animation

        CurrentRoomNr++;

        tutorialRooms[CurrentRoomNr - 1].TutorialDoor.SetActive(false);
        StartCoroutine(moveCamera(tutorialRooms[CurrentRoomNr - 1].RoomCenter.position));
    }

    public float CameraSmooth= 1.45f;
    public Transform CameraTarget;
    private IEnumerator moveCamera(Vector3 targetPosition)
    {
        Vector3 startPosition = CameraTarget.position;
        float lerp = 0;
        float smoothLerp = 0;

        while (lerp < 1 && CameraSmooth > 0)
        {
            lerp = Mathf.MoveTowards(lerp, 1, Time.deltaTime / CameraSmooth);
            smoothLerp = Mathf.SmoothStep(0, 1, lerp);
            CameraTarget.position = Vector3.Lerp(startPosition, targetPosition, smoothLerp);
            yield return null;
        }
        CameraTarget.position = targetPosition;
    }

    public void checkInput()
    {

    }
}