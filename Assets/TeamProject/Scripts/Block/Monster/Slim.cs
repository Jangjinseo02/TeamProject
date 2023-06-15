using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slim : Block
{
    [SerializeField] Item item;

    Animator anim;
    bool isDeadRoutine;

    public override void OnEnable()
    {
        base.OnEnable();

        boxCol.enabled = true;
        isDeadRoutine = false;
    }

    public override void Awake()
    {
        base.Awake();

        anim = GetComponent<Animator>();
    }

    public override void OnDamaged(int damage)
    {
        if (isDeadRoutine)
            return;

        health -= damage;

        if (health <= 0)
        {
            SoundManager.Instance.SfxPlay(SoundManager.Sfx.AttackMonster, false);

            isDeadRoutine = true;
            boxCol.enabled = false;
            anim.SetTrigger("IsDead");
            
            StartCoroutine(DeadRoutine());
        }
    }

    IEnumerator DeadRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        //GameObject dropItem = Instantiate(dropPrefab);
        group.blockManager.SettingBlockList(item.type, this.row, this.col);
        gameObject.SetActive(false);
    }

}
