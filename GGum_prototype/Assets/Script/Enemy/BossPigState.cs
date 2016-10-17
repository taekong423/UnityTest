﻿using UnityEngine;
using System.Collections;

public partial class BossPig {

    public class BossPigState : EnemyState
    {
        public enum Stat
        {
            Init,
            Idle,
            Move,
            Attack,
            Hit,
            Dead,
        }


        protected BossPig _bossPig;

        protected Stat _state;

        protected CameraController _camera;

        public BossPigState(Enemy enemy) : base(enemy)
        {
            _bossPig = enemy as BossPig;
        }

        public override void SetState(string value)
        {
            Debug.Log("bosspig : " + value);
            SetState<Stat>(ref _state, value);
        }

        protected IEnumerator IdleStay()
        {
            while (_state == Stat.Idle)
            {
                if (_bossPig._currentMoveDleay <= 0)
                {
                    SetState("Move");
                    _bossPig._currentMoveDleay = _bossPig._moveDelay;
                    _bossPig._target = _bossPig._wayPoints[_bossPig._numWayPoint];
                }
                else
                {
                    _bossPig._currentMoveDleay -= Time.deltaTime;
                }

                yield return null;
            }

            NextState(_state.ToString());
        }

        protected IEnumerator HitState()
        {
            _bossPig.SoundPlay("Hit");
            _bossPig.animator.SetTrigger("Hit");
            _bossPig.OroraActive(true);
            _bossPig.isInvincible = false;
            _bossPig._isHide = false;
            _bossPig.m_collider.enabled = true;

            yield return new WaitForSeconds(1.0f);

            SetState("Idle");

            yield return null;

            NextState(_state.ToString());
        }

        protected IEnumerator DeadState()
        {
            _bossPig.ChildAllDead();

            _bossPig.OroraActive(true);
            _bossPig.animator.SetTrigger("Hit");

            if (_bossPig._hpBar)
                _bossPig._hpBar.transform.parent.gameObject.SetActive(false);

            _bossPig.isInvincible = true;
            _bossPig._isHide = true;
            _bossPig.m_collider.enabled = false;

            yield return new WaitForSeconds(0.5f);

            _camera.ShakeCamera(1.0f);
            _bossPig.animator.SetTrigger("Hide");

            yield return new WaitForSeconds(1.0f);

            _bossPig.gameObject.SetActive(false);

        }
    }

    public class Appear : BossPigState
    {

        public Appear(Enemy enemy) : base(enemy)
        {
        }

        public override void StartState()
        {
            NextState("Appear");
        }

        IEnumerator AppearState()
        {
            _bossPig.isInvincible = true;
            _bossPig.m_collider.enabled = false;

            _bossPig.SetOrora(0);
            _bossPig.OroraActive(true);
            if (_bossPig._hpBar)
                _bossPig._hpBar.transform.parent.gameObject.SetActive(false);

            yield return null;

            _camera = Global.shared<CameraController>();

            yield return new WaitForSeconds(1.0f);

            _camera.ShakeCamera(1.0f);

            _bossPig._isHide = true;

            yield return new WaitForSeconds(0.5f);

            _bossPig.animator.SetTrigger("Hide");

            yield return new WaitForSeconds(0.5f);

            EnemyState statePattern = _bossPig._statePatternList[typeof(Pattern0)] as Pattern0;

            _bossPig._statePattern = statePattern;
            statePattern.StartState();

        }
    }

    public class Pattern0 : BossPigState
    {
        public Pattern0(Enemy enemy) : base(enemy)
        {
        }

        public override void StartState()
        {
            NextState("Init");
        }

        IEnumerator InitState()
        {
            SetState("Idle");
            _bossPig.moveSpeed = _bossPig._baseMoveSpeed;
            _bossPig.SetOrora(0);
            _bossPig.OroraActive(true);
            if (_bossPig._hpBar)
                _bossPig._hpBar.transform.parent.gameObject.SetActive(true);

            yield return null;

            _camera = Global.shared<CameraController>();
            
            NextState(_state.ToString());
        }

