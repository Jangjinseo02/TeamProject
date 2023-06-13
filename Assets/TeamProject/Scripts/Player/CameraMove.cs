using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private GameObject player;
    PlayerMoveMent playerScript;

    Vector3 pos;

    HashSet<Block> inCamera = new HashSet<Block>();

    int layer = 0;

    private void Awake()
    {
        playerScript = player.GetComponent<PlayerMoveMent>();
    }

    private void Start()
    {
        pos.x = player.transform.position.x + 0.1f;
        pos.y = player.transform.position.y;

        this.transform.position = pos;
        
    }

    void Update()
    {
        if(!playerScript.isDead)
            this.transform.position = new Vector3(transform.position.x, player.transform.position.y, -10);
    }

    public void StartSetting()
    {
        GameManager.Level level = GameManager.Instance.level;

        switch (level)
        {
            case GameManager.Level.Easy:
                Camera.main.orthographicSize = 4.5f;
                break;
            case GameManager.Level.Nomal:
                Camera.main.orthographicSize = 5.85f;
                break;
            case GameManager.Level.Hard:
                Camera.main.orthographicSize = 7.15f;
                break;
            case GameManager.Level.Extra:
                Camera.main.orthographicSize = 7.15f;
                break;
            case GameManager.Level.Tutorial:
                Camera.main.orthographicSize = 4.5f;
                break;
        }

        pos.x = player.transform.position.x + 0.1f;
        pos.y = player.transform.position.y;

        this.transform.position = pos;
        layer = (1 << LayerMask.NameToLayer("Block")) + (1 << LayerMask.NameToLayer("Monster"));

        StartCoroutine(CheckList());
    }

    void Check()
    {
        if (GameManager.Instance.clear)
            return;
        ObjectCheckAtiveInHierarchy();
        HashSet<Block> blocks = new HashSet<Block>(inCamera);

        foreach (Block member in blocks)
        {
            CheckBlock(member);
        }
    }

    public void CheckBlock(Block target)
    {
        if (GameManager.Instance.clear)
            return;

        switch (target.tag)
        {
            case "Monster":
                Monster monster = target.GetComponent<Monster>();
                if (monster != null)
                {
                    if (monster.inCamera)
                        return;
                    monster.CheckTarget(inCamera);
                }
                break;
            case "Glass":
                GlassBlock glass = target.GetComponent<GlassBlock>();
                if (glass != null)
                {
                    if (glass.inCamera)
                        return;
                    glass.InCamera();
                }
                break;
        }
    }

    void ObjectCheckAtiveInHierarchy()
    {
        if (GameManager.Instance.clear)
        {
            List<Block> allRemoveList = new List<Block>(inCamera);

            foreach(Block member in allRemoveList)
            {
                if(member != null)
                    inCamera.Remove(member);
            }

            return;
        }

        List<Block> removeList = new List<Block>();

        foreach (Block member in inCamera)
        {
            if (!member.gameObject.activeInHierarchy)
                if (!removeList.Contains(member))
                    removeList.Add(member);
        }

        for (int i = 0; i < removeList.Count; i++)
        {
            inCamera.Remove(removeList[i]);
        }

        removeList.Clear();
    }

    IEnumerator CheckList()
    {
        PlayerMoveMent playerDead = player.GetComponent<PlayerMoveMent>();

        while (!playerDead.isDead || !GameManager.Instance.clear)
        {
            Check();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public HashSet<Block> ReturnBlock()
    {
        return inCamera;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.clear)
            return;

        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Block")) ||
            collision.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
        {
            inCamera.Add(collision.GetComponent<Block>());

            Debug.Log(inCamera.Count);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GameManager.Instance.clear)
            return;

        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Block")) || 
            collision.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
        {
            switch (collision.tag)
            {
                case "Monster":
                    Monster monster = collision.GetComponent<Monster>();
                    if (monster != null)
                    {
                        if (monster.inCamera)
                            monster.CameraOut();
                    }
                    break;
            }
            inCamera.Remove(collision.GetComponent<Block>());
        }
    }
}
