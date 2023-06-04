using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group
{
    HashSet<Block> blocks;
    BlockMgr blockMgr;

    public bool unbalance = false;

    public int Count
    {
        get { return this.blocks.Count; }
    }
    public BlockMgr blockManager
    {
        get { return this.blockMgr; }
    }

    public IEnumerator<Block> GetEnumerator()
    {
        foreach (Block block in blocks)
        {
            yield return block;
        }
    }

    public Group(BlockMgr blockMgr)
    {
        blocks = new HashSet<Block>();
        this.blockMgr = blockMgr;
    }

    public bool AddGroup(Block block)
    {
        block.group = this;
        return blocks.Add(block);
    }
    public void CheckGroupUnbalance()
    {
        HashSet<Group> historyGroup = new HashSet<Group>();
        HashSet<Group> resultGroup = new HashSet<Group>();

        GroupUnbalance(historyGroup, resultGroup);

        blockManager.CheckUnbalanceList(resultGroup);
    }

    //여기서 재귀 부분 문제가 발생하는 것 같음. 
    public void GroupUnbalance(HashSet<Group> history, HashSet<Group> result)
    {
        if (this.unbalance)
            return;

        history.Add(this);

        foreach (Block block in blocks)
        {
            //아래 블록
            Block underBlock = blockManager.UnderBlock(block.row, block.col);

            if (underBlock != null && underBlock.group != this)
            {
                if (underBlock.group.unbalance)
                    continue;

                this.unbalance = false;
                return;
            }
        }
        
        this.unbalance = true;
        result.Add(this);
        //blockManager.CheckUnbalanceList(this);

        foreach (Block block in blocks)
        {
            Block upBlock = blockManager.UpBlock(block.row, block.col);
            if (upBlock != null && !history.Contains(upBlock.group))
            {
                if (upBlock.group.unbalance)
                    continue;

                upBlock.group.GroupUnbalance(history, result);
            }
        }
    }

}


public class Block : MonoBehaviour
{
    public int type;
    public Group group;
    public float health = 10;
    public bool isCheck = false;

    public float gravity = 3f;
    float shakeTime = 1;
    
    IEnumerator drop;
    IEnumerator shake;
    IEnumerator blink;

    protected SpriteRenderer sprite;
    [SerializeField] Sprite[] sprites;

    [SerializeField] protected GameObject effect;
    [SerializeField] protected GameObject aliveEffect;

    public virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public virtual void OnEnable()
    {
        if(aliveEffect != null)
            SetAlive();
    }

    void SetAlive()
    {
        StartCoroutine(SetAliveRoutine());
    }

    IEnumerator SetAliveRoutine()
    {
        sprite.color = Color.clear;
        aliveEffect.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        sprite.color = Color.white;
        aliveEffect.SetActive(false);
    }

    public virtual int attackDamage
    {
        get { return 35 + (GameManager.Instance.stageLevel * 2); }
    }

    public int row
    {
        get { return Mathf.CeilToInt(transform.localPosition.y);}
    }
    public int col;

    public bool dropping
    {
        get { return this.drop != null; }
    }

    public bool shaking
    {
        get { return this.shake != null; }
    }

    public bool blinking
    {
        get { return this.blink != null; }
    }

    public void DropStart()
    {
        ShakeEnd();
        drop = GetDropEnumerator(gravity);
    }

    public void ShakeStart()
    {
        if (this.row == 0)
            return;

        shake = GetShakeEnumerator();
        if (gameObject.activeInHierarchy)
            StartCoroutine(shake);
    }

    public void BlinkStart()
    {
        blink = GetBlinkEnumerator();
        if (gameObject.activeInHierarchy)
            StartCoroutine(blink);
    }
    public void BlinkEnd()
    {
        if (gameObject.activeInHierarchy)
            StopCoroutine(blink);
        blink = null;
    }

    public void ShakeEnd()
    {
        if (gameObject.activeInHierarchy && shake != null)
            StopCoroutine(shake);
        shake = null;
    }

    public void DropNext()
    {
        if (gameObject.activeInHierarchy)
            drop.MoveNext();
    }

    public void DropEnd()
    {
        drop = null;
    }

    IEnumerator GetDropEnumerator(float gravity)
    {
        this.group.blockManager.RemovePos(this);
        while (true)
        {
            transform.localPosition = transform.localPosition + Vector3.down * gravity * Time.deltaTime;
            yield return true;
        }
    }
    IEnumerator GetShakeEnumerator()
    {
        Vector2 pos = Vector2.zero;
        pos.x = col;
        pos.y = transform.localPosition.y;

        float curTime = 0;
        float beforeX = pos.x;

        while (curTime < shakeTime)
        {
            float offset = Mathf.PingPong(Time.time * 10f, 0.1f) - 0.05f;
            pos.x += offset;
            transform.localPosition = pos;

            curTime += Time.deltaTime;
            yield return true;
        }

        pos.x = beforeX;
        transform.localPosition = pos;
        DropStart();
    }

    IEnumerator GetBlinkEnumerator()
    {
        float startTime = Time.time;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        while (Time.time < startTime + 0.5f)
        {
            float alpha = Mathf.Sin(Time.time * 1000.0f);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);

            yield return true;
        }
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 255);

        this.OnDamaged(100);
    }

    public void Setting(int row, int col/*, int type*/)
    {
        //this.row = row;
        this.col = col;
        if (type < 5 && type != 0)
            sprite.sprite = sprites[GameManager.Instance.ran];
    }

    public void RemoveGroup(BlockMgr blockMgr)
    {
        group = null;
        group = new Group(blockMgr);
        group.blockManager.Search(this);
    }

    //ClearBlock Method
    public virtual void StageClear()
    {
        Debug.Log("Block");
    }


    public virtual void OnDamaged(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            //type = -1;
            if(gameObject.activeInHierarchy)
                StartCoroutine(DestroyRoutine());
        }
    }

    IEnumerator DestroyRoutine()
    {
        effect = ObjectManager.Instance.GetEffect((int)ObjectManager.effect.Nomal);
        effect.transform.position = this.transform.position;
        sprite.color = Color.clear;
        effect.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        ObjectManager.Instance.ReturnEffect(effect, (int)ObjectManager.effect.Nomal);
        yield return new WaitForSeconds(0.05f);

        group.blockManager.RemovePos(this);
        gameObject.SetActive(false);
    }
}
