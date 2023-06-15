using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : Block, IItem
{
    public bool setting = false;
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

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
        blockMgr.AddBlock(row, col, (int)BlockMgr.Object.BarrierObj, this.gameObject);

        StartCoroutine(ReGroupBlocks(block));

        StartCoroutine(BarrierRoutine(block));
    }

    IEnumerator ReGroupBlocks(Block block)
    {
        while (true)
        {
            foreach (Block member in block.group)
            {
                if (member.dropping)
                    continue;
                else
                {
                    if (block == member)
                        continue;
                    foreach (Block members in block.group)
                    {
                        if (block == members)
                            continue;
                        members.RemoveGroup(block.group.blockManager);
                    }
                    yield break;
                }                
            }

            yield return null;
        }
    }

    IEnumerator BarrierRoutine(Block target)
    {
        yield return new WaitForSeconds(5f);
        OnDamaged(Mathf.CeilToInt(health));
    }
}
