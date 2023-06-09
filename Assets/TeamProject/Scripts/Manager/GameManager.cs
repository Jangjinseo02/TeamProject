using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingleTon<GameManager>
{
    public GameObject player;
    public CameraMove followCamera;

    public enum BreakType { Single, Multi, AirCore, ClearLevel, Monster }
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

    public bool clear = false;

    [Header("---------------------Level")]
    //레벨 목표 마리수
    [SerializeField] GameObject clearObject;
    public int curCatch = 0;
    public int clearCatch = 0;

    public int resultDepth = 0;

    public IEnumerator pick;
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
                    player.GetComponent<PlayerMoveMent>().curOxygen = 0.5f;
                    blockMgrs[i].numCol = 7;
                    clearCatch = 10;
                    break;
                case Level.Nomal:
                    player.GetComponent<PlayerMoveMent>().curOxygen = 0.55f;
                    blockMgrs[i].numCol = 9;
                    clearCatch = 25;
                    break;
                case Level.Hard:
                    player.GetComponent<PlayerMoveMent>().curOxygen = 1f;
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
            clear = true;
            clearObject.transform.position = new Vector2(0, player.transform.position.y + Vector2.down.y);
            clearObject.SetActive(true);

            GameExit();
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
        if (stageLevel == 0) //첫 시작은 외계인 기지
            ran = 0;
        else
            ran = Random.Range(0, 5);

        if (ran == curRan)
            RanValue();
        else
            curRan = ran;
    }

    public void PickUp(GameObject target)
    {
        GameObject tongs = followCamera.transform.GetChild(0).gameObject;

        pick = PickUpRoutine(tongs, target);
        StartCoroutine(pick);
    }

    IEnumerator PickUpRoutine(GameObject pick, GameObject target)
    {
        pick.transform.position = new Vector3(target.transform.position.x, pick.transform.position.y, 0);

        while (pick.transform.position.y >= target.transform.position.y)
        {
            pick.transform.position = pick.transform.position + Vector3.down * 5/*moveSpeed*/ * Time.deltaTime;
            yield return true;
        }

        //집게 애니메이션 출력
        GameObject tongs = pick.transform.GetChild(0).gameObject;
        tongs.GetComponent<Animator>().SetBool("Pick", true);

        target.GetComponent<Animator>().SetBool("Pick", true);
        yield return new WaitForSeconds(0.2f);
        target.transform.parent = pick.transform;
        yield return new WaitForSeconds(0.3f);

        //외부 이동
        while (pick.transform.localPosition.y < 6)
        {
            pick.transform.localPosition = pick.transform.localPosition + Vector3.up * 5/*moveSpeed*/ * Time.deltaTime;
            yield return true;
        }

        tongs.GetComponent<Animator>().SetBool("Pick", false);

        target.transform.parent = null;
        this.pick = null;
    }

    //플레이어 죽음 판별, 이후 행동 실행
    void PlayerDead()
    {
        resultDepth = (int)player.transform.position.y;

        if (clear)
        {
            player.GetComponent<PlayerMoveMent>().ClearPos();
        }
        else
        {
            if (!player.GetComponent<PlayerMoveMent>().isDead)
                player.GetComponent<PlayerMoveMent>().OnDamaged(1000, false);
            PickUp(player);
        }

        //결과창으로 이동
        if (!giveUp)
            StartCoroutine(PopRoutine());
        else
            StartCoroutine(GiveUpRoutine());
    }

    IEnumerator PopRoutine()
    {
        while (pick != null)
        {
            yield return null;
        }
        if (clear)
            yield return new WaitForSecondsRealtime(3f);
        UIManager.Instance.ResultScreenPopup();
        Time.timeScale = 0;
    }

    IEnumerator GiveUpRoutine()
    {
        while (pick != null)
        {
            yield return null;
        }
        SceneManager.LoadScene("TitleScene");
        Time.timeScale = 1f;
    }

    public void GameExit()
    {
        if (!clear)
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
        
        SoundManager.Instance.SfxAllStop();

        //목표에 대한 행동 정의
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
            case BreakType.Monster:
                curCatch += 1;
                score += curCatch * 300;
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
