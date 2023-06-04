using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMgr : MonoBehaviour
{
    public enum BlocksType { ClearBlock, BlockB, BlockC, BlockD, BlockF, HardBlock, MeteorBlcok, GlassBlock }
    public enum MonsterType { Slim = 8, MonsterB, MonsterC }

    public enum Object { BarrierObj = 13 }

    GameObject[,] blocks;
    int[,] types;

    [SerializeField] GameObject blockPool;
    public int numRow;
    public int numCol;

    [SerializeField] float allPers;
    [SerializeField] float glassPers;
    [SerializeField] float meteorPers;
    [SerializeField] float hardPers;

    List<Block> unbalanceBlockList = new List<Block>();
    HashSet<Group> balanceBlockList = new HashSet<Group>();

    float[] earlyValue = { 50, 30, 15, 5, 0 };
    float[] bfValue = { 50, 30, 15, 5, 0 };
    float[] curValue = { 50, 30, 15, 5, 0 };

    BoxCollider2D breathRoom;

    private void Awake()
    {
        breathRoom = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        breathRoom.offset = new Vector2(GameManager.Instance.player.transform.position.x, 100); //박스 콜라이더 위치 이동
    }

    private void OnEnable()
    {
        BlockCreate();
        HardBlockSetting();
        AllGrouping();
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
            Block firstmember = null;
            foreach (Block block in group)
            {
                if (firstmember == null)
                    firstmember = block;

                UpdatePos(block);
                block.DropEnd();
                block.ShakeEnd();

                if (unbalanceBlockList.Contains(block))
                {
                    unbalanceBlockList.Remove(block);
                    block.group.unbalance = false;
                }
            }

            if (firstmember != null)
                Search(firstmember);

            if (firstmember.group.Count > 3)
                foreach (Block member in firstmember.group)
                    member.BlinkStart();
            else
            {
                firstmember.group.CheckGroupUnbalance();

                if (firstmember.group.unbalance)
                    SetCurTime(firstmember);
            }

        }
        balanceBlockList.Clear();
    }

    void SetCurTime(Block first)
    {
        float setCurTime = 0;

        foreach (Block member in first.group)
        {
            if (setCurTime < member.curTime)
                setCurTime = member.curTime;
        }

        foreach (Block member in first.group)
        {
            member.ShakeEnd();
            member.curTime = setCurTime;
        }
    }

    public void StartSetting()
    {
        blocks = new GameObject[numRow, numCol];
        types = new int[numRow, numCol];
    }

    void BlockCreate()
    {
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
                if (blocks[i, j] != null)
                    continue;

                spawnSlim = spawnRand == j ? true : false;
                spawnMonster = spawnRand == j ? true : false;

                if (i < 6) //클리어 블록 스폰
                {
                    blocks[i, j] = ObjectManager.Instance.GetBlock((int)BlocksType.ClearBlock);
                }
                else if (!(i > 94 && i < numRow) && SlimCount > 0 && afslimRow >= 10 && spawnSlim)
                {
                    //슬라임 몬스터 스폰
                    //MosterA == 슬라임.  
                    blocks[i, j] = ObjectManager.Instance.GetBlock((int)MonsterType.Slim);
                    SlimCount -= 1;
                    spawnPoint[SlimCount] = new Vector2(j, i);
                    afslimRow = 0;
                }
                else if (!(i > 94 && i < numRow) && MonsterCount > 0 && afMonsterRoW >= 29 && spawnMonster)
                {
                    //몬스터 스폰
                    int ran = Random.Range((int)MonsterType.MonsterB, (int)(MonsterType.MonsterC + 1));
                    blocks[i, j] = ObjectManager.Instance.GetBlock(ran);
                    MonsterCount -= 1;
                    afMonsterRoW = 0;
                }
                else
                {
                    //나머지 공간은 random 기본 블록 스폰, 특수 블록 스폰
                    float blockRan = Random.Range(0f, allPers);

                    if (i < 95 && blockRan <= glassPers) //0.5f
                    {
                        blocks[i, j] = ObjectManager.Instance.GetBlock((int)BlocksType.GlassBlock);
                    }
                    else if (i < 95 && blockRan <= glassPers + meteorPers) //1.5f
                    {
                        blocks[i, j] = ObjectManager.Instance.GetBlock((int)BlocksType.MeteorBlcok);
                    }
                    else if(i < 95 && blockRan < glassPers + meteorPers + hardPers) //4f
                    {
                        blocks[i, j] = ObjectManager.Instance.GetBlock((int)BlocksType.HardBlock);
                    }
                    else
                    {
                        int ran = Random.Range((int)BlocksType.BlockB, (int)(BlocksType.BlockF + 1));
                        blocks[i, j] = ObjectManager.Instance.GetBlock(ran);
                    }
                }

                
                SetBlock(i, j);

                //blocks[i, j].transform.parent = this.transform;
                //blocks[i, j].transform.localPosition = new Vector3(j, i, 0);
                //Block block = blocks[i, j].GetComponent<Block>();
                //types[i, j] = block.type;
                //block.group = new Group(this);
                //block.Setting(i, j);
                
            }
        }
    }

    void HardBlockSetting()
    {
        int level = GameManager.Instance.stageLevel + 1;

        curValue[0] = earlyValue[0] - ((level - 1) * 7.5f);
        curValue[1] = level < 4 ? earlyValue[0] + earlyValue[1] - (level - 1) * 7f - curValue[0] : bfValue[1] * 0.8f;
        curValue[2] = level < 8 ? earlyValue[0] + earlyValue[1] + earlyValue[2] - (level - 1) * 5f - (curValue[0] + curValue[1]) : bfValue[2] * 0.85f;
        curValue[3] = level < 12 ? earlyValue[0] + earlyValue[1] + earlyValue[2] + earlyValue[3] - (level - 1) * 4f - (curValue[0] + curValue[1] + curValue[2]) : bfValue[3] * 0.9f;
        curValue[4] = 100 - curValue[0] - curValue[1] - curValue[2] - curValue[3];

        Debug.Log(curValue[0].ToString() + " " + curValue[1].ToString() + " " + curValue[2].ToString() + " " + curValue[3].ToString() + " " + curValue[4].ToString());

        for (int i = 0; i < numRow; i++)
        {
            for (int j = 0; j < numCol; j++)
            {
                if (blocks[i, j].GetComponent<Block>().type == (int)MonsterType.Slim)
                {
                    float ran = Random.Range(0, 101);
                    int type = (int)BlocksType.HardBlock;

                    if (ran <= curValue[0])
                    {
                        //0개
                        Debug.Log(i.ToString() + " , " + j.ToString() + ": 0개 " + curValue[0].ToString());
                    }
                    else if (ran > curValue[0] && ran <= curValue[0] + curValue[1])
                    {
                        //한개
                        Debug.Log(i.ToString() + " , " + j.ToString() + ": 1개 " + curValue[1].ToString());
                        int ranDir = Random.Range(0, 4);
                        CheckBlock(ranDir, i, j, type);
                    }
                    else if (ran > curValue[0] + curValue[1] && ran <= curValue[0] + curValue[1] + curValue[2])
                    {
                        //두개
                        Debug.Log(i.ToString() + " , " + j.ToString() + ": 2개 " + curValue[2].ToString());
                        for(int k = 0; k < 2; k++)
                        {
                            int ranDir = Random.Range(0, 4);
                            CheckBlock(ranDir, i, j, type);
                        }
                    }
                    else if (ran > curValue[0] + curValue[1] + curValue[2] && ran <= curValue[0] + curValue[1] + curValue[2] + curValue[3])
                    {
                        //세개
                        Debug.Log(i.ToString() + " , " + j.ToString() + ": 3개 " + curValue[3].ToString());
                        for (int k = 0; k < 3; k++)
                        {
                            int ranDir = Random.Range(0, 4);
                            CheckBlock(ranDir, i, j, type);
                        }
                    }
                    else if (ran > curValue[0] + curValue[1] + curValue[2] + curValue[3] && ran <= curValue[0] + curValue[1] + curValue[2] + curValue[3] + curValue[4])
                    {
                        //4개
                        Debug.Log(i.ToString() + " , " + j.ToString() + ": 4개 " + curValue[4].ToString());
                        for (int k = 0; k < 4; k++)
                        {
                            int ranDir = Random.Range(0, 4);
                            CheckBlock(ranDir, i, j, type);
                        }
                    }
                }
            }
        }
    }

    //방향 체크
    public void CheckBlock(int ranDir, int i, int j, int type)
    {
        if (ranDir == 0)
            ChangeBlock(BlockCheck(i - 1, j), i - 1, j, type);
        else if (ranDir == 1)
            ChangeBlock(BlockCheck(i + 1, j), i + 1, j, type);
        else if (ranDir == 2)
            ChangeBlock(BlockCheck(i, j - 1), i, j - 1, type);
        else if (ranDir == 3)
            ChangeBlock(BlockCheck(i, j + 1), i, j + 1, type);
        else
            ChangeBlock(BlockCheck(i, j), i, j, type);
    }

    //교체
    void ChangeBlock(GameObject block, int row, int col, int type)
    {
        if (block == null)
            return;

        block.gameObject.transform.parent = blockPool.transform;
        ObjectManager.Instance.ReturnBlock(block);
        blocks[row, col] = ObjectManager.Instance.GetBlock(type);
        SetBlock(row, col);
    }

    //교체가 아닌 추가
    public void AddBlock(int row, int col, int type, GameObject obj)
    {
        if (BlockCheck(row, col) == null)
        {
            if (obj == null)
                blocks[row, col] = ObjectManager.Instance.GetBlock(type);
            else
                blocks[row, col] = obj;

            SetBlock(row, col);

            Search(blocks[row, col].GetComponent<Block>());
            blocks[row, col].SetActive(true);
        }
    }

    public void AllGrouping()
    {
        for (int i = 0; i < numRow; i++)
        {
            for (int j = 0; j < numCol; j++)
            {
                if (blocks[i, j] == null)
                    continue;
                Search(blocks[i, j].GetComponent<Block>());
                blocks[i, j].SetActive(true); //나중에 가장 마지막에 true로 바꾸기
            }
        }
    }

    public void Search(Block block)
    {
       
        List<Block> block_s = new List<Block>();
        Queue<Block> queue = new Queue<Block>();

        //운석 블록의 경우 그룹 내에 혼자 존재
        if (block.type == (int)BlocksType.MeteorBlcok)
        {
            block.GetComponent<MeteorBlock>().OnlyOne();
            return;
        }

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

            //if (block.type == (int)BlocksType.SpeBlock1)
            //    continue;

            //윗 방향 체크
            if (row + 1 < numRow)
            {
                GameObject upBlock = BlockCheck(row + 1, col);
                if (upBlock != null)
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
                if (rightBlock != null)
                    queue.Enqueue(blocks[row, col + 1].GetComponent<Block>());
            }

            //왼 방향 체크
            if (col - 1 >= 0)
            {
                GameObject leftBlock = BlockCheck(row, col - 1);
                if (leftBlock != null)
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

    public void Collision(Block block)
    {
        //int row = Mathf.CeilToInt(block.transform.localPosition.y);

        GameObject rightblockObj = block.col < numCol - 1 ? BlockCheck(block.row, block.col + 1) : null;
        GameObject leftblockObj = block.col != 0 ? BlockCheck(block.row, block.col - 1) : null;
        GameObject underblockObj = block.row != 0 ? BlockCheck(block.row - 1, block.col) : null;

        Block rightBlock = rightblockObj != null ? rightblockObj.GetComponent<Block>() : null;
        Block leftBlock = leftblockObj != null ? leftblockObj.GetComponent<Block>() : null;
        Block underBlock = underblockObj != null ? underblockObj.GetComponent<Block>() : null;

        //운석 블록 양옆 충돌 금지
        if (leftBlock != null && leftBlock.GetComponent<MeteorBlock>())
            leftBlock = null;
        if (rightBlock != null && rightBlock.GetComponent<MeteorBlock>())
            rightBlock = null;

        //아래 블록이 체크되지 않음
        if (underBlock /*&& !underBlock.group.unbalance*/ || (rightBlock != null && block.group != rightBlock.group && block.type == rightBlock.type /*&& !rightBlock.group.unbalance*/)
                                                        || (leftBlock != null && block.group != leftBlock.group && block.type == leftBlock.type /*&& !leftBlock.group.unbalance*/))
        {
            block.DropEnd();
            balanceBlockList.Add(block.group);
        }
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
            upBlock.group.CheckGroupUnbalance();
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

        GameObject blockObj = BlockCheck(row + 1, col);

        return blockObj != null ? blockObj.GetComponent<Block>() : null;
    }

    GameObject BlockCheck(int row, int col)
    {
        if (row >= numRow || row < 0 || col < 0 || col >= numCol)
            return null;
        if (blocks[row, col] == null)
            return null;

        return blocks[row, col];
    }

    public void CheckUnbalanceList(HashSet<Group> group)
    {
        foreach (Group memberGroup in group)
        {
            foreach (Block member in memberGroup)
            {
                if (!unbalanceBlockList.Contains(member))
                {
                    unbalanceBlockList.Add(member);
                    //member.ShakeStart(); //여기서 shake를 시작 시키고 shakeNext메서드를 이용해서 매 프레임 동일한 shake를 만들도록 만든다. 또한 그룹으로 편입된 블록들에 대해서도 더 편하게
                    //움직임을 만들 수 있다.
                }
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
        while (transform.childCount > 0)
        {
            GameObject child = transform.GetChild(0).gameObject;

            ObjectManager.Instance.ReturnBlock(child);
        }

        for (int i = 0; i < numRow; i++)
        {
            for (int j = 0; j < numCol; j++)
            {
                blocks[i, j] = null;
                types[i, j] = -1;
            }
        }

        unbalanceBlockList.Clear();
        balanceBlockList.Clear();

        gameObject.SetActive(false);

    }
    public GameObject SettingBlockList(int blockType, int row, int col)
    {
        blocks[row, col] = ObjectManager.Instance.GetBlock(blockType);
        SetBlock(row, col);
        Search(blocks[row, col].GetComponent<Block>());
        blocks[row, col].SetActive(true);

        return blocks[row, col];
    }

    void SetBlock(int row, int col)
    {
        blocks[row, col].transform.parent = this.transform;
        blocks[row, col].transform.localPosition = new Vector3(col, row, 0);
        Block newblock = blocks[row, col].GetComponent<Block>();
        types[row, col] = newblock.type;
        newblock.group = new Group(this);
        newblock.Setting(row, col);
    }

    void ReturnBlock(int row, int col)
    {
        blocks[row, col].SetActive(false);
        blocks[row, col].gameObject.transform.parent = blockPool.transform;
        //blocks[i,j]에 없는 값이 있음, 반환할 리스트를 하나 만들어야 할 듯?
        ObjectManager.Instance.ReturnBlock(blocks[row, col]);
        blocks[row, col] = null;
        types[row, col] = -1;
    }
}
