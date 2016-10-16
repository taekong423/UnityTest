﻿using UnityEngine;
using System.Collections;

public class ChaseState : EnemyState {

    Transform _target;

    public ChaseState(Enemy enemy, Searchable searchable) : base(enemy, searchable)
    {
        CurrentState = "Chase";
    }

    protected override IEnumerator Enter()
    {
        _enemy.animator.SetTrigger("Move");

        _target = _enemy._player.transform;

        yield return null;
    }

    protected override IEnumerator Execute()
    {
        while (_enemy._statePattern is ChaseState)
        {
            _enemy.GoToTarget(_target.position);

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    protected override IEnumerator Exit()
    {
        _enemy._statePattern.StartState();

        yield return null;
    }

}
