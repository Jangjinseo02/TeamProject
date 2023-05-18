using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    public GameObject player;

    public enum BreakType { Single, Multi, AirCore, ClearLevel }
    [Header("---------------------Score")]
    [SerializeField] public int score;
    [SerializeField] int airCount;

    [Header("---------------------GameObject")]
    [SerializeField] BlockMgr[] blockMgrs;
    [SerializeField] GameObject backGround;
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
    Level level = Level.Easy;
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
        switch (level)
        {
            case Level.Easy:
                clearDepth = depth[(int)Level.Easy];
                break;
            case Level.Nomal:
                clearDepth = depth[(int)Level.Nomal];
                break;
            case Level.Hard:
                clearDepth = depth[(int)Level.Hard];
                break;
        }

        stageLevel = 0;
        blockMgrPos = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBlockStage()
    {
        StageSetting();
        blockMgrPos.x = 0;
        blockMgrPos.y -= 100;
        blockMgrs[curLevel].gameObject.transform.position = blockMgrPos;
        blockMgrs[curLevel].gameObject.SetActive(true);

        backGround.gameObject.transform.position = blockMgrPos;
    }

    public void BlockStageClear()
    {
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
