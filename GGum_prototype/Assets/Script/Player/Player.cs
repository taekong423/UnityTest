﻿using UnityEngine;
using System.Collections;
using New;

public partial class Player : New.Character, IPlayer {

    #region 변수

    Animator _animator;

    Rigidbody2D _rigidbody;

    [SerializeField]
    CharacterMovement _movement;

    float _xAxis = 0;
    float _yAxis = 0;

    public float _test;

    [SerializeField]
    Weapon _weapon;

    bool _canAttack = true;
    bool _isAttacking = false;

    #endregion

    #region 프로퍼티

    public bool CanAttack { get { return _canAttack; } set { _canAttack = value; } }

    #endregion

    #region 메소드

    void Awake()
    {
        Global.shared<Player>(this);
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponentInParent<Rigidbody2D>();
    }

    void Update()
    {
        // Climb 가능한지, 지금 Climb하는 중이 아닌지 검사
        if (_movement.CanClimbing && !_movement.IsClimbing)
        {
            if (_yAxis != 0)
            {
                _movement.OnClimb();
                _animator.SetTrigger("Climb");
            }
        }
        // Climb 가능하지 않은지, 지금 Climb하는 중인지 검사
        else if(!_movement.CanClimbing && _movement.IsClimbing)
        {
            _movement.OffClimb();
        }
        PlayerAnimation();
        _test = GetComponentInParent<Rigidbody2D>().velocity.y;
    }


    void FixedUpdate()
    {

        MoveVertical();
        MoveHorizontal();

    }

    public override void NoIgnoreCollision(Collider2D[] colliders)
    {
        base.NoIgnoreCollision(colliders);
        _movement.CanCheckPlatform = true;
    }

    public override void IgnoreCollision(Collider2D[] colliders)
    {
        base.IgnoreCollision(colliders);
        _movement.CanCheckPlatform = false;
    }

    void PlayerAnimation()
    {
        _animator.SetFloat("xSpeed", Mathf.Abs(_movement.GetVelocity.x));
        _animator.SetFloat("ySpeed", Mathf.Abs(_movement.GetVelocity.y));
        _animator.SetBool("IsGround", _movement.IsGround);
        _animator.SetBool("IsClimbing", _movement.IsClimbing);
        _animator.SetBool("CanAttack", _canAttack);
    }

    void MoveHorizontal()
    {
        if (!_movement.IsClimbing)
        {
            //_movement.Move(Vector2.right, _xAxis);
            _movement.PhysicsXMove(_xAxis);
        }
        else
        {
            _movement.ClimbX(_xAxis);
        }
    }

    void MoveVertical()
    {
        if (_movement.IsClimbing)
        {
            _movement.ClimbY(_yAxis);
        }
    }

    public void Move(float xAxis, float yAxis)
    {
        _xAxis = xAxis;
        _yAxis = yAxis;
    }

    public void Jump()
    {
        if (_yAxis < 0 && CanThrough)
        {
            _movement.Jump(2500);
            IgnoreCollision(_ignoreColliders);
        }

        _movement.Jump();
    }

    public void OnFly()
    {
        _movement.OnFly();
    }

    public void OffFly()
    {
        _movement.OffFly();
    }

    IEnumerator AttackForSeconds()
    {
        _animator.SetTrigger("Attack");

        _isAttacking = true;

        yield return null;

        _canAttack = false;

        yield return new WaitForSeconds(_weapon.Delay);

        _canAttack = true;
        _isAttacking = false;

    }

    public void OnAttack()
    {
        if (_canAttack)
        {
            if(!_isAttacking)
                StartCoroutine(AttackForSeconds());
        }
    }

    public void Attack()
    {
        _weapon.Attack();
    }

    #endregion


    #region Old
    //enum PCState
    //{
    //    Any,
    //    Jump,
    //    Shoot,
    //    Ladder,
    //    Swim,
    //}

    //public int maxJumps;
    //public Transform fairyPoint;
    //public float currExp;
    //public float nextLevelExp;
    //[HideInInspector]
    //public int maxHpItem;
    //[HideInInspector]
    //public int attackDamageItem;
    //[HideInInspector]
    //public float attackSpeedItem;
    //[HideInInspector]
    //public float moveSpeedItem;

    //public ScreenFade screen;

    //private string lastState;
    //private PCState playerState;
    //private int jumpCounter;
    //private bool canShoot;
    //private bool canClimb;
    //private float hAxis;
    //private float vAxis;
    //private float rayLength;
    //private Vector3 rayPosCenter;
    //private Vector3 rayPosRight;
    //private Vector3 rayPosLeft;
    //private GameObject ladder;
    //private SpriteRenderer spriteRenderer;

    //private bool currGroundCheck;
    //private bool lastGroundCheck;

