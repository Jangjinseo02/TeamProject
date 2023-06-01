using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    public GameObject player;
    public CameraMove followCamera;

    public enum BreakType { Single, Multi, AirCore, ClearLevel }
    [Header("---------------------Score")]
    [SerializeField] public int score;
    [SerializeField] int airCount;

    [Header("---------------------GameObject")]
    [SerializeField] BlockMgr[] blockMgrs;
    [SerializeField] GameObject backGround;
    public BlockMgr stayBlockMgr;
    Vector2 blockMgrPos;

    [Header("---------------------BackGround")]
    [SerializeField] SpriteRenderer backGroundObject;
    [SerializeField] Sprite[] backGroundSprites;
    public int ran = 0;
    int curRan = -1;

    BoxCollider2D boxCollider;
    public int stageLevel = 0;
    int curLevel = 0;

    //레벨 목표 깊이
    
    public enum Level { Easy, Nomal, Hard };
    public Level level = Level.Easy;
    [Header("---------------------Level")]
    [SerializeField] int[] depth;
    public int clearDepth = 0;


    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        if (DataManager.Instance != null)
            level = (GameManager.Level)DataManager.Instance.ReturnLevel();

        //목표 깊이 설정
        for (int i = 0; i < blockMgrs.Length; i++)
        {
            switch (level)
            {
                case Level.Easy:
                    //clearDepth = depth[(int)Level.Easy];
                    blockMgrs[i].numCol = 7;
                    break;
                case Level.Nomal:
                    //clearDepth = depth[(int)Level.Nomal];
                    blockMgrs[i].numCol = 9;
                    break;
                case Level.Hard:
                    //clearDepth = depth[(int)Level.Hard];
                    blockMgrs[i].numCol = 11;
                    break;
            }

            blockMgrs[i].StartSetting();
        }

        //벽 위치 조정
        BoxCollider2D boxCol = backGround.GetComponentInChildren<BoxCollider2D>();
        boxCol.offset = new Vector2(blockMgrs[0].numCol, boxCol.offset.y);
        
        //player 위치 및 Camera 위치 변경
        player.transform.position = new Vector2((blockMgrs[0].numCol - 1) / 2, player.transform.position.y);
        followCamera.StartSetting();

        stageLevel = 0;
        blockMgrPos = Vector2.zero;
    }

    public void SetBlockStage()
    {
        StageSetting();
        blockMgrPos.x = 0;
        blockMgrPos.y -= 100;
        blockMgrs[curLevel].gameObject.transform.position = blockMgrPos;
        blockMgrs[curLevel].gameObject.SetActive(true);
        stayBlockMgr = blockMgrs[curLevel];
        player.transform.parent = stayBlockMgr.transform;

        backGround.gameObject.transform.position = blockMgrPos;
    }

    public void BlockStageClear()
    {
        player.transform.parent = null;

        blockMgrs[curLevel].Disable();
        UIManager.Instance.SelectBackGroundPopup();

        GetScore(BreakType.ClearLevel); //stagelevel++ and addscore

        curLevel = stageLevel % 2;
    }

    public void StageSetting()
    {
        RanValue();

        backGroundObject.sprite = backGroundSprites[ran];
    }

    void RanValue()
    {
        if (stageLevel == 0) //첫 시작은 흙
            ran = 0;
        else
            ran = Random.Range(0, 4);

        if (ran == curRan)
            RanValue();
        else
            curRan = ran;
    }

    //플레이어가 죽음
    public void PlayerDead()
    {
        Time.timeScale = 0;
        //결과창으로 이동
        UIManager.Instance.ResultScreenPopup();
    }

    public void GameExit()
    {
        //게임 종료 시 실행되던 모든 block들의 코루틴 종료
        Block[] childs = null;
        for (int i = 0; i < blockMgrs.Length; i++)
        {
            if (blockMgrs[i].gameObject.activeInHierarchy)
                childs = blockMgrs[i].GetComponentsInChildren<Block>();
        }

        foreach (Block member in childs)
            if (member.gameObject.activeInHierarchy)
                member.StopAllCoroutines();
    }

    public void GetScore(BreakType breakType)
    {
        switch (breakType)
        {
            case BreakType.Single:
                score += 10;
                break;
            case BreakType.Multi:
                score += 30;
                break;
            case BreakType.AirCore:
                airCount += 1;
                score += airCount * 100;
                break;
            case BreakType.ClearLevel:
                stageLevel += 1;
                score += stageLevel * 1000;
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        { 
            SetBlockStage();
            boxCollider.offset = new Vector2(0 , blockMgrPos.y - 8);
        }
    }
}
