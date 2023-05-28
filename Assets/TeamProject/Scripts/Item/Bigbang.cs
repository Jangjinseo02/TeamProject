using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bigbang : Item, IItem
{
    public void Use(GameObject target)
    {
        if (isUse)
            return;

        PlayerMoveMent player = target.GetComponent<PlayerMoveMent>();
        
        if (player!= null && !isUse)
        {
            HashSet<Block> destroyBlock = new HashSet<Block>(player.followCamera.ReturnBlock());

            foreach (Block block in destroyBlock)
            {
                if (block.tag == "Item")
                    continue;
                block.OnDamaged(Mathf.CeilToInt(block.health));
            }

            //Ãß°¡
            if (group != null)
            {
                group.blockManager.RemovePos(this);
            }
            gameObject.SetActive(false);
        }
    }
}
