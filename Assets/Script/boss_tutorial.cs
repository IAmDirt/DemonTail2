using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_tutorial : StateManager
{
    public Stage1 stage1;

    public void Start()
    {
        Init();
        setNewState(stage1);
        HealthUI.setMaxFill(block.maxHealth);
    }
    public void Init()
    {
        stage1.SetBrain(this);
    }

    public override void Update()
    {
        base.Update();
        Rotate();
    }

    [Header("UI")]
    public updateUI HealthUI;
    public Block block;

    public void setFill()
    {
        HealthUI.setFill(block.currentHealth);
    }
    public void checkDamage()
    {
        stage1.damageTaken++;
    }


    public bool RotateToPLayer = true;
    public float RotateSpeed = 2;
    public Rigidbody rb;
    public void Rotate()
    {
        var direction = player.position - transform.position;
        direction.y = 0;
        float singleStep = RotateSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, direction.normalized, singleStep, 0.0f);

        rb.rotation = Quaternion.LookRotation(newDirection);
    }
    [System.Serializable]
    public class Stage1 : IState
    {
        boss_tutorial _brain;

        public GameObject ballDeflect;
        public float damageTaken;
        private int stateHealth = 8; 
        public void SetBrain(boss_tutorial brain)
        {
            _brain = brain;
        }

        public void enterState(StateManager manager)
        {
            _brain.block.maxHealth= stateHealth;
            _brain.block.Init();
            _brain.HealthUI.setMaxFill(stateHealth);

            ballDeflect.SetActive(true);

        }
        public void exitState(StateManager manager)
        {
            ballDeflect.SetActive(false);
        }
        public void updateState(StateManager manager)
        {
            if(damageTaken>= stateHealth)
            {
                //next
                Debug.Log("next");
            }
        }
        public void FixedUpdateState(StateManager manager)
        {
        }
    
    
    }
}