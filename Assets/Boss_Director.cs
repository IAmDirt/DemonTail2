using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using UnityEngine;

public class Boss_Director : StateManager
{

    public class Action
    {
        //eiter have duration
        //or set coroutine on complete call next attack queue

        public float duration;
        public IEnumerator AttackCoroutine;
    }


    public Stage1 stage1;
    public Stage2 stage2;
    public Stage3 stage3;
    [Header("Refrences")]
    public CinemachineImpulseSource impulse;
    public Transform ArenaCenter;
    public Rigidbody rb;

    void Start()
    {
        Init();
        setNewState(stage1);
        rb = GetComponent<Rigidbody>();
        RotateTarget = player;
        HealthUI.setMaxFill(block.maxHealth);
    }
    public void Init()
    {
        stage1.SetBrain(this);
        stage2.SetBrain(this);
        stage3.SetBrain(this);
    }

    [Header("UI")]
    public updateUI HealthUI;
    public Block block;

    public void setFill()
    {
        HealthUI.setFill(block.currentHealth);
    }


    public override void Update()
    {
        if(RotateToPLayer)
        Rotate();
        base.Update();

    }
    public bool RotateToPLayer = true;
    public float RotateSpeed = 2;
    public Transform RotateTarget;
    public void Rotate()
    {
        var direction = RotateTarget.position - transform.position;
        direction.y = 0;
        float singleStep = RotateSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction.normalized, singleStep, 0.0f);

