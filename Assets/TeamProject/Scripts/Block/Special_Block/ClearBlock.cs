using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBlock : Block
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        boxCol.enabled = true;
    }


    public override void StageClear()
    {
        GameManager.Instance.BlockStageClear();
    }

    public override void OnDamaged(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            boxCol.enabled = false;

            gameObject.SetActive(false);
        }
    }
}
