using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slim : Block
{
    [SerializeField] GameObject dropPrefab;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public override void OnDamaged(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            //type = -1;
            StartCoroutine(DeadRoutine());
        }
    }

    IEnumerator DeadRoutine()
    {
        anim.SetTrigger("IsDead");
        yield return new WaitForSeconds(1f);
        //GameObject dropItem = Instantiate(dropPrefab);
        group.blockManager.SettingBlockList(13, this.row, this.col);
        gameObject.SetActive(false);
    }

}
