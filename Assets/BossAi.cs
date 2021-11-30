using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BossAi : MonoBehaviour
{
    public Block block;
    public bulletSpawner spawner;
    // Start is called before the first frame update
    void Start()
    {
        leftHand. StartOffset = leftHand.handTarget.position - transform.position;
        rightHand. StartOffset = rightHand.handTarget.position - transform.position;

        HealthUI.setMaxFill(block.maxHealth);
        StartCoroutine(moveBoss(point2.position));
    }
    [System.Serializable]
    public class hand
    {
        public Vector3 StartOffset;
        public Transform handTarget;
        public damageSphere indicator;

    }
    [Header("hand")]
    public Transform player;

    public hand leftHand;
    public hand rightHand;

    public float NextAttack_left;
    public float NextAttack_right;
    public float attackRate = 7;

    private bool moving;
    private bool attaking;
    private float moveDuration = 3;
    public Transform point1;
    public Transform point2;
    public Transform point3;
    private int shoot =0;

    IEnumerator moveBoss(Vector3 endPoint)
    {
        if (shoot >= 2)
        {
            StartCoroutine(spawner.StartBulletHell( 0.2f));
            shoot = 0;
        }
        else
            shoot++;
        moving = true;
        multiplyer = 2;
        var timeElapsed = 0f;
        var startPoint = transform.position;
        do
        {
            timeElapsed += Time.deltaTime;
            var prosentage = timeElapsed / moveDuration;

            //move foot here
            var animationPoint = Vector3.Lerp(startPoint, endPoint, prosentage);
            transform.position = animationPoint;
            yield return null;
        }
        while (timeElapsed <= moveDuration);
        //reached end location

        var randomPoint = Random.Range(1, 4);
        var nextEndpoint = transform;
        switch (randomPoint)
        {
            case 1:
                nextEndpoint = point1;
                break;
            case 2:
                nextEndpoint = point2;
                break;
            case 3:
                nextEndpoint = point3;
                break;
            default:
                break;
        }
        moving = false;
        multiplyer = 1;
        yield return new WaitForSeconds(Random.Range(2,4));
        while (attaking)
        {
            yield return null;
        }
        StartCoroutine(moveBoss(nextEndpoint.position));
    }

    void Update()
    {
        if (!moving)
        {
            if (NextAttack_left > 0)
            {
                NextAttack_left -= Time.deltaTime;
            }
            else
            {
                NextAttack_left = attackRate + Random.Range(-3, 3);
                StartCoroutine(handAttack(leftHand));
            }
            if (NextAttack_right > 0)
            {
                NextAttack_right -= Time.deltaTime;
            }
            else
            {
                NextAttack_right = attackRate + Random.Range(-2, 2);
                StartCoroutine(handAttack(rightHand));
            }
        }
        HeadLook();

    }
    public Transform hoverParent;
    public float hoverHeight;
    public float hoverSpeed = 1;
    public float smoothTime = 0.3F;

    private float multiplyer= 1;
    private Vector3 velocity = Vector3.zero;
    public void FixedUpdate()
    {
        var pingPongPosition = new Vector3(0, Mathf.PingPong(Time.time * hoverSpeed * multiplyer, hoverHeight / multiplyer) - hoverHeight / 2f / multiplyer, 0);
        hoverParent.localPosition = Vector3.SmoothDamp(hoverParent.localPosition, pingPongPosition, ref velocity, smoothTime); ;
    }

    public Transform headLookTarget;
    public void HeadLook()
    {
        headLookTarget.transform.position = player.transform.position;
    }
    private IEnumerator handAttack(hand Hand)
    {
        Hand. indicator.gameObject.SetActive(true);
        Hand.indicator.transform.localScale = Vector3.one;
        attaking = true;
        var playerPosition = player.transform.position;

        Hand.indicator.transform.position = playerPosition;

        //show indicator
        LeanTween.scale(Hand.indicator.gameObject, Vector3.one * 12, 0.5f)
            .setEaseOutElastic();
        //move over player
        LeanTween.move(Hand.handTarget.gameObject, playerPosition + Vector3.up * 6, 0.6f)
            .setEaseInOutBack();
        LeanTween.scale(Hand.handTarget.gameObject, Vector3.one * 1.4f, 0.5f)
            .setEaseInOutBack();
        yield return new WaitForSeconds(0.8f);


            //slam down
        LeanTween.move(Hand.handTarget.gameObject, playerPosition, 0.1f)
         .setEaseInOutBack()
         .setEaseOutBounce();

        yield return new WaitForSeconds(0.1f);
        Hand.indicator.active = true;

        LeanTween.scale(Hand.handTarget.gameObject, Vector3.one * 2, 0.3f)
            .setEasePunch();
        LeanTween.scale(Hand.indicator.gameObject, Vector3.one, 0.1f)
       .setEaseOutElastic();

        yield return new WaitForSeconds(0.4f);
        Hand.indicator.active = false;
     
        LeanTween.scale(Hand.handTarget.gameObject, Vector3.one, 1.5f)
   .setEaseOutElastic();
        LeanTween.move(Hand.handTarget.gameObject, transform.position+ Hand.StartOffset, 1)
     .setEaseInOutBack();
        attaking = false;
        Hand.indicator.gameObject.SetActive(false);
    }

    public updateUI HealthUI;
    public void setFill()
    {
        HealthUI.setFill(block.currentHealth);
    }  
}