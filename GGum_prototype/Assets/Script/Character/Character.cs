﻿using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class Character : MonoBehaviour {

    [Header("Status Setting")]
    public string id;

    public int maxHP;
    [HideInInspector]
    public int currentHP;

    public int maxShield;
    [HideInInspector]
    public int currentShield;

    public int attackDamage;

    public float attackSpeed;
    public float moveSpeed;
    public float jumpForce;

    [HideInInspector]
    public bool isStop;
    [HideInInspector]
    public bool onGround;
    //[HideInInspector]
    public bool isInvincible;
    protected float invincibleTime;
    protected Rigidbody2D m_rigidbody;
    protected Collider2D m_collider;

    [HideInInspector]
    public State state;

    //[HideInInspector]
    public StatePattern _statePattern;

    public Dictionary<Type, StatePattern> _statePatternList;

    [Header("Object Setting")]
    public Transform attackBox;
    public GameObject effect;
    public GameObject bullet;
    public GameObject container;
    public Animator animator;
    

    //public bool IsStop { get { return isStop; } set { isStop = value; } }

    //public string Id { get { return id; } set { id = value; } }

    //public int MaxHP { get { return maxHP; } set { maxHP = value; } }
    //public int CurrentHP { get { return currentHP; } set { currentHP = value; } }
    //public int MaxShield { get { return maxShield; } set { maxShield = value; } }
    //public int CurrentShield { get { return currentShield; } set { currentShield = value; } }
    //public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }

    //public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    //public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    //public float JumpForce { get { return jumpForce; } set { jumpForce = value; } }

    //public State CurrentState { get { return state; } set { state = value; } }


    protected virtual void InitCharacter()
    {
        currentHP = maxHP;
        currentShield = maxShield;
        onGround = false;
        isInvincible = false;
        isStop = false;
        m_rigidbody = GetComponent<Rigidbody2D>();

        _statePatternList = new Dictionary<Type, StatePattern>();

        if (currentHP > 0)
            state = State.Init;
        else
            state = State.Dead;
    }

    protected void Move(Axis axis, float keyValue)
    {
        if (axis == Axis.Horizontal)
        {
            transform.Translate(Vector2.right * keyValue * moveSpeed * Time.fixedDeltaTime);
        }
        else if (axis == Axis.Vertical)
        {
            transform.Translate(Vector2.up * keyValue * moveSpeed * Time.fixedDeltaTime);
        }
    }

    protected void Flip(float dir)
    {
        if (dir > 0)
            container.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (dir < 0)
            container.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    protected void Jump()
    {
        if (m_rigidbody != null)
        {
            m_rigidbody.velocity = Vector2.zero;
            m_rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        else
            Debug.Log("Null Rigidbody...");
    }

    virtual protected void Attack(HitData hitInfo) { }

    protected void CreateBullet(HitData hitInfo)
    {
        GameObject obj = (GameObject)Instantiate(bullet, attackBox.position, attackBox.rotation);
        obj.GetComponent<Bullet>().pHitData = hitInfo;
    }

    public void OnHit(HitData hitInfo)
    {
        if (/*_statePattern._currentState != "Dead"*/ state != State.Dead && !isInvincible)
        {
            int damage = hitInfo.damage;

            CalcDamage(ref currentShield, ref damage);

            if (damage > 0)
            {
                CalcDamage(ref currentHP, ref damage);
            }

            Debug.Log("Shield = " + currentShield + "HP = " + currentHP);

            if (currentHP <= 0)
            {
                // When Dead
                state = State.Dead;
                //_statePattern.SetState("Dead");
            }
            else
                HitFunc();
        }
    }

    public IEnumerator NoDamageForSeconds(float time)
    {
        isInvincible = true;
        yield return new WaitForSeconds(time);
        isInvincible = false;
    }

    private void CalcDamage(ref int point, ref int damage)
    {
        if (point > 0)
        {
            if (point >= damage)
            {
                point -= damage;
                damage = 0;
            } 
            else
            {
                damage -= point;
                point = 0;
            }
        }
    }

    protected bool IsGround(Vector2 originPos, float rayLength)
    {
        RaycastHit2D hit = Physics2D.Raycast(originPos, Vector2.down, rayLength, LayerMask.GetMask("Ground"));
        return (hit.collider != null);
    }

    protected virtual void HitFunc() { }


    protected virtual IEnumerator InitState()
    {
        yield return null;
    }

    protected virtual IEnumerator IdleState()
    {
        yield return null;
    }

    protected virtual IEnumerator MoveState()
    {
        yield return null;
    }

    protected virtual IEnumerator AttackState()
    {
        yield return null;
    }

    protected virtual IEnumerator HitState()
    {
        yield return null;
    }

    protected virtual IEnumerator DeadState()
    {
        yield return null;
    }

    public void NextState()
    {
        string methodName = state.ToString() + "State";
        MethodInfo info = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }
}
