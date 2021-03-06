﻿using UnityEngine;
using System.Collections;

public class ThrowSquirrel : Enemy {


    protected override void InitCharacter()
    {
        base.InitCharacter();

        _statePatternList.Add(typeof(InitState), new InitState(this));
        _statePatternList.Add(typeof(IdleState), new ThrowIdleState(this));
        _statePatternList.Add(typeof(AttackState), new ThrowState(this));
        _statePatternList.Add(typeof(HitState), new HitState(this, 1.5f, 0.5f, null));
        _statePatternList.Add(typeof(DeadState), new DeadState(this));

        SetStatePattern<InitState>();

        //_statePatternList.Add(typeof(ThrowPattern), new ThrowPattern(this));

        //SetStatePattern<ThrowPattern>();
    }

    protected override void Attack(HitData hitInfo)
    {
        GameObject obj = Instantiate(bullet, attackBox.position, attackBox.rotation) as GameObject;

        //obj.GetComponent<Bullet>().pHitData = pHitData;
    }

    public class ThrowIdleState : IdleState
    {
        public ThrowIdleState(Enemy enemy) : base(enemy, null)
        {

        }

        protected override IEnumerator Execute()
        {
            while (_enemy._statePattern is IdleState)
            {
                if (_delay <= 0)
                {
                    _enemy.SetStatePattern<AttackState>();
                    _delay = _enemy._attackDelay;
                }
                else
                {
                    _delay -= Time.deltaTime;
                }

                yield return null;
            }

            yield return null;
        }

    }


    public class ThrowState : AttackState
    {
        public ThrowState(Enemy enemy) : base(enemy, null)
        {
        }

        protected override IEnumerator Enter()
        {
            _enemy.animator.SetTrigger("Attack");

            animTime = _enemy.animator.GetCurrentAnimatorStateInfo(0).length;

            yield return null;
        }

        protected override IEnumerator Execute()
        {
            float delay = 0.0f;

            while (_enemy._statePattern is AttackState)
            {
                delay += Time.deltaTime;

                if (delay > animTime + 0.2f)
                {
                    _enemy.OnAttack();
                    _enemy.SetStatePattern<IdleState>();
                    break;                    
                }

                yield return null;
            }

            yield return null;
        }

    }


    //protected override void HitFunc()
    //{
    //    if (!_isHitEffectDelay)
    //    {
    //        _statePattern.SetState("Hit");
    //        _isHitEffectDelay = true;
    //    }
    //}

    public class ThrowPattern : EnemyState
    {
        ThrowSquirrel _squirrel;

        public ThrowPattern(Enemy enemy) : base(enemy)
        {
            _squirrel = enemy as ThrowSquirrel;
        }

        public override void StartState()
        {
            NextState("Init");
        }

        public override void SetState(string value)
        {
            CurrentState = value;
        }

        IEnumerator InitState()
        {
            SetState("Idle");
            _enemy.GetComponent<BoxCollider2D>().enabled = true;

            yield return null;

            NextState(CurrentState);

        }

        IEnumerator IdleState()
        {
            _enemy.animator.SetTrigger("Idle");

            yield return null;

            float delay = _squirrel._attackDelay;

            while (CurrentState == "Idle")
            {
                if (delay <= 0.0f)
                {
                    SetState("Throw");
                }
                else
                {
                    delay -= Time.deltaTime;
                }

                yield return null;
            }

            NextState(CurrentState);
        }


        IEnumerator ThrowState()
        {
            _enemy.animator.SetTrigger("Attack");

            GameObject obj = Instantiate(_squirrel.bullet, _squirrel.attackBox.position, _squirrel.attackBox.rotation) as GameObject;

            //obj.GetComponent<Bullet>().pHitData = _squirrel.pHitData;

            yield return null;

            //while (CurrentState == "Throw")
            //{
            //    Debug.Log("ThrowLoop");
            //    yield return null;
            //}

            yield return new WaitForSeconds(0.5f);

            SetState("Idle");

            yield return null;

            NextState(CurrentState);
        }

        IEnumerator HitState()
        {
            _enemy.animator.SetTrigger("Hit");
            _enemy.PlaySound("Hit");

            yield return null;

            float hitDelay = 0;

            while (hitDelay <= 1.5f)
            {
                hitDelay += Time.deltaTime;

                if (hitDelay >= 0.5f && CurrentState == "Hit")
                {
                    SetState("Idle");
                    NextState(CurrentState);
                }

                yield return null;
            }

            _enemy._isHitEffectDelay = false;

        }

        IEnumerator DeadState()
        {
            
            _enemy.isInvincible = true;
            _enemy.GetComponent<BoxCollider2D>().enabled = false;

            _enemy.Dead();

            yield return null;

            _enemy.animator.SetTrigger("Hit");

            yield return new WaitForSeconds(0.5f);

            SetState("Init");

            _enemy.currentHP = _enemy.maxHP;
            _enemy._isHitEffectDelay = false;

            _enemy.gameObject.SetActive(false);
        }

    }

}
