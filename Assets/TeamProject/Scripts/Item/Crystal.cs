using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : Item, IItem
{
    public void Use(GameObject target)
    {
        if (isUse)
            return;

        PlayerMoveMent player = target.GetComponent<PlayerMoveMent>();

        if (player != null && !isUse)
        {
            UIManager.Instance.SettingItemUIImage(type);
            
            HashSet<Block> destroyBlock = new HashSet<Block>(player.followCamera.ReturnBlock());
            int ran = 1; //Random.Range((int)BlockMgr.BlocksType.BlockB, (int)BlockMgr.BlocksType.BlockF + 1);
            Block firstBlock = null;

            foreach (Block block in destroyBlock)
            {
                if (firstBlock == null)
                {
                    firstBlock = block;
                    ran = firstBlock.type;
                }

                if (block.type != ran || !block.gameObject.activeInHierarchy)
                    continue;

                block.group.blockManager.CheckBlock(5, block.row, block.col, (int)BlockMgr.BlocksType.GlassBlock);

                foreach (Block member in block.group)
                    member.RemoveGroup(member.group.blockManager);
            }

            firstBlock.group.blockManager.AllGrouping();

            //Ãß°¡
            if (group != null)
            {
                group.blockManager.RemovePos(this);
            }
            gameObject.SetActive(false);
        }
    }
}
