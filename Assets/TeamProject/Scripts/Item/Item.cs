using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Block
{
    public bool isUse = false;

    public override void OnEnable()
    {
        if (group != null)
            group.CheckGroupUnbalance(0.2f);
        isUse = false;
    }

    public void UsingItem()
    {
        if (!gameObject.activeInHierarchy)
            return;
        isUse = true;
    }
}
