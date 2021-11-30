using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/stats/FootPlaceData")]
public class MovementDataCollection : ScriptableObject
{
    [SerializeField] public SpeedParamaters walkParamaters;
    [SerializeField] public SpeedParamaters runParamaters;
    [SerializeField] public SpeedParamaters AirParameters;
    [SerializeField] public SpeedParamaters GrapplingParameters;

    public float currentSpeed =1;
    public float currentMax =1;

    [SerializeField] public FootAnimationData WalkAnimation;
    [SerializeField] public FootAnimationData RunAnimation;

    [Header("Scale")]
    public float currentScale = 1;

    [Header("Misc")]
    public LayerMask groundLayer;
    [SerializeField] public bool debugRays = false;


    public void setCurrentSpeed(float current, float Max)
    {
        currentSpeed = current;
        currentMax = Max;
    }

    //getters...

    public float minMoveDist { get { return RelativeToSpeed(WalkAnimation.m_minMoveDist, RunAnimation.m_minMoveDist); } }
    public float maxMoveDist { get { return RelativeToSpeed(WalkAnimation.m_maxMoveDist, RunAnimation.m_maxMoveDist); } }
    public float SurveyorRadius { get { return RelativeToSpeed(WalkAnimation.m_SurveyorRadius, RunAnimation.m_SurveyorRadius); } }
    public float moveDuration { get { return RelativeToSpeed(WalkAnimation.m_moveDuration, RunAnimation.m_moveDuration); } }
    public float overshootMultiplier { get { return RelativeToSpeed(WalkAnimation.m_overshootMultiplier, RunAnimation.m_overshootMultiplier) ; } }
    public float HightOffset { get { return WalkAnimation.m_HightOffset; } }

    public float heightCurve (float normalizedTime)
    {
        var walkHeight = WalkAnimation.heightCurve.Evaluate(normalizedTime);
        var runHeight = RunAnimation.heightCurve.Evaluate(normalizedTime);

        return RelativeToSpeed(walkHeight, runHeight);
    }
    public float SideCurve(float normalizedTime)
    {
        var walkHeight = WalkAnimation.SideCurve.Evaluate(normalizedTime);
        var runHeight = RunAnimation.SideCurve.Evaluate(normalizedTime);

        return RelativeToSpeed(walkHeight, runHeight);
    }

    public float moveCurve(float normalizedTime)
    {
        var walkMoveCurve = WalkAnimation.moveCurve.Evaluate(normalizedTime);
        var runMoveCurve = RunAnimation.moveCurve.Evaluate(normalizedTime);

        return RelativeToSpeed(walkMoveCurve, runMoveCurve);
    }

    public float StickGroundCurve(float normalizedTime) // 0 = to ground  
    {
        var walkMoveCurve = WalkAnimation.StickToGroundStrenght.Evaluate(normalizedTime);
        var runMoveCurve = RunAnimation.StickToGroundStrenght.Evaluate(normalizedTime);

        return RelativeToSpeed(walkMoveCurve, runMoveCurve);
    }

    private float RelativeToSpeed(float min, float max)
    {
        var relativeFloat = Mathf.Lerp(min, max, ProsentageToTotalMaxSpeed());

        return relativeFloat;
    }
    public float ProsentageToTotalMaxSpeed()
    {
        var prosentage = currentSpeed / runParamaters.maxSpeed;
        prosentage = Mathf.Clamp(prosentage, 0, 1);
        //if (currentSpeed < 4 && manualSet)
        //  prosentage = 0;
        return prosentage;
    }
    public float ProsentageToCurrentMaxSpeed()
    {
        var prosentage = currentSpeed / currentMax;

        prosentage = Mathf.Clamp(prosentage, 0, 1);
        //if (currentSpeed < 4 && manualSet)
        //  prosentage = 0;
        return prosentage;
    }

}
[System.Serializable]
public class SpeedParamaters
{
    public float maxSpeed = 8;
    public float Acceleration = 200;
    [Range(1, 2)]
    public float movementDampen =1f;
    public AnimationCurve AccelerationFactorFromDot;
    public float MaxAccelForce = 150;
    public AnimationCurve MaxAccelerationForceFactorFromDot;

    public Vector3 forceScale = new Vector3(1, 0, 1);

  //  [Range (0.01f, 1)]
  //  public float ExternalDampen = 0.1f;    //time it takes to go back to "ghoal velocity"

}

[System.Serializable]
public class FootAnimationData
{
    [Range(0.3f, 2f)]
    [SerializeField] public float m_SurveyorRadius = 0.5f;
    [SerializeField] public float m_minMoveDist = 0.3f;
    [SerializeField] public float m_maxMoveDist = 1.4f;
    [SerializeField] public float m_moveDuration = 0.12f;

    [Header("move")]
    public AnimationCurve moveCurve;
    [SerializeField] public float m_overshootMultiplier = 0.3f;

    [Header("Offset")]
    public AnimationCurve heightCurve;
    public AnimationCurve SideCurve;
    public AnimationCurve StickToGroundStrenght;
    [SerializeField] public float m_HightOffset = 0.05f;    //to prevent foot clipping through ground
}