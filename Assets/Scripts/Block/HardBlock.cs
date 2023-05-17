using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardBlock : Block
{
    bool isDestory;


    private void OnEnable()
    {
        health = 50f;
        isDestory = false;
    }

    public override void OnDamaged(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            isDestory = true;
            group.blockManager.RemovePos(this);

            //Ãß°¡
            foreach (Block member in this.group)
            {
                if (this == member)
                    continue;
                member.RemoveGroup(group.blockManager);
                member.group.CheckGroupUnbalance();
            }

            gameObject.SetActive(false);
        }
    }

    public bool isDestroy()
    {
        return isDestory;
    }
}