        IEnumerator IdleState()
        {

            if (!_bossPig._isHide)
            {
                _camera.ShakeCamera(1.0f);
                _bossPig.isInvincible = true;
                _bossPig._isHide = true;
                _bossPig.m_collider.enabled = false;

                yield return new WaitForSeconds(0.5f);

                _bossPig.OroraActive(false);
                _bossPig.animator.SetTrigger("Hide");

                yield return new WaitForSeconds(0.5f);
            }

            yield return null;

            yield return IdleStay();

        }

        IEnumerator MoveState()
        {
            if (!_bossPig._isHide)
            {
                _camera.ShakeCamera(1.0f);
                _bossPig.isInvincible = true;
                _bossPig._isHide = true;
                _bossPig.m_collider.enabled = false;

                yield return new WaitForSeconds(0.5f);

                _bossPig.OroraActive(false);
                _bossPig.animator.SetTrigger("Hide");

                yield return new WaitForSeconds(0.5f);
            }

            yield return null;

            bool arrive = false;

            while (_state == Stat.Move)
            {
                if (_bossPig._target != null && !arrive)
                    arrive = _bossPig.GoToTarget(_bossPig._target.position);

                if (arrive)
                {
                    _bossPig.SetWayPointNum();

                    if (_bossPig._childPigNum <= 0)
                    {
                        SetState("Attack");
                    }
                    else
                    {
                        SetState("Idle");
                    }

                }

                yield return new WaitForFixedUpdate();
            }

            NextState(_state.ToString());

        }

        IEnumerator AttackState()
        {
            _bossPig.StartCoroutine(Attack());

            while (_state == Stat.Attack)
            {
                yield return null;
            }

            if (_state == Stat.Dead)
                _bossPig.StopAllCoroutines();

            NextState(_state.ToString());
        }

        IEnumerator Attack()
        {
            _camera.ShakeCamera(1.0f);
            yield return new WaitForSeconds(1.0f);

            _bossPig.isInvincible = false;
            _bossPig._isHide = false;
            _bossPig.m_collider.enabled = true;

            _bossPig.LookTarget(_bossPig._player.transform);

            _bossPig.OroraActive(true);
            _bossPig.animator.SetTrigger("Attack");

            _bossPig.SpawNormalPig();

            yield return new WaitForSeconds(3.0f);

            if (_state != Stat.Dead)
                SetState("Idle");

            yield return null;

        }

        public override void HitFunc()
        {
            if (_bossPig.currentHP <= _bossPig.maxHP * 0.7f)
            {
                SetState("Init");

                EnemyState statePattern = _bossPig._statePatternList[typeof(Pattern1)] as Pattern1;

                _bossPig._statePattern = statePattern;

                _bossPig.transform.localScale -= _bossPig._baseSize * 0.15f;

                _bossPig.StopAllCoroutines();

                statePattern.SetState("Init");

                statePattern.NextState(statePattern.CurrentState);
            }
        }

    }

    public class Pattern1 : BossPigState
    {
        int _attackCount = 0;

        public Pattern1(Enemy enemy) : base(enemy)
        {

        }

        public override void StateLog()
        {
            Debug.Log(CurrentState);
        }

        IEnumerator InitState()
        {
            SetState("Idle");
            _bossPig.OroraActive(false);
            _bossPig.SetOrora(1);
            yield return null;

            _camera = Global.shared<CameraController>();

            NextState(_state.ToString());
        }

        IEnumerator IdleState()
        {
            if (!_bossPig._isHide)
            {
                _camera.ShakeCamera(1.0f);
                _bossPig.isInvincible = true;
                _bossPig._isHide = true;
                _bossPig.m_collider.enabled = false;

                yield return new WaitForSeconds(0.5f);

                _bossPig.OroraActive(false);
                _bossPig.animator.SetTrigger("Hide");

                yield return new WaitForSeconds(0.5f);
            }

            yield return null;

            yield return IdleStay();
        }

        IEnumerator MoveState()
        {
            if (!_bossPig._isHide)
            {
                _camera.ShakeCamera(1.0f);
                _bossPig.isInvincible = true;
                _bossPig._isHide = true;
                _bossPig.m_collider.enabled = false;

                yield return new WaitForSeconds(0.5f);

                _bossPig.OroraActive(false);
                _bossPig.animator.SetTrigger("Hide");

                yield return new WaitForSeconds(0.5f);
            }

            yield return null;

            float patternDelay = 3;

            while (_state == Stat.Move)
            {
                if (_bossPig.GoToTarget(_bossPig._target.position))
                {
                    _bossPig.SetWayPointNum();

                    SetState("Idle");
                }
                else
                {
                    if (patternDelay <= 0)
                    {
                        SetState("Attack");
                    }
                    else
                        patternDelay -= Time.fixedDeltaTime;
                }
                yield return new WaitForFixedUpdate();
            }

            NextState(_state.ToString());
        }

