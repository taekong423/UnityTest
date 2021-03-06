﻿using UnityEngine;
using System.Collections;

public class DeadState : EnemyState {

    public DeadState(Enemy enemy) : base(enemy)
    {
        CurrentState = "Dead";
    }

    protected override IEnumerator Enter()
    {
        _enemy.isInvincible = true;
        _enemy.GetComponent<BoxCollider2D>().enabled = false;

        _enemy.Dead();

        yield return null;
    }

    protected override IEnumerator Execute()
    {
        _enemy.animator.SetTrigger("Hit");

        yield return new WaitForSeconds(0.5f);
    }

    protected override IEnumerator Exit()
    {
        _enemy.SetStatePattern<InitState>();

        _enemy.currentHP = _enemy.maxHP;
        _enemy._isHitEffectDelay = false;

        yield return null;

        _enemy.gameObject.SetActive(false);
    }
}

public class NewDeadState : DeadState
{
    public NewDeadState(Enemy enemy) : base(enemy)
    {

    }

    protected override IEnumerator Execute()
    {
        _enemy.animator.SetTrigger("Dead");

        float animTime = _enemy.animator.GetCurrentAnimatorStateInfo(0).length;

        while (animTime > 0.0f)
        {
            animTime -= Time.deltaTime;
            yield return null;
        }

    }
}
