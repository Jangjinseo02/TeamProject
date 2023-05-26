using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Block
{
    public enum Type { Monster_A, Monster_B }
    public Type monsterType;

    public bool isDead;
    public bool inCamera;

    private void Awake()
    {
        //anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        isDead = false;
        inCamera = false;
    }

    public virtual void CameraOut()
    {
        inCamera = false;
    }
    public virtual void CheckTarget(HashSet<Block> blocks)
    {
        inCamera = true;
        Debug.Log("Parent : CheckTarget");
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
    }

    
}
