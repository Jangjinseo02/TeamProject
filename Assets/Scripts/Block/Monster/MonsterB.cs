using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterB : Monster
{
    BlockMgr blockmgr;

    Animator anim;

    IEnumerator checkTarget;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (group != null)
            blockmgr = group.blockManager;
    }

    public override void CameraOut()
    {
        base.CameraOut();

        if (checkTarget != null)
        {
            StopCoroutine(checkTarget);
            checkTarget = null;
        }
    }

    public override void CheckTarget(HashSet<Block> blocks)
    {
        base.CheckTarget(blocks);

        if (checkTarget == null)
        {
            checkTarget = CheckTargetPlayer(blocks);
            StartCoroutine(checkTarget);
        }
    }
    IEnumerator CheckTargetPlayer(HashSet<Block> blocks)
    {
        anim.SetTrigger("CheckTarget");
        yield return null;
        AttackReady(blocks);
    }

    void AttackReady(HashSet<Block> blocks)
    {
        StartCoroutine(AttackReadyRoutine(blocks));
    }

    IEnumerator AttackReadyRoutine(HashSet<Block> blocks)
    {
        if (inCamera && !isDead)
        {
            yield return new WaitForSeconds(4f);

            //ATTACK ANIM AND ATTACK
            anim.SetTrigger("Attack");
            Attack(blocks);
        }
    }

    public void Attack(HashSet<Block> blocks)
    {
        //base.Attack();
        if (isDead || !inCamera)
            return;
        Debug.Log("몬스터 공격");

        StartCoroutine(AttackRoutine(blocks));
    }

    IEnumerator AttackRoutine(HashSet<Block> blocks)
    {
        if (inCamera && !isDead)
        {
            yield return new WaitForSeconds(0.5f);
            //스킬 사용
            ActiveAttack(blocks);
            yield return new WaitForSeconds(2f);

            //다시 스킬 사용 준비
            if (checkTarget != null)
                CameraOut();
        }
    }

    public void ActiveAttack(HashSet<Block> blocks)
    {
        //base.ActiveAttack(blocks);

        HashSet<Block> checkList = new HashSet<Block>(blocks);
        Debug.Log("active attack");

        int count = 0;
        int pickCount = Random.Range(5, 9);

        foreach (Block block in checkList)
        {
            if (!block.gameObject.activeInHierarchy)
                continue;

            if (block.tag == "Monster" || block.tag == "Glass")
                continue;

            int ran = Random.Range(0, 3);

            if (ran <= 1 && count <= pickCount)
            {
                if (!block.dropping || !block.blinking || !block.shaking)
                {
                    blockmgr.CheckBlock(5, block.row, block.col, (int)BlockMgr.BlocksType.GlassBlock);

                    foreach (Block member in block.group)
                        member.RemoveGroup(blockmgr);
                    count++;
                }
            }
        }


        blockmgr.AllGrouping();
    }

    public override void OnDamaged(int damage)
    {
        base.OnDamaged(damage);

        if (health <= 0)
        {
            StartCoroutine(DeadRoutine());
        }
    }

    IEnumerator DeadRoutine()
    {
        anim.SetTrigger("Dead");
        yield return new WaitForSeconds(1f);

        GameObject item = group.blockManager.SettingBlockList(11, this.row, this.col);
        item.GetComponent<UseItem>().SpriteSetting((int)monsterType);
        gameObject.SetActive(false);
    }
}
