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
        Debug.Log("Hit");

        if (health <= 0)
        {
            //type = -1;
            Debug.Log("OnDamaged");
            isDestory = true;
            group.blockManager.RemovePos(this);
            gameObject.SetActive(false);
        }
    }

    public bool isDestroy()
    {
        return isDestory;
    }
}
