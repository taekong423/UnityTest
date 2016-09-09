﻿using UnityEngine;
using System.Collections;

public class Squirrel : Enemy {


    protected override IEnumerator InitState()
    {
        state = State.Idle;

        NextState();
        StartCoroutine(Search());

        yield return null;
    }

    protected override IEnumerator IdleState()
    {
        while (state == State.Idle)
        {

            if (_currentMoveDleay <= 0.0f)
            {
                _currentMoveDleay = _moveDelay;

                if (_wayPoints.Length != 0)
                    _target = _wayPoints[_numWayPoint];

                state = State.Move;
            }
            else
            {
                _currentMoveDleay -= Time.fixedDeltaTime;
            }

            yield return null;

        }

        NextState();

        yield return null;
    }

    protected override IEnumerator MoveState()
    {

        while (state == State.Move)
        {
            if (_target != null)
            {
                //도착.
                if (GoToTarget(_target.position))
                {
                    SetWayPointNum();
                    _target = _wayPoints[_numWayPoint];
                    state = State.Idle;
                }

            }

            yield return new WaitForFixedUpdate();
        }

        NextState();

        yield return null;
    }

    protected override IEnumerator HitState()
    {



        yield return null;
    }

    protected override IEnumerator DeadState()
    {



        yield return null;
    }

    protected override IEnumerator Search()
    {
        while (state != State.Dead)
        {
            if (state != State.Attack && state != State.Hit)
            {

                if (Search(_player.transform, _detectionRange))
                {
                    _currentMoveDleay = _moveDelay;
                    _target = _player.transform;
                    state = State.Move;
                }
                else
                {
                    if (_target == _player.transform)
                    {
                        if (_wayPoints.Length != 0)
                            _target = _wayPoints[_numWayPoint];
                    }
                }

            }

            yield return null;
        }


        yield return null;
    }

}
