using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public bool giveUp = false;

    BoxCollider2D boxCollider;
    public int stageLevel = 0;
    int curLevel = 0;

    public enum Level { Easy, Nomal, Hard };
    public Level level = Level.Easy;

    [Header("---------------------Level")]
    //레벨 목표 마리수
    [SerializeField] GameObject clearObject;
    public int curCatch = 0;
    public int clearCatch = 0;

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
                    blockMgrs[i].numCol = 7;
                    clearCatch = 10;
                    break;
                case Level.Nomal:
                    blockMgrs[i].numCol = 9;
                    clearCatch = 25;
                    break;
                case Level.Hard:
                    blockMgrs[i].numCol = 11;
                    clearCatch = 50;
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
        //UIManager.Instance.SelectBackGroundPopup();

        GetScore(BreakType.ClearLevel); //stagelevel++ and addscore

        curLevel = stageLevel % 2;

        if (clearCatch <= curCatch)
        {
            clearObject.transform.position = new Vector2(0, player.transform.position.y + Vector2.down.y);
            clearObject.SetActive(true);

            //GameExit 사용하면 될듯
            //플레이어 내부에 클리어에 관한 플레이어 행동이 정의되어야함
            //코루틴으로
            //플레이어 animation 출력
            //결과창 출력
        }
        else
        {
            UIManager.Instance.Fade();
        }
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

    //플레이어 죽음 판별, 이후 행동 실행
    void PlayerDead()
    {
        if (!player.GetComponent<PlayerMoveMent>().isDead)
            player.GetComponent<PlayerMoveMent>().OnDamaged(1000, false);

        //결과창으로 이동
        if(!giveUp)
            StartCoroutine(PopRoutine());
        else
            StartCoroutine(GiveUpRoutine());
    }

    IEnumerator PopRoutine()
    {
        //player anim 출력
        yield return new WaitForSeconds(3f);
        UIManager.Instance.ResultScreenPopup();
        Time.timeScale = 0;
    }

    IEnumerator GiveUpRoutine()
    {
        //player anim 출력
        yield return new WaitForSecondsRealtime(3f);
        SceneManager.LoadScene("TitleScene");
        Time.timeScale = 1f;
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

        //목표 완수시 

        //목표 완수 실패시
        PlayerDead();
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
