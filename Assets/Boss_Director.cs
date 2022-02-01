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
    void Start()
    {
        Init();
        setNewState(stage1);
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
            yield return new WaitForSeconds(1);
        }
        private IEnumerator StickSpin()
        {
            Debug.Log("StickSpin");
            yield return new WaitForSeconds(1);
        }
        private IEnumerator KnockAway()
        {
            Debug.Log("KnockAway");
            yield return new WaitForSeconds(1);
        }
        private IEnumerator ClusterShot()
        {
            Debug.Log("ClusterShot");
            yield return new WaitForSeconds(1);
        }
        private IEnumerator HomingMissiles()
        {
            Debug.Log("HomingMissiles");
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
