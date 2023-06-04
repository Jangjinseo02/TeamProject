using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slim : Block
{
    [SerializeField] GameObject dropPrefab;

    Animator anim;
    BoxCollider2D bodyBoxCol;
    bool isDeadRoutine;

    public override void OnEnable()
    {
        isDeadRoutine = false;
    }

    public override void Awake()
    {
        base.Awake();

        anim = GetComponent<Animator>();
        bodyBoxCol = GetComponent<BoxCollider2D>();
    }

    public override void OnDamaged(int damage)
    {
        if (isDeadRoutine)
            return;

        health -= damage;

        if (health <= 0)
        {
            isDeadRoutine = true;
            bodyBoxCol.enabled = false;
            anim.SetTrigger("IsDead");
            
            StartCoroutine(DeadRoutine());
        }
    }

    IEnumerator DeadRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        //GameObject dropItem = Instantiate(dropPrefab);
        group.blockManager.SettingBlockList(12, this.row, this.col);
        gameObject.SetActive(false);
    }

}
