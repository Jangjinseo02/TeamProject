using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slim : Block
{
    [SerializeField] GameObject dropPrefab;

    Animator anim;
    bool isDeadRoutine;

    private void OnEnable()
    {
        isDeadRoutine = false;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnDamaged(int damage)
    {
        if (isDeadRoutine)
            return;

        health -= damage;

        if (health <= 0)
        {
            //type = -1;
            StartCoroutine(DeadRoutine());
        }
    }

    IEnumerator DeadRoutine()
    {
        isDeadRoutine = true;
        anim.SetTrigger("IsDead");
        yield return new WaitForSeconds(1f);
        //GameObject dropItem = Instantiate(dropPrefab);
        group.blockManager.SettingBlockList(13, this.row, this.col);
        gameObject.SetActive(false);
    }

}
