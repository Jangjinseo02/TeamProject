using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardBlock : Block
{
    bool isDestory;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        health = 50f;
        isDestory = false;
    }

    public override void OnDamaged(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            isDestory = true;

            if (gameObject.activeInHierarchy)
                StartCoroutine(DestroyRoutine());
        }
    }

    IEnumerator DestroyRoutine()
    {
        effect = ObjectManager.Instance.GetEffect((int)ObjectManager.effect.Hard);
        effect.transform.position = this.transform.position;
        sprite.color = Color.clear;
        effect.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        ObjectManager.Instance.ReturnEffect(effect, (int)ObjectManager.effect.Hard);

        yield return new WaitForSeconds(0.05f);
        foreach (Block member in this.group)
        {
            if (this == member)
                continue;
            member.RemoveGroup(group.blockManager);
            member.group.CheckGroupUnbalance();
        }

        group.blockManager.RemovePos(this);
        gameObject.SetActive(false);
    }

    public bool isDestroy()
    {
        return isDestory;
    }
}
