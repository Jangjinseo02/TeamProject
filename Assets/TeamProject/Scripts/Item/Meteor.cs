using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : Item, IItem
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

            int count = 0;
            int changeCount = 0;

            if (GameManager.Instance.level == GameManager.Level.Easy)
                changeCount = Random.Range(5, 10);
            else if (GameManager.Instance.level == GameManager.Level.Nomal)
                changeCount = Random.Range(15, 25);
            else if (GameManager.Instance.level == GameManager.Level.Hard)
                changeCount = Random.Range(20, 35);

            Block firstBlock = null;

            foreach (Block block in destroyBlock)
            {
                int ran = Random.Range(0, 2);

                if (!block.gameObject.activeInHierarchy || !(block.type >= (int)BlockMgr.BlocksType.BlockB && block.type <= (int)BlockMgr.BlocksType.BlockF) || ran == 1)
                    continue;

                if (changeCount == count)
                    break;
                else
                    count++;

                if (firstBlock == null)
                {
                    firstBlock = block;
                }

                block.group.blockManager.CheckBlock(5, block.row, block.col, (int)BlockMgr.BlocksType.MeteorBlcok);
                
                //재그룹화를 위한 그룹 삭제
                foreach (Block member in block.group)
                    member.RemoveGroup(block.group.blockManager);
            }

            //group.blockManager.AllGrouping();

            firstBlock.group.blockManager.AllGrouping();

            //추가
            if (group != null)
            {
                group.blockManager.RemovePos(this);
            }
            gameObject.SetActive(false);
        }
    }
}
