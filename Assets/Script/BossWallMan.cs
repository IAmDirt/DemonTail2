using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using UnityEngine;
public class BossWallMan : StateManager
{
    public bool dead;
    public Hide hideState = new Hide();
    public ChasePlayer chaseState = new ChasePlayer();
    public EnragedChase EnragedState = new EnragedChase();

    public Animator anim;

    readonly int m_ChaseAttack = Animator.StringToHash("ChaseAttack");
    readonly int m_ShootProjectile = Animator.StringToHash("Shoot");
    readonly int m_Scream = Animator.StringToHash("Scream");
    readonly int m_Jump = Animator.StringToHash("Jump");
    readonly int m_Dead = Animator.StringToHash("Dead");
    readonly int m_Hit = Animator.StringToHash("Hit");
    readonly int m_Tired = Animator.StringToHash("Tired");


    public ParticleSystem ballSuck;
    public ParticleSystem screamParticle;
    [Header("audio")]
    public RandomAudioPlayer JumpImpact;
    public RandomAudioPlayer screamSound;

    #region basic Unity Functions
    public void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        Init();
    }
    public void Init()
    {
        hideState.SetBrain(this);
        chaseState.SetBrain(this);
        EnragedState.SetBrain(this);
    }
    public void Start()
    {
        HealthUI.setMaxFill(block.maxHealth);
        setNewState(hideState);

        rightHand_IK.StartOffset = rightHand_IK.target.localPosition;
        leftHand_IK.StartOffset = leftHand_IK.target.localPosition;
        Root_IK.StartOffset = Root_IK.target.localPosition;
    }
    public override void Update()
    {
        base.Update();


        HeadLook();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        WiggleAnim(Root_IK);
        WiggleAnim(rightHand_IK);
        WiggleAnim(leftHand_IK);
    }
    #endregion
    public void DeathEvent()
    {
        dead = true;
        anim.SetTrigger(m_Dead);
        anim.applyRootMotion = true;
        StopCoroutine(EnragedState.CurrentAction);
    }
    public void DamageTakenEvent()
    {
        hideState.hideDamage();
        anim.SetTrigger(m_Hit);
    }
    public void playParticle()
    {
        currentCorner.destroyParticle.Play();
    }

    #region general boss functions

    [Header("Important locations")]
    public Transform headLookTarget;
    public Transform Center;

    [Header("Movement / rotation")]
    public float moveDuration = 1;
    public float rotationSpeed = 20;
    public float jumpHeight;
    public AnimationCurve jumpCurve;
    private Rigidbody rb;
    public ParticleSystem particleJump;

    [Header("projectiles")]
    public GameObject bigProjectile;
    public Transform spawnTrans;
    public float shootAngle = 25;
    public float predictMultiplyer = 1;

    [Header("UI")]
    public updateUI HealthUI;
    public Block block;

    public void setFill()
    {
        HealthUI.setFill(block.currentHealth);
    }
    public void rotateBodySmooth(Vector3 direction, float smooth = 20)
    {
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * smooth));
    }
    IEnumerator MoveToPosition(Vector3 endPoint)
    {
        var startPoint = rb.transform.position;
        var timeElapsed = 0f;
        do
        {
            timeElapsed += Time.deltaTime;
            var prosentage = timeElapsed / moveDuration;

            //move foot here
            var animationPoint = Vector3.Lerp(startPoint, endPoint, prosentage);
            rb.MovePosition(animationPoint);
            yield return null;
        }
        while (timeElapsed <= moveDuration);
        //reached end location
    }
    IEnumerator moverArc(Vector3 endPoint)
    {
        anim.SetTrigger(m_Jump);
        BodyRig.weight = 0.3f;
        var startPoint = rb.transform.position;
        var timeElapsed = 0f;
        do
        {
            timeElapsed += Time.deltaTime;

            var prosentage = timeElapsed / moveDuration;
            var jumpArc = Vector3.up * jumpHeight * jumpCurve.Evaluate(prosentage);   //calculate height of jumpArc

            //move foot here
            var animationPoint = Vector3.Lerp(startPoint, endPoint, prosentage) + jumpArc;
            rb.MovePosition(animationPoint);
            yield return null;
        }
        while (timeElapsed <= moveDuration);
        anim.transform.localPosition = Vector3.zero;
        anim.transform.localRotation = Quaternion.identity;
        //reached end location
        particleJump.Play();
        JumpImpact.PlayRandomClip();
        impulse.GenerateImpulse(5);
        BodyRig.weight = 1;
    }
    public CinemachineImpulseSource impulse;
    public Vector3 directionPlayerFLAT()
    {
        var direction = player.position - transform.position;
        direction.y = 0;
        return direction = direction.normalized;
    }
    public void arcProjectile(bool isDud, Vector3 playerPosition)
    {
        var randomCircle = Random.insideUnitCircle * 20;
        var target = playerPosition + new Vector3(randomCircle.x, 0, randomCircle.y);

        var velocityPredicition = player.GetComponent<Rigidbody>().velocity;
        velocityPredicition *= Random.Range(predictMultiplyer - 0.1f, predictMultiplyer + 0.2f);


        var spawned = PoolManager.Spawn(bigProjectile.gameObject, spawnTrans.position, spawnTrans.rotation);
        var ball = spawned.GetComponent<ArcProjectile>();
        ball.isDud = isDud;
        ball.PhysicsShoot(target, Random.Range(shootAngle - 5, shootAngle + 5), velocityPredicition);
    }

    #endregion

    #region IK Animations
    [System.Serializable]
    public class IkTarget
    {
        //public ParticleSystem smashParticle;
        public bool isRight;
        public bool isIdle = true;
        public Transform target;
        [HideInInspector] public Vector3 StartOffset;

        [Header("Wiggle")]

        public Vector3 WiggleMagnitude = Vector3.one;
        public Vector3 magnitudeOffset = Vector3.zero;
        public float scaleModifyer = 1;
        public float nextChangeTimer = 0.2f;
        public float wiggleSpeed = 10;

        [HideInInspector] public Vector3 wiggleOffset;
        [HideInInspector] public Vector3 vel;
        [HideInInspector] public float directionChangeTimer;
    }
    public IkTarget leftHand_IK;
    public IkTarget rightHand_IK;
    public IkTarget Root_IK;

    public Rig BodyRig;
    public void HeadLook()
    {
        headLookTarget.transform.position = player.transform.position + Vector3.up * 5;
    }

    public void WiggleAnim(IkTarget IKData)
    {
        if (!IKData.isIdle)
            return;

        if (IKData.directionChangeTimer <= 0)
        {
            changeDirectionWiggle(IKData);
        }
        else
            IKData.directionChangeTimer -= Time.deltaTime;

        var targetPosition = IKData.StartOffset + transform.InverseTransformPoint(transform.position + IKData.wiggleOffset);

        IKData.target.localPosition = Vector3.SmoothDamp(IKData.target.localPosition, targetPosition, ref IKData.vel, Time.deltaTime * IKData.wiggleSpeed);
    }

    public void changeDirectionWiggle(IkTarget IKData)
    {
        var offsetMagnitude = IKData.WiggleMagnitude;
        var xPos = Random.Range(-offsetMagnitude.x, offsetMagnitude.x) + IKData.magnitudeOffset.x;
        var yPos = Random.Range(-offsetMagnitude.y, offsetMagnitude.y) + IKData.magnitudeOffset.y;
        var zPos = Random.Range(-offsetMagnitude.z, offsetMagnitude.z) + IKData.magnitudeOffset.z;

        IKData.wiggleOffset = new Vector3(xPos, yPos, zPos);
        IKData.wiggleOffset *= IKData.scaleModifyer;

        IKData.directionChangeTimer = IKData.nextChangeTimer * Random.Range(0.6f, 1.4f);
    }

    #endregion

    [System.Serializable]
    public class Corner
    {
        public Transform BossStandPosition;
        public Transform[] WallPositions;
        public Transform wallBlock;
        public Transform DestroyedBlock;
        public ParticleSystem destroyParticle;
        public bool HasBeenUsed = false;
    }
    private Corner currentCorner;

    #region states
    [System.Serializable]
    public class Hide : IState
    {
        public Corner[] corners;
        public GameObject wallPrefab;

        [Header("diceProjectiles")]
        public float ProjectileDuration = 5.5f;
        public float CardThrowDuration = 2.5f;
        public float BallSuckDuration = 2.5f;
        public float AttackDuration = 0;

        private bool SpawnersSummoned;
        private float damageTakenThisState;
        private bool waiting;
        private bool RemoveBalls;

        private int DifficultyScaling = 0;
        private bool rage = false;
        BossWallMan _brain;

        #region corners
        private Corner GetNextCorner()
        {
            Corner nextCorner = null;
            for (int i = 0; i < corners.Length; i++)
            {
                if (!corners[i].HasBeenUsed)
                {
                    nextCorner = corners[i];
                    break;
                }
            }
            return nextCorner;
        }
        public void moveToRandomCorner()
        {
            var nextCorner = GetNextCorner();
            if (nextCorner == null)
            {
                _brain.setNewState(_brain.EnragedState);
                return;
            }

            nextCorner.HasBeenUsed = true;
            _brain.currentCorner = nextCorner;


            CoroutineHelper.RunCoroutine(_brain.moverArc(nextCorner.BossStandPosition.position));
            CoroutineHelper.RunCoroutine(destroyCorner());
        }
        private IEnumerator destroyCorner()
        {
            yield return new WaitForSeconds(0.8f);
            var nextCorner = _brain.currentCorner;
            nextCorner.wallBlock.gameObject.SetActive(false);
            nextCorner.DestroyedBlock.gameObject.SetActive(true);

            spawnWalls(nextCorner);
            _brain.playParticle();
            waiting = false;
        }
        public void spawnWalls(Corner currentCorner)
        {
            foreach (var wallTrans in currentCorner.WallPositions)
            {
                var spawned = Instantiate(wallPrefab, _brain.transform.position, wallTrans.rotation, wallTrans);

                var spawnAnim = spawned.GetComponent<BlockSpawnAnimation>();
                if (spawnAnim) spawnAnim.spawnAnimation(wallTrans.position, wallTrans.rotation);
            }
        }
        public bool AllWallsDestroyed(Corner currentCorner)
        {
            var wallsLeft = 0;
            foreach (var wallTrans in currentCorner.WallPositions)
            {
                if (wallTrans.childCount > 0)
                    wallsLeft++;
            }
            return wallsLeft <= 0;
        }
        private IEnumerator MoveOutOfCorner()
        {
            rage = true;
            _brain.anim.SetTrigger(_brain.m_Scream);
            yield return new WaitForSeconds(1);
            _brain.screamParticle.Play();
            _brain.impulse.GenerateImpulse(5);
            _brain.screamSound.PlayRandomClip();
            yield return new WaitForSeconds(1);

            _brain.anim.SetTrigger(_brain.m_Scream);

            yield return new WaitForSeconds(0.2f);

            CoroutineHelper.RunCoroutine(_brain.EnragedState.EnragedJumpAttack());
            yield return new WaitForSeconds(_brain.EnragedState.jumpAttackDuration);
            _brain.setNewState(_brain.hideState);
        }
        #endregion

        public void SetBrain(BossWallMan brain)
        {
            _brain = brain;
        }
        public void enterState(StateManager manager)
        {
            RemoveBalls = true;
            waiting = true;
            moveToRandomCorner();
            damageTakenThisState = 0;
            DifficultyScaling++;
            AttackDuration = 2f;
            SpawnersSummoned = true;
        }
        public void exitState(StateManager manager)
        {
            rage = false;
        }
        public void FixedUpdateState(StateManager manager)
        {
        }
        public void updateState(StateManager manager)
        {
            if (waiting)
            {
                return;
            }
            //AllWallsDestroyed(_brain.currentCorner) ||
            if (damageTakenThisState > _brain.block.maxHealth / 5)
            {
                //exit state after kneeling down
                if (!rage)
                {
                    CoroutineHelper.RunCoroutine(MoveOutOfCorner());
                    suckInBalls();
                }
            }
            else
            {
                if (AttackDuration <= 0)
                {
                    /*if (RemoveBalls)
                    {
                        suckInBalls();

                    }
                    else*/
                    if (!SpawnersSummoned)
                    {
                        SpawnersSummoned = true;
                        var Bulletspawner = _brain.GetComponent<bulletSpawner>();
                        Bulletspawner.spiralStart();
                        AttackDuration = CardThrowDuration;
                    }
                    else
                    {
                        CoroutineHelper.RunCoroutine(fireCluster());
                        SpawnersSummoned = false;
                        AttackDuration = ProjectileDuration;
                    }
                }
                else
                    AttackDuration -= Time.deltaTime;
            }
            _brain.rotateBodySmooth(_brain.directionPlayerFLAT(), _brain.rotationSpeed);
        }

        public void hideDamage()
        {
            damageTakenThisState++;
        }
        public IEnumerator fireCluster()
        {
            var amountOfShots = 7;
            var dudShot = Random.Range(0, amountOfShots);

            _brain.anim.SetTrigger(_brain.m_ShootProjectile);
            var playerPosition = _brain.player.position;

            yield return new WaitForSeconds(1f);
            while (amountOfShots > 0)
            {
                amountOfShots--;
                var isdud = amountOfShots == dudShot ? true : true;
                _brain.arcProjectile(isdud, playerPosition);
                yield return new WaitForSeconds(0.4f);
            }
        }
        public void suckInBalls()
        {
            RemoveBalls = false;
            AttackDuration = BallSuckDuration;
            _brain.ballSuck.Play();

            var balls = Physics.OverlapSphere(Vector3.zero, 100, 1 << 8);

            foreach (var ballCollider in balls)
            {
                var behavior = ballCollider.GetComponent<BallBehavior>();
                if (behavior)
                {
                    ballCollider.enabled = false;
                    var moveTime = 1.5f;
                    LeanTween.move(behavior.gameObject, _brain.transform.position, moveTime);
                    behavior.Invoke("selfDestroy", moveTime);
                }
            }
        }

        /*
        [Header("spanwers")]
        public float spawnHeight = 10;
        public GameObject spawner;
        public IEnumerator SummonSpawners(StateManager manager)
        {
            SpawnersSummoned = true;
            var attacksDone = 0;
            while (attacksDone < DifficultyScaling - 1)
            {
                yield return new WaitForSeconds(1.5f);

                var randomOffset = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                SpawnDebris(_brain.Center.position + randomOffset);
                attacksDone++;

                yield return null;
            }

            yield return new WaitForSeconds(1f);
        }

        public void SpawnDebris(Vector3 position)
        {
            _brain.anim.SetTrigger(_brain.m_ShootProjectile);
            var spawnPos = position + Vector3.up * spawnHeight;
            var spawned = PoolManager.Spawn(spawner, spawnPos, spawner.transform.rotation);

            var ConeAnimation = spawned.transform.GetChild(0).GetComponent<bulletSpawner>();
            ConeAnimation.Animate(spawnPos, position);
        }
        */
    }

    [System.Serializable]
    public class ChasePlayer : IState
    {
        BossWallMan _brain;

        private float StayTime = 7;
        private float TimeUntilExit;

        //misc testing
        private int AttackMovesDone = 0;
        private Queue<int> attackQueue = new Queue<int>();

        public void SetBrain(BossWallMan brain)
        {
            _brain = brain;
        }

        public void enterState(StateManager manager)
        {
            CoroutineHelper.RunCoroutine(_brain.moverArc(_brain.Center.position));
            //  generateAttackQueue();
            // chooseNextAttack();
            TimeUntilExit = StayTime;
            AttackMovesDone = 0;

        }
        public void exitState(StateManager manager)
        {
        }
        public void updateState(StateManager manager)
        {
            //leave
            if (TimeUntilExit >= 0)
            {
                TimeUntilExit -= Time.deltaTime;
            }
            else
                _brain.setNewState(_brain.hideState);
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
                    CoroutineHelper.RunCoroutine(Chase());
                    break;

                case 1:     //errectEyes
                    Debug.Log("eyes");
                    CoroutineHelper.RunCoroutine(Chase());
                    break;

                case 2:     //slash
                    Debug.Log("slash");
                    CoroutineHelper.RunCoroutine(Chase());
                    break;

                default:
                    //out of attacks exit
                    Debug.Log("Exit");
                    break;
            }

        }

        /*
        public GameObject PierceObeject;
        public Vector3 startScale = new Vector3(6, 6, 3);
        private IEnumerator PierceAttack()
        {
            PierceObeject.SetActive(true);
            PierceObeject.transform.localScale = startScale *0.01f;

            //antisipation
            CoroutineHelper.RunCoroutine(handAntisipation(rightHand));
            CoroutineHelper.RunCoroutine(handAntisipation(leftHand));
            yield return new WaitForSeconds(1);


            //end position
            //folowthrough


          //  yield return new WaitForSeconds();


            //retreat

            PierceObeject.SetActive(false);

        }

        private IEnumerator handAntisipation(hand Hand)
        {
            var offset = (Hand.isRight ? 3 : -3) * _brain.transform.right;
            var Antisipation = _brain.transform.position + _brain.transform.up * 28 + offset;
            var TargetPos = _brain.transform.position + _brain.transform.forward * 10 + offset;

            //move over player
            LeanTween.move(Hand.handTarget.gameObject, Antisipation, 0.15f)
                .setEaseInOutBack();
            /* LeanTween.scale(Hand.handTarget.gameObject, Vector3.one * 1.4f, 0.5f)
                 .setEaseInOutBack();
            yield return new WaitForSeconds(0.17f);
            //slam down
            LeanTween.move(Hand.handTarget.gameObject, TargetPos, 0.02f)
                .setEaseInOutBack()
                .setEaseOutBounce();

        }
            
        */
        //chase 
        private IEnumerator Chase()
        {
            yield return new WaitForSeconds(_brain.moveDuration + 0.4f);
            var timeStay = 1f;
            while (timeStay > 0)
            {
                timeStay -= Time.deltaTime;

                _brain.rotateBodySmooth(_brain.directionPlayerFLAT(), _brain.rotationSpeed);

                yield return new WaitForFixedUpdate();
            }
            _brain.anim.SetTrigger(_brain.m_ChaseAttack);

            //antisipation and rotate
            timeStay = 4;
            Task HandSlamTask = null;
            bool UseRightHand = true;
            while (timeStay > 0)
            {
                timeStay -= Time.deltaTime;



                _brain.rb.MovePosition(_brain.transform.position + _brain.transform.forward * Time.fixedDeltaTime * 15);
                _brain.rotateBodySmooth(_brain.directionPlayerFLAT(), 8f);

                if (HandSlamTask == null || !HandSlamTask.Running)  //hand attack
                {
                    var hand = _brain.leftHand_IK;
                    if (UseRightHand) hand = _brain.rightHand_IK;
                    UseRightHand = !UseRightHand;

                    // HandSlamTask = new Task(handAttack(hand));
                }
                yield return new WaitForFixedUpdate();
            }

            //chase and slam hands
            yield return new WaitForSeconds(1);

            //coldown and reset
        }
        private IEnumerator handAttack(IkTarget Hand)
        {
            var offset = (Hand.isRight ? 3 : -3) * _brain.transform.right;
            var TargetPos = _brain.transform.position + _brain.transform.forward * 3 + offset;

            //move over player
            LeanTween.move(Hand.target.gameObject, TargetPos + Vector3.up * 2, 0.15f)
                .setEaseInOutBack();
            /* LeanTween.scale(Hand.handTarget.gameObject, Vector3.one * 1.4f, 0.5f)
                 .setEaseInOutBack();*/
            yield return new WaitForSeconds(0.17f);


            //slam down
            LeanTween.move(Hand.target.gameObject, TargetPos, 0.02f)
                .setEaseInOutBack()
                .setEaseOutBounce();

            yield return new WaitForSeconds(.02f);
            //Hand.smashParticle.Play();

            yield return new WaitForSeconds(.04f);

            // LeanTween.scale(Hand.handTarget.gameObject, Vector3.one * 2, 0.3f)
            //  .setEasePunch();sadsa

            /*  yield return new WaitForSeconds(0.1f);

              LeanTween.scale(Hand.handTarget.gameObject, Vector3.one, 1.5f)
                  .setEaseOutElastic();
              LeanTween.move(Hand.handTarget.gameObject, _brain.transform.position + Hand.StartOffset, 1)
                  .setEaseInOutBack();*/
        }
    }

    [System.Serializable]
    public class EnragedChase : IState
    {
        BossWallMan _brain;
        public damageSphere DamageCollider_Jump;
        public bulletSpawner[] spiralSpawners;
        public IEnumerator CurrentAction;

        public float SpiralAttackDuration = 5.5f;
        public float jumpAttackDuration = 12f;
        public float ProjectileDuration = 7.5f;

        public float AttackDuration = 0;
        private int nextAttack = 0;
        public void SetBrain(BossWallMan brain)
        {
            _brain = brain;
        }
        public void enterState(StateManager manager)
        {
            // CoroutineHelper.RunCoroutine(EnragedJumpAttack());
        }
        public void exitState(StateManager manager)
        {
        }
        public void updateState(StateManager manager)
        {
            if (_brain.dead)
            {
                return;
            }
            if (AttackDuration <= 0)
            {
                switch (nextAttack)
                {

                    case 0:     //chase
                        CurrentAction = SpiralShoot();
                        CoroutineHelper.RunCoroutine(CurrentAction);
                        AttackDuration = SpiralAttackDuration;
                        nextAttack++;
                        break;

                    case 1:     //errectEyes
                        CurrentAction = EnragedJumpAttack();
                        CoroutineHelper.RunCoroutine(EnragedJumpAttack());
                        AttackDuration = jumpAttackDuration;
                        nextAttack++;
                        break;
                    case 2:     //errectEyes
                        CurrentAction = FireProjectiles();
                        CoroutineHelper.RunCoroutine(CurrentAction);
                        AttackDuration = ProjectileDuration;
                        nextAttack++;
                        break;
                    case 3:     //errectEyes
                        CurrentAction = EnragedJumpAttack();
                        CoroutineHelper.RunCoroutine(CurrentAction);
                        AttackDuration = jumpAttackDuration;
                        nextAttack++;
                        break;
                    default:
                        //out of attacks exit
                        nextAttack = 0;
                        return;
                }
            }
            else
                AttackDuration -= Time.deltaTime;
        }

        public IEnumerator FireProjectiles()
        {
            CoroutineHelper.RunCoroutine(_brain.moverArc(_brain.Center.position));
            yield return new WaitForSeconds(_brain.moveDuration + 0.2f);
            CoroutineHelper.RunCoroutine(_brain.hideState.fireCluster());

        }
        public void FixedUpdateState(StateManager manager)
        {
        }
        public IEnumerator EnragedJumpAttack()
        {
            //wait while jumping to middle of map
            yield return new WaitForSeconds(_brain.moveDuration + 0.1f);
            var timeStay = 1f;

            while (timeStay > 0)
            {
                //enraged animation


                timeStay -= Time.deltaTime;


                _brain.rotateBodySmooth(_brain.directionPlayerFLAT(), _brain.rotationSpeed);

                yield return new WaitForFixedUpdate();
            }

            var jumpsLeft = 3;
            Task m_JumpTask = null;

            //jump after player
            while (jumpsLeft > 0)
            {

                //coroutine done sceduel new jump
                if (m_JumpTask == null)
                {
                    m_JumpTask = new Task(_brain.moverArc(_brain.player.position));
                }

                if (!m_JumpTask.Running)
                {
                    m_JumpTask = null;
                    jumpsLeft--;
                    //open damage collider (jump has ended)
                    DamageCollider_Jump.OpenDamageCollider();
                    _brain.particleJump.Play();
                    yield return new WaitForSeconds(0.15f);
                    DamageCollider_Jump.CloseDamageCollider();

                    yield return new WaitForSeconds(0.45f);
                }
                else
                {
                    _brain.rotateBodySmooth(_brain.directionPlayerFLAT(), 8f);
                }
                yield return new WaitForFixedUpdate();
            }

            //coldown and reset
            _brain.anim.SetTrigger(_brain.m_Tired);

        }
        private IEnumerator SpiralShoot()
        {
            CoroutineHelper.RunCoroutine(_brain.moverArc(_brain.Center.position));
            yield return new WaitForSeconds(_brain.moveDuration + 0.8f);
            foreach (var BulletSpawner in spiralSpawners)
            {
                BulletSpawner.spiralStart();
            }
        }
    }
    #endregion
}