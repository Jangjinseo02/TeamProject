using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierItem : Item, IItem
{
    public void Use(GameObject target)
    {
        if (isUse)
            return;

        PlayerMoveMent player = target.GetComponent<PlayerMoveMent>();

        if (player != null && !isUse)
        {
            UIManager.Instance.SettingItemUIImage(type);

            GameObject obj = ObjectManager.Instance.GetBlock((int)BlockMgr.Object.BarrierObj);
            obj.transform.parent = player.gameObject.transform;
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(true);

            //Ãß°¡
            if (group != null)
            {
                group.blockManager.RemovePos(this);
            }
            gameObject.SetActive(false);
        }
    }
}
