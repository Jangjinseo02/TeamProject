using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : Block, IItem
{
    public void Use(GameObject target)
    {
        PlayerMoveMent player = GetComponentInParent<PlayerMoveMent>();
        Block block = target.GetComponent<Block>();
        if (player == null || block == null)
            return;

        block.OnDamaged(Mathf.CeilToInt(block.health));

        int row = Mathf.RoundToInt(player.transform.localPosition.y);
        int col = block.col;

        //배리어 블록 매니저 오브젝트에 추가
        BlockMgr blockMgr = GameManager.Instance.stayBlockMgr;
        //Debug.Log(row.ToString() + " " + col.ToString());
        blockMgr.AddBlock(row, col, (int)BlockMgr.Object.BarrierObj, this.gameObject);

        //재그룹화
        foreach (Block member in block.group)
        {
            member.RemoveGroup(block.group.blockManager);
            member.group.CheckGroupUnbalance();
        }

        if (group != null)
            group.CheckGroupUnbalance();
        StartCoroutine(BarrierRoutine());
    }

    IEnumerator BarrierRoutine()
    {
        yield return new WaitForSeconds(5f);
        OnDamaged(Mathf.CeilToInt(health));
    }
}
