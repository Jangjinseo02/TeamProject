using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingleTon<GameManager>
{
    public GameObject player;

    [SerializeField] BlockMgr[] blockMgrs;
    [SerializeField] GameObject backGround;
    Vector2 blockMgrPos;

    BoxCollider2D boxCollider;
    public int stageLevel = 0;
    int curLevel = 0;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        stageLevel = 0;
        blockMgrPos = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetBlockStage()
    {
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

        stageLevel += 1;
        curLevel = stageLevel % 2;
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
