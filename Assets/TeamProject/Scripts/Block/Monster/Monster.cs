using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Block
{
    public enum Type { Monster_A, Monster_B }
    public Type monsterType;

    public bool isDead;
    public bool inCamera;

    protected Animator anim;

    public override void Awake()
    {
        base.Awake();
    }

    public virtual void CameraOut()
    {
        inCamera = false;
    }
    public virtual void CheckTarget(HashSet<Block> blocks)
    {
        inCamera = true;
    }

    public virtual void Attack()
    {
        if (isDead || !inCamera)
            return;
        Debug.Log("몬스터 공격");

        //공격 애니메이션
        //대상 체력 감소
    }

    public override void OnDamaged(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            if (isDead)
                return;

            isDead = true;
            boxCol.enabled = false;
            anim.SetTrigger("Dead");
            StartCoroutine(DeadRoutine());
        }
    }

    IEnumerator DeadRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        group.blockManager.SettingBlockList(11, this.row, this.col);
        gameObject.SetActive(false);
    }


}
