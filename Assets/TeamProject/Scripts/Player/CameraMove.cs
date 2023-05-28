using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private GameObject player;

    Vector3 pos;

    HashSet<Block> inCamera = new HashSet<Block>();

    private void Start()
    {
        pos.x = player.transform.position.x;
        pos.y = player.transform.position.y;

        this.transform.position = pos;

        StartCoroutine(CheckList());
    }

    void Update()
    {
        this.transform.position = new Vector3(transform.position.x, player.transform.position.y, -10);
    }

    void Check()
    {
        ObjectCheckAtiveInHierarchy();
        HashSet<Block> blocks = new HashSet<Block>(inCamera);

        foreach (Block member in blocks)
        {
            CheckBlock(member);
        }
    }

    public void CheckBlock(Block target)
    {

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
                    glass.OnDamaged((int)target.health);
                break;
        }
    }

    void ObjectCheckAtiveInHierarchy()
    {
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

        while (!playerDead.isDead)
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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Block"))
        {
            inCamera.Add(collision.GetComponent<Block>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Block"))
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