        transform.rotation = Quaternion.LookRotation(newDirection);
    }
    [Header("Jump")]
    public AnimationCurve jumpCurve;
    public float jumpDuration = 1;
    public float jumpHeight;
    IEnumerator jumpArc(Vector3 endPoint)
    {
        var startPoint = rb.transform.position;
        var timeElapsed = 0f;
        do
        {
            timeElapsed += Time.deltaTime;

            var prosentage = timeElapsed / jumpDuration;
            var jumpArc = Vector3.up * jumpHeight * jumpCurve.Evaluate(prosentage);   //calculate height of jumpArc

            //move foot here
            var animationPoint = Vector3.Lerp(startPoint, endPoint, prosentage) + jumpArc;
            rb.MovePosition(animationPoint);
            yield return null;
        }
        while (timeElapsed <= jumpDuration);
        //reached end location
       // particleJump.Play();
       // JumpImpact.PlayRandomClip();
        impulse.GenerateImpulse(5);
    }


    [System.Serializable]
    public class Stage1 : IState
    {
        Boss_Director _brain;

        //misc testing
        public Task CurrentTask;
        private int uniqueAttacks = 4;
        private int AttackMovesDone = 0;
        private int[] Shuffle(int[] a)
        {
            // Loops through array
            for (int i = a.Length - 1; i > 0; i--)
            {
                // Randomize a number between 0 and i (so that the range decreases each time)
                int rnd = Random.Range(0, i);

                // Save the value of the current i, otherwise it'll overright when we swap the values
                int temp = a[i];

                // Swap the new and old values
                a[i] = a[rnd];
                a[rnd] = temp;
            }
            return a;
        }
        private Queue<int> attackQueue = new Queue<int>();

        public void SetBrain(Boss_Director brain)
        {
            _brain = brain;
        }

        public void enterState(StateManager manager)
        {
            generateAttackQueue();
            chooseNextAttack();

        }
        public void exitState(StateManager manager)
        {

        }
        public void updateState(StateManager manager)
        {
        }
        public void FixedUpdateState(StateManager manager)
        {
        }

        private void generateAttackQueue()
        {
            AttackMovesDone = 0;

            var attackList = new int[uniqueAttacks];

            for (int i = 0; i < attackList.Length; i++)
            {
                attackList[i] = i;
            }

            attackList = Shuffle(attackList);   //shuffle and enqueue attack sequence
            foreach (var i in attackList)
            {
                attackQueue.Enqueue(i);
            }
        }
        public void chooseNextAttack()
        {
            AttackMovesDone++;
            var nextAttack = -1;
            if (attackQueue.Count > 0)
                nextAttack = attackQueue.Dequeue();
            else
            {
                Debug.Log("AttackQueue Depleted");
                generateAttackQueue();
                chooseNextAttack();
                return;
            }

            IEnumerator nextTask;


            switch (nextAttack)
            {
                case 0:     //chase
                    nextTask = ClusterShot();
                    break;

                case 1:     //errectEyes
                    nextTask = AcrobaticSwing();
                    break;

                case 2:     //slash
                    nextTask = StickSpin();
                    break;
                case 3:     //slash
                    nextTask = HomingMissiles();
                    break;
                    
                default:
                    //out of attacks exit
                    nextTask = SomeCoroutine();
                    break;
            }

            CurrentTask = new Task(nextTask);

            CurrentTask.Finished += delegate (bool manual)
            {
                if (manual)
                    Debug.Log("CurrentTask was stopped manually.");
                else
                {
                    //when current task Is completed
                    Debug.Log("CurrentTask completed execution normally.");
                    chooseNextAttack();
                }
            };

        }

        private IEnumerator SomeCoroutine()
        {
            Debug.Log("SomeCoroutine");
            yield return new WaitForSeconds(1);
        }
        private IEnumerator AcrobaticSwing()
        {
            Debug.Log("AcrobaticSwing");
            //jump out of arena
            yield return new WaitForSeconds(1);
         //prediction where to swing
            yield return new WaitForSeconds(1);
            //swimg all predicted paths
            //leav fire trail that damages
            yield return new WaitForSeconds(1);
            //land at position 
            yield return new WaitForSeconds(1);
        //end
        }
        [Header("StickSpin")]
        public Transform Stick;
        public Transform StickEnd;
        public damageSphere Collider;

        public float rotateDuration;
        private Vector3 startSize = new Vector3(1.5f, 0.1f, 1.5f);
        private Vector3 endSize = new Vector3(1, 1, 1);
        private IEnumerator StickSpin()
        {
         CoroutineHelper.RunCoroutine(    _brain.jumpArc(_brain.ArenaCenter.position));
            Debug.Log("StickSpin");
            yield return new WaitForSeconds(_brain.jumpDuration +0.2f);

            Stick.localEulerAngles = new Vector3(0, 0, 0);
            //Stick summon anim
            Stick.localScale = startSize;
            Stick.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            LeanTween.scale(Stick.gameObject, endSize, 0.6f).setEaseInOutBack();

            yield return new WaitForSeconds(1);
           // _brain.RotateToPLayer = false;
            _brain.RotateTarget = StickEnd;
            //stick fall
            LeanTween.rotateLocal(Stick.gameObject, new Vector3(90, 0,0), 1).setEaseOutBounce();

            yield return new WaitForSeconds(_brain.jumpDuration +0.2f);
            //stick rotate
            Collider.active = true;
            float startRotation = Stick.eulerAngles.y;
            float endRotation = startRotation + 360.0f *2;
            float t = 0.0f;
            while (t < rotateDuration)
            {
                t += Time.deltaTime;
                float yRotation = Mathf.Lerp(startRotation, endRotation, t / rotateDuration) % 360.0f;
                Stick.eulerAngles = new Vector3(Stick.eulerAngles.x, yRotation, Stick.eulerAngles.z);
                yield return null;
            }
            LeanTween.rotateLocal(Stick.gameObject, new Vector3(0,0,0), 0.7f);
            yield return new WaitForSeconds(0.5f);
            LeanTween.scale(Stick.gameObject, startSize, 0.5f).setEaseInOutBack();
            Collider.active = false;
            yield return new WaitForSeconds(0.4f);
            //stick Disapear
            Stick.gameObject.SetActive(false);
            //_brain.RotateToPLayer = true;
            _brain.RotateTarget = _brain.player;
            yield return new WaitForSeconds(1);
        }
        private IEnumerator KnockAway()
        {
            Debug.Log("KnockAway");
            //animation build up
            yield return new WaitForSeconds(1);
            //knock player away
            yield return new WaitForSeconds(1);
        }
        [Header("clusterSHot")]
        public bulletSpawner clusterSpawner;
        private IEnumerator ClusterShot()
        {
            var amountOfShots = 2;
            Debug.Log("ClusterShot");
            while (amountOfShots > 0)
            {
                amountOfShots--;
            clusterSpawner. FirePatternCircle();
                // spawn homing
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(1);
        }

        [Header("HomingMIssiles")]
        public bulletSpawner HomingSpawner;
        private IEnumerator HomingMissiles()
        {
            var amountOfShots = 3;
            Debug.Log("HomingMissiles");

            while (amountOfShots > 0)
            {
                amountOfShots--;
                HomingSpawner.SpawnHoming(false);
                // spawn homing
                yield return new WaitForSeconds(0.4f);
            }
            //
            yield return new WaitForSeconds(1);
        }
    }

    [System.Serializable]
    public class Stage2 : IState
    {
        Boss_Director _brain;


        public void SetBrain(Boss_Director brain)
        {
            _brain = brain;
        }

        public void enterState(StateManager manager)
        {


        }
        public void exitState(StateManager manager)
        {

        }
        public void updateState(StateManager manager)
        {
        }
        public void FixedUpdateState(StateManager manager)
        {
        }


    }

    [System.Serializable]
    public class Stage3 : IState
    {
        Boss_Director _brain;


        public void SetBrain(Boss_Director brain)
        {
            _brain = brain;
        }

        public void enterState(StateManager manager)
        {


        }
        public void exitState(StateManager manager)
        {

        }
        public void updateState(StateManager manager)
        {
        }
        public void FixedUpdateState(StateManager manager)
        {
        }


    }

}
