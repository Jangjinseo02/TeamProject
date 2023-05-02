using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMgr : MonoBehaviour
{
    enum BlocksType { NormalBlock, ClearBlock, BlockB, BlockC, BlockD, BlockF, SpeBlock1, SpeBlock2 }
    enum MonsterType { MonsterA = 8, MonsterB, MonsterC, MonsterD }

    GameObject[,] blocks;
    int[,] types;

    [SerializeField] GameObject blockPool;
    [SerializeField] private int numRow;
    [SerializeField] private int numCol;

    List<Block> unbalanceBlockList = new List<Block>();
    HashSet<Group> balanceBlockList = new HashSet<Group>();
    HashSet<Block> regroupBlockList = new HashSet<Block>();
    List<Group> balanceGroupList = new List<Group>();

    public void Search(Block block)
    {
        List<Block> block_s = new List<Block>();
        Queue<Block> queue = new Queue<Block>();

        queue.Enqueue(block);

        while (queue.Count > 0)
        {
            Block newBlock = queue.Dequeue();

            if (block.type != newBlock.type || newBlock.isCheck)
                continue;
            block_s.Add(newBlock);
            newBlock.isCheck = true;

            int row = newBlock.row;
            int col = newBlock.col;

            //윗 방향 체크
            if (row + 1 < numRow)
            {
                GameObject upBlock = BlockCheck(row + 1, col);
                if(upBlock != null)
                    queue.Enqueue(upBlock.GetComponent<Block>());
            }
            
            //아래 방향 체크
            if (row - 1 >= 0)
            {
                GameObject downBlock = BlockCheck(row - 1, col);
                if (downBlock != null)
                    queue.Enqueue(downBlock.GetComponent<Block>());
            }
            
            //오른 방향 체크
            if (col + 1 < numCol)
            {
                GameObject rightBlock = BlockCheck(row, col + 1);
                if(rightBlock != null)
                    queue.Enqueue(blocks[row, col + 1].GetComponent<Block>());
            }
                
            //왼 방향 체크
            if (col - 1 >= 0)
            {
                GameObject leftBlock = BlockCheck(row, col - 1);
                if(leftBlock != null)
                    queue.Enqueue(blocks[row, col - 1].GetComponent<Block>());
            }
        }

        Block firstMember = null;

        foreach (Block checkBlock in block_s)
        {
            if (firstMember == null)
                firstMember = checkBlock;

            checkBlock.isCheck = false;
            if (!firstMember.group.AddGroup(checkBlock))
                continue;
        }
    }

    private void OnEnable()
    {
        blocks = new GameObject[numRow, numCol];
        types = new int[numRow, numCol];
        //슬라임 스폰
        int SlimCount = 8;
        int afslimRow = 0;
        bool spawnSlim = false;
        Vector2[] spawnPoint = new Vector2[8];

        //몬스터 스폰
        int MonsterCount = 3;
        int afMonsterRoW = 0;
        bool spawnMonster = false;

        int spawnRand = 0;

        for (int i = 0; i < numRow; i++)
        {
            afslimRow += 1;
            afMonsterRoW += 1;

            spawnRand = Random.Range(1, numCol - 1);

            for (int j = 0; j < numCol; j++)
            {
                spawnSlim = spawnRand == j ? true : false;
                spawnMonster = spawnRand == j ? true : false;

                if (i < 6) //클리어 블록 스폰
                {
                    blocks[i, j] = ObjectManager.Instance.GetBlock((int)BlocksType.ClearBlock);
                }
                else if (i > 94 && i < numRow) //기본 블록 스폰
                    blocks[i, j] = ObjectManager.Instance.GetBlock((int)BlocksType.NormalBlock);
                else if (SlimCount > 0 && afslimRow >= 10 && spawnSlim)
                {
                    //슬라임 몬스터 스폰
                    //MosterA == 슬라임.  
                    blocks[i, j] = ObjectManager.Instance.GetBlock((int)MonsterType.MonsterA);
                    SlimCount -= 1;
                    spawnPoint[SlimCount] = new Vector2(j, i);
                    afslimRow = 0;
                }
                else if (MonsterCount > 0 && afMonsterRoW >= 29 && spawnMonster)
                {
                    //몬스터 스폰
                    int ran = Random.Range((int)MonsterType.MonsterB, (int)(MonsterType.MonsterD + 1));
                    blocks[i, j] = ObjectManager.Instance.GetBlock(ran);
                    MonsterCount -= 1;
                    afMonsterRoW = 0;
                }
                else
                {
                    //나머지 공간은 random 블록 스폰
                    int ran = Random.Range((int)BlocksType.BlockB, (int)(BlocksType.BlockF + 1));
                    blocks[i, j] = ObjectManager.Instance.GetBlock(ran);
                }

                blocks[i, j].transform.localPosition = new Vector3(j, i, 0);
                Block block = blocks[i, j].GetComponent<Block>();
                types[i, j] = block.type;
                block.group = new Group(this);
                block.Setting(i, j);
            }
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < numRow; i++)
        {
            for (int j = 0; j < numCol; j++)
            {
                //blocks[i, j].gameObject.transform.parent = blockPool.transform;

                //ObjectManager.Instance.ReturnBlock(blocks[i, j]);
                blocks[i, j] = null;
                types[i, j] = 0;
            }
        }


    }

    private void Start()
    {
        for (int i = 0; i < numRow; i++)
        {
            for (int j = 0; j < numCol; j++)
            {
                blocks[i, j].gameObject.transform.parent = this.transform;
                Search(blocks[i, j].GetComponent<Block>());
            }
        }
    }
    //그룹에 대해서는 충돌 체크 안하고
    //그룹 중 하나라도 멈추면 그룹 내 블록들을 모두 갱신한다.

    void Update()
    {
        HashSet<Block> unbalanceBlocks = new HashSet<Block>(unbalanceBlockList);
        
        //찾은 unabalance 그룹의 블록의 shake, drop, collision 처리
        foreach (Block member in unbalanceBlocks)
        {
            Group group = member.group;
            if (!group.unbalance) continue;

            if (!member.shaking && !member.dropping)
                member.ShakeStart();
            else if (member.dropping)
            {
                member.DropNext();

                Collision(member);

            }
        }

        //충돌되어 멈춘 블록들에 대해서 처리
        foreach (Group group in balanceBlockList)
        {
            foreach (Block block in group)
            {
                UpdatePos(block);
                block.DropEnd();
                block.ShakeEnd();
                regroupBlockList.Add(block);

                if (unbalanceBlockList.Contains(block))
                {
                    unbalanceBlockList.Remove(block);
                    block.group.unbalance = false;
                }
            }
        }

        foreach(Block block in regroupBlockList)
        {
            Search(block);
        }


        //reGroup이 모두 된 블록들에 대해서 blink처리
        foreach (Block block in regroupBlockList)
        {
            if (!balanceGroupList.Contains(block.group))
            {
                balanceGroupList.Add(block.group);
            }
        }
        //blink처리할 그룹들의 블록이 3개 초과라면 blink시작.
        for (int i = 0; i < balanceGroupList.Count; i++)
        {
            foreach (Block block in balanceGroupList[i])
            {
                if (block.group.Count > 3)
                {
                    block.BlinkStart();
                }
            }
        }

        regroupBlockList.Clear();
        balanceBlockList.Clear();
        balanceGroupList.Clear();

    }

    public void Collision(Block block)
    {
        //int row = Mathf.CeilToInt(block.transform.localPosition.y);

        GameObject rightblockObj = block.col < numCol - 1 ? BlockCheck(block.row, block.col + 1) : null;
        GameObject leftblockObj = block.col != 0 ? BlockCheck(block.row, block.col - 1) : null;
        GameObject underblockObj = block.row != 0 ? BlockCheck(block.row - 1, block.col) : null;

        Block rightBlock = rightblockObj != null ? rightblockObj.GetComponent<Block>() : null;
        Block leftBlock = leftblockObj != null ? leftblockObj.GetComponent<Block>() : null;
        Block underBlock = underblockObj != null ? underblockObj.GetComponent<Block>() : null;

        //아래 블록이 체크되지 않음
        if (underBlock || (rightBlock != null && block.group != rightBlock.group && block.type == rightBlock.type)
                                                        || (leftBlock != null && block.group != leftBlock.group && block.type == leftBlock.type))
        {
            block.DropEnd();            
            balanceBlockList.Add(block.group);
        }

        //block.ResetMove();
    }


    public void UpdatePos(Block block)
    {
        block.transform.localPosition = new Vector3(block.col, block.row, 0);
        types[block.row, block.col] = block.type; 
        blocks[block.row, block.col] = block.gameObject;
    }

    public void RemovePos(Block block)
    {
        types[block.row, block.col] = -1;
        blocks[block.row, block.col] = null;

        Block upBlock = UpBlock(block.row, block.col);
        if (upBlock != null)
            upBlock.group.GroupUnbalance();
    }


    public Block UnderBlock(int row, int col)
    {
        if (row == 0 || types[row - 1, col] == -1)
            return null;

        GameObject blockObj = BlockCheck(row - 1, col);

        return blockObj != null ? blockObj.GetComponent<Block>() : null;
    }

    public Block UpBlock(int row, int col)
    {
        if (row == numRow - 1 || types[row + 1, col] == -1)
            return null;
        
        GameObject blockObj = BlockCheck(row +1, col);

        return blockObj != null ? blockObj.GetComponent<Block>() : null;
    }

    GameObject BlockCheck(int row, int col)
    {
        if (blocks[row, col] == null)
            return null;

        return blocks[row, col];
    }

    public void CheckUnbalanceList(Group group)
    {
        foreach(Block member in group)
        {
            if (!unbalanceBlockList.Contains(member))
            {
                unbalanceBlockList.Add(member);
                //member.ShakeStart(); //여기서 shake를 시작 시키고 shakeNext메서드를 이용해서 매 프레임 동일한 shake를 만들도록 만든다. 또한 그룹으로 편입된 블록들에 대해서도 더 편하게
                //움직임을 만들 수 있다.
            }
               
        }

        unbalanceBlockList.Sort(delegate (Block a, Block b)
        {
            if (a.row == b.row)
                if (a.col == b.col)
                {
                    return 0;
                }
                else
                {
                    return (a.col < b.col ? -1 : 1);
                }
            else
                return (a.row < b.row ? -1 : 1);
        });
    }

    public void Disable()
    {
        for (int i = 0; i < numRow; i++)
        {
            for (int j = 0; j < numCol; j++)
            {
                blocks[i, j].gameObject.transform.parent = blockPool.transform;
                //blocks[i,j]에 없는 값이 있음, 반환할 리스트를 하나 만들어야 할 듯?
                ObjectManager.Instance.ReturnBlock(blocks[i, j]);
                blocks[i, j] = null;
                types[i, j] = 0;
            }
        }
    }

    //unbalance 체크
    //for (int i = 0; i < numRow; i++)
    //{
    //    for (int j = 0; j < numCol; j++)
    //    {
    //        GameObject blockObj = BlockCheck(i, j);

    //        if (i != numRow - 1 && blockObj == null)
    //        {
    //            Block upBlock = UpBlock(i, j);
    //            if (upBlock == null || upBlock.group.unbalance)
    //                continue;

    //            upBlock.group.GroupUnbalance();
    //        }
    //    }
    //}

    //unbalance 블록 그룹들 찾기
    //for (int i = 0; i < numRow; i++)
    //{
    //    for (int j = 0; j < numCol; j++)
    //    {
    //        GameObject blockObj = BlockCheck(i, j);
    //        Block block = blockObj != null ? blockObj.GetComponent<Block>() : null;
    //        if (block != null && block.group.unbalance)
    //        {
    //            if (!unbalanceBlockList.Contains(block))
    //            {
    //                unbalanceBlockList.Add(block);

    //                unbalanceBlockList.Sort(delegate (Block a, Block b)
    //                {
    //                    if (a.row == b.row)
    //                        return 0;
    //                    else
    //                        return (a.row < b.row ? -1 : 1);
    //                });
    //            }
    //        }
    //    }
    //}
}
