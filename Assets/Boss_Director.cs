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


    public ChasePlayer Chase;
    // Start is called before the first frame update
    void Start()
    {

        Init();
        setNewState(Chase);
    }
    public void Init()
    {
        Chase.SetBrain(this);
    }


    [System.Serializable]
    public class ChasePlayer : IState
    {
        Boss_Director _brain;


        //misc testing
        private int AttackMovesDone = 0;
        private Queue<int> attackQueue = new Queue<int>();

        public void SetBrain(Boss_Director brain)
        {
            _brain = brain;
        }

        public void enterState(StateManager manager)
        {
            //  generateAttackQueue();
            // chooseNextAttack();
            AttackMovesDone = 0;


            // Equivalent to StartCoroutine(SomeCoroutine())
            var t = new Task(SomeCoroutine());

            t.Finished += delegate (bool manual)
            {
                if (manual)
                    Debug.Log("t was stopped manually.");
                else
                    Debug.Log("t completed execution normally.");
            };

        }
        public void exitState(StateManager manager)
        {
            //var x = MoveOutOfCorner();
            //CoroutineHelper.RunCoroutine(x);


            //testing if task hast complete
      
            }
        public void updateState(StateManager manager)
        {
            //leave
        }
        public void FixedUpdateState(StateManager manager)
        {
        }

        private int uniqueAttacks = 3;
        private void generateAttackQueue()
        {

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
        public void chooseNextAttack()
        {
            var nextAttack = -1;
            if (attackQueue.Count > 0)
                nextAttack = attackQueue.Dequeue();

            switch (nextAttack)
            {

                case 0:     //chase
                    break;

                case 1:     //errectEyes
                    Debug.Log("eyes");
                    break;

                case 2:     //slash
                    Debug.Log("slash");
                    break;

                default:
                    //out of attacks exit
                    Debug.Log("Exit");
                    break;
            }

        }


        private IEnumerator SomeCoroutine()
        {
            yield return new WaitForSeconds(1);
        }
    }
}
