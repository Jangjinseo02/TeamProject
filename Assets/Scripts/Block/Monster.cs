using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Block
{
    public enum Type { Monster_A, Monster_B }
    public Type monsterType;
    public virtual void Setting()
    {

    }

    public virtual void Attack()
    {

    }

    public virtual void Skil()
    {

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
        //anim.SetTrigger("IsDead");
        yield return new WaitForSeconds(1f);
        //GameObject dropItem = ObjectManager.Instance.GetBlock(12);
        //dropItem.GetComponent<IItem>().Set(gameObject);
        GameObject item = group.blockManager.SettingBlockList(11, this.row, this.col);
        item.GetComponent<UseItem>().SpriteSetting((int)monsterType);
        gameObject.SetActive(false);
    }
}