    //float alpha;
    //float rayPosX;
    //float rayPosY;

    //bool blinkOn;
    //bool check;


    //// Use this for initialization
    //void Awake()
    //{
    //    InitCharacter();
    //    _statePattern = new PlayerState(this);
    //    _statePattern.StartState();
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    KeyInput();

    //    if (blinkOn)
    //    {
    //        BlinkSprite();
    //    }
    //}

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(rayPosRight, rayPosRight - new Vector3(0, rayLength, 0));
    //    Gizmos.DrawLine(rayPosLeft, rayPosLeft - new Vector3(0, rayLength, 0));
    //}

    //protected override void InitCharacter()
    //{
    //    base.InitCharacter();

    //    canClimb = false;
    //    canShoot = true;
    //    currGroundCheck = false;
    //    lastGroundCheck = false;
    //    blinkOn = false;
    //    m_collider = GetComponent<CircleCollider2D>();
    //    rayLength = 2.5f;
    //    rayPosX = 2.1f;
    //    rayPosY = 1.0f;
    //    invincibleTime = 2.0f;

    //    spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    //    soundPlayer = GetComponent<SoundPlayer>();
    //}

    //private void KeyInput()
    //{
    //    GroundCheck();

    //    if (!isStop && !CheckState("Dead"))
    //    {
    //        // Input Jump Key
    //        if (GameController.ButtonDown(EButtonCode.Jump))
    //        {
    //            if (currGroundCheck)
    //            {
    //                jumpCounter = 0;
    //                m_rigidbody.gravityScale = 10;
    //            }

    //            if (canClimb)
    //            {
    //                jumpCounter = 0;
    //                canClimb = false;
    //            }

    //            if (jumpCounter < maxJumps)
    //            {
    //                playerState = PCState.Jump;
    //                SetTrigger("Jump");
    //                PlaySound("Jump");
    //                m_rigidbody.velocity = Vector2.zero;
    //                Jump();
    //                m_rigidbody.gravityScale = 50;
    //                jumpCounter++;
    //            }
    //        }

    //        // Input Attack Key
    //        if (GameController.ButtonPress(EButtonCode.Attack))
    //        {
    //            if (canShoot && !canClimb)
    //            {
    //                StartCoroutine(Shoot());
    //            }
    //        }
    //    }
    //}

    //private void AxisInput()
    //{
    //    if (!isStop && !CheckState("Dead"))
    //    {

    //        hAxis = GameController.GetAxisRaw(EButtonCode.MoveX);
    //        vAxis = GameController.GetAxisRaw(EButtonCode.MoveY);

    //        Flip(hAxis);

    //        if (canClimb)
    //        {
    //            if (vAxis != 0)
    //            {
    //                InitLadder();
    //                Move(Axis.Vertical, vAxis);
    //            }
    //            else if (vAxis == 0 && hAxis != 0)
    //            {
    //                if (_statePattern.CurrentState != "Move")
    //                    _statePattern.SetState("Move");
    //                SetTrigger("Run");
    //                Move(Axis.Horizontal, hAxis);
    //            }
    //        }
    //        else
    //        {
    //            if (hAxis == 0)
    //            {
    //                if (_statePattern.CurrentState != "Idle")
    //                    _statePattern.SetState("Idle");
    //            }
    //            else
    //            {
    //                if (_statePattern.CurrentState != "Move")
    //                    _statePattern.SetState("Move");
    //                Move(Axis.Horizontal, hAxis);
    //            }
    //        }
    //    }
    //}

    //private void InitLadder()
    //{
    //    playerState = PCState.Ladder;
    //    SetTrigger("Ladder");
    //    if(!isStop)
    //        m_rigidbody.gravityScale = 0;
    //    m_rigidbody.velocity = Vector2.zero;
    //    gameObject.transform.position = new Vector3(ladder.transform.position.x, gameObject.transform.position.y, 0);
    //    ladder.GetComponent<LadderCollisionTrigger>().IgnorePlatform(true);
    //}

    //private void GroundCheck()
    //{
    //    rayPosCenter = transform.position + new Vector3(0, rayPosY, 0);
    //    rayPosRight = rayPosCenter + new Vector3(rayPosX, 0, 0);
    //    rayPosLeft = rayPosCenter + new Vector3(-rayPosX, 0, 0);

    //    if (IsGround(rayPosLeft, rayLength) || IsGround(rayPosRight, rayLength))
    //    {
    //        currGroundCheck = true;
    //    }
    //    else
    //    {
    //        currGroundCheck = false;
    //    }