        IEnumerator AttackState()
        {
            _bossPig.StartCoroutine(Attack());

            while (_state == Stat.Attack)
            {
                yield return null;
            }

            if (_state == Stat.Dead)
                _bossPig.StopAllCoroutines();

            NextState(_state.ToString());
        }

        IEnumerator Attack()
        {

            _camera.ShakeCamera(1.0f);
            _bossPig.isInvincible = true;
            _bossPig._isHide = true;
            _bossPig.m_collider.enabled = false;

            yield return new WaitForSeconds(0.5f);

            _bossPig.animator.SetTrigger("Hide2");

            yield return new WaitForSeconds(2.0f);

            _camera.ShakeCamera(1.0f);

            yield return new WaitForSeconds(0.5f);

            RaycastHit2D hit = Physics2D.Raycast(_bossPig._player.transform.position, Vector2.down, Mathf.Infinity, LayerMask.GetMask("Ground"));

            _bossPig.transform.position = hit.point;

            yield return new WaitForSeconds(0.5f);

            _bossPig.isInvincible = false;
            _bossPig._isHide = false;
            _bossPig.m_collider.enabled = true;

            _bossPig.OroraActive(true);
            _bossPig.animator.SetTrigger("Attack");

            _attackCount++;

            _bossPig._numWayPoint = Random.Range(0, _bossPig._wayPoints.Length);

            if (_attackCount >= 4)
            {
                _attackCount = 0;
                _bossPig.RandomSpawnPig();
            }

            yield return new WaitForSeconds(1.0f);

            if(_state != Stat.Dead)
                SetState("Idle");

            yield return null;

        }

        public override void HitFunc()
        {
            if (_bossPig.currentHP <= _bossPig.maxHP * 0.4f)
            {
                SetState("Init");

                EnemyState statePattern = _bossPig._statePatternList[typeof(Pattern2)] as Pattern2;

                _bossPig._statePattern = statePattern;

                _bossPig.moveSpeed *= 6;
                _bossPig.transform.localScale -= _bossPig._baseSize * 0.15f;

                _bossPig.StopAllCoroutines();

                statePattern.SetState("Init");

                statePattern.NextState(statePattern.CurrentState);
            }
        }

    }

    public class Pattern2 : BossPigState
    {
        public Pattern2(Enemy enemy) : base(enemy)
        {

        }

        public override void StateLog()
        {
            Debug.Log("CurrentState : "+CurrentState);
        }

        IEnumerator InitState()
        {
            SetState("Idle");
            _bossPig.OroraActive(false);
            _bossPig.SetOrora(2);
            yield return null;

            _camera = Global.shared<CameraController>();
            NextState(_state.ToString());
        }

        IEnumerator IdleState()
        {
            if (_bossPig._isHide)
            {
                _camera.ShakeCamera(1.0f);

                yield return new WaitForSeconds(0.5f);

                _bossPig.isInvincible = false;
                _bossPig._isHide = false;
                _bossPig.m_collider.enabled = true;

                _bossPig.GetComponent<BoxCollider2D>().enabled = true;

                
                _bossPig.animator.SetTrigger("Rush");

                yield return new WaitForSeconds(0.5f);

            }
            _bossPig.OroraActive(true);
            yield return null;

            yield return IdleStay();
        }

        IEnumerator MoveState()
        {
            StateLog();
            _bossPig.animator.SetTrigger("Rush");

            yield return null;

            bool arrive = false;

            while (_state == Stat.Move)
            {
                if(!arrive)
                    arrive = _bossPig.GoToTarget(_bossPig._target.position);

                if (arrive)
                {
                    SetState("Idle");
                    _bossPig.SetWayPointNum();
                    _bossPig._currentMoveDleay = 1.5f;
                    _bossPig.LookTarget(_bossPig._player.transform);
                }

                yield return new WaitForFixedUpdate();
            }

            NextState(_state.ToString());
        }
    }
}
