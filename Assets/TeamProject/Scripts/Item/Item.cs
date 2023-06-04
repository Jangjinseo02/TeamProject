using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Block
{
    public bool isUse = false;

    public override void OnEnable()
    {
        if (group != null)
            this.group.CheckGroupUnbalance();

        isUse = false;
    }

    public void UsingItem()
    {
        if (!gameObject.activeInHierarchy)
            return;
        isUse = true;
    }
}
