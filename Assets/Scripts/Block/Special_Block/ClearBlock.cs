using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBlock : Block
{

    public override void StageClear()
    {
        GameManager.Instance.BlockStageClear();
    }

    public override void OnDamaged(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