    //    if (currGroundCheck != lastGroundCheck)
    //    {
    //        if (currGroundCheck)
    //        {
    //            if (playerState != PCState.Any)
    //            {
    //                SetAnimationBack();
    //                playerState = PCState.Any;
    //            }
    //            if(!isStop)
    //                m_rigidbody.gravityScale = 10;
    //        }
    //        else
    //        {
    //            playerState = PCState.Jump;

    //            if (!canClimb && !isStop)
    //                m_rigidbody.gravityScale = 50;
    //        }
    //    }

    //    lastGroundCheck = currGroundCheck;
    //}

    //private float GetAttackSpeed(float attackSpeed)
    //{
    //    if (attackSpeed <= 0)
    //        return 1.0f;
    //    else
    //        return 1.0f / attackSpeed;
    //}

    //protected override void Attack(HitData hitInfo)
    //{
    //    CreateBullet(hitInfo);
    //}

    //IEnumerator Shoot()
    //{
    //    canShoot = false;
    //    PlaySound("Attack");

    //    if (playerState == PCState.Jump)
    //    {
    //        SetBool("JumpAttack", true);
    //        SetTrigger("Attack");
    //    }
    //    else
    //    {
    //        SetBool("JumpAttack", false);
    //        SetBool("CriticalAttack", false);
    //        SetTrigger("Attack");
    //    }

    //    Instantiate(effect, attackBox.position, Quaternion.identity);
    //    Attack(new HitData(gameObject, attackDamage + attackDamageItem));

    //    yield return new WaitForSeconds(GetAttackSpeed(attackSpeed + attackSpeedItem));

    //    SetAnimationBack();

    //    canShoot = true;
    //}

    //private void SetAnimationBack()
    //{
    //    if (CheckState("Idle"))
    //        SetTrigger("Idle");
    //    else if (CheckState("Move"))
    //        SetTrigger("Run");
    //}

    //private bool CheckColliderByLayer(string layerName, Collider2D other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer(layerName))
    //        return true;
    //    else
    //        return false;
    //}

    //protected override void HitFunc()
    //{
    //    StartCoroutine(Hit());
    //}

    //protected IEnumerator Hit()
    //{
    //    PlaySound("Hit");
    //    isInvincible = true;

    //    if (playerState != PCState.Ladder)
    //    {
    //        SetTrigger("Hit");
    //        if (!isStop)
    //        {
    //            isStop = true;
    //            yield return new WaitForSeconds(0.5f);
    //            isStop = false;
    //            SetAnimationBack();
    //        }
    //        else
    //        {
    //            yield return new WaitForSeconds(0.5f);

    //            SetAnimationBack();
    //        }

    //    }

    //    blinkOn = true;

    //    yield return new WaitForSeconds(invincibleTime);
    //    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1.0f);
    //    isInvincible = false;
    //    blinkOn = false;
    //}

    //private void BlinkSprite()
    //{
    //    alpha = spriteRenderer.color.a;

    //    if (check)
    //        alpha -= Time.deltaTime;
    //    else
    //        alpha += Time.deltaTime;

    //    if (alpha > 0.7f)
    //    {
    //        check = true;
    //    }
    //    else if (alpha < 0.4f)
    //    {
    //        check = false;
    //    }

    //    alpha = Mathf.Clamp(alpha, 0, 1);

    //    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
    //}

    //public bool CheckState(string stateName)
    //{
    //    if (_statePattern.CurrentState == stateName)
    //        return true;
    //    else
    //        return false;
    //}

    //public void SetTrigger(string name)
    //{
    //    animator.SetTrigger(name);
    //}

    //public void SetBool(string name, bool check)
    //{
    //    animator.SetBool(name, check);
    //}

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (CheckColliderByLayer("Ladder", other))
    //    {
    //        if (!canClimb)
    //        {
    //            canClimb = true;
    //            ladder = other.gameObject;
    //        }
    //    }

    //    if (other.CompareTag("Enemy"))
    //    {
    //        Enemy enemy = other.GetComponent<Enemy>();

    //        HitData hitData;
    //        if (enemy != null)
    //        {
    //            hitData = enemy.pHitData;
    //        }
    //        else
    //        {
    //            hitData = other.GetComponent<NewEnemy>().pHitData;
    //        }
    //        OnHit(hitData);
    //    }

    //    if (other.CompareTag("Bullet"))
    //    {
    //        HitData hitData = other.GetComponent<Bullet>().pHitData;
    //        if (!hitData.attacker.CompareTag("Player"))
    //        {
    //            OnHit(hitData);
    //            other.GetComponent<Bullet>().DestroyBullet(0.0f);
    //        }
    //    }
    //}

    //void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
    //    {
    //        if (canClimb)
    //        {
    //            canClimb = false;
    //            m_rigidbody.gravityScale = 50;
    //            jumpCounter = 0;
    //        }
    //    }
    //} 
    #endregion
}
