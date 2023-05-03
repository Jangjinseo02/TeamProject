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

    //여기서 재귀 부분 문제가 발생하는 것 같음. 
    public void GroupUnbalance()
    {
        Debug.Log("GroupUnbalance");
        foreach (Block block in blocks)
        {
            //아래 블록
            Block underBlock = blockManager.UnderBlock(block.row, block.col);

            if (underBlock != null)
            {
                if (underBlock.group == this)
                {
                    continue;
                }
                else
                {
                    
                    if (underBlock.group.unbalance)
                        continue;
                
                    this.unbalance = false;
                    return;
                }
            }
        }
        
        this.unbalance = true;
        blockManager.CheckUnbalanceList(this);

        foreach (Block block in blocks)
        {
            Block upBlock = blockManager.UpBlock(block.row, block.col);
            if (upBlock != null)
            {
                if (upBlock.group == this || upBlock.group.unbalance)
                    continue;

                upBlock.group.GroupUnbalance();
            }
        }
    }

}


public class Block : MonoBehaviour
{
    public enum BlockState { Drop, Idle }
    public BlockState blockState = BlockState.Idle;
    public int type;
    public float gravity = 5f;

    protected float health = 10;
    public Group group;
    public bool isCheck = false;
    bool isMove = false;
    public Vector3 afVec;
    public Vector3 pos;
    //public int row;
    //public int col;

    IEnumerator drop;
    IEnumerator shake;
    IEnumerator blink;

    SpriteRenderer sprite;
    [SerializeField] Sprite[] sprites;

    private void OnEnable()
    {
        pos = transform.localPosition;
    }
    private void OnDisable()
    {
        //ObjectManager.Instance.ReturnBlock(this.gameObject);
    }
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }


    private void Start()
    {
    }

    public int attackDamage
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

        float startTime = Time.time;
        float beforeX = pos.x;

        while (Time.time < startTime + 0.5f)
        {
            float offset = Mathf.PingPong(Time.time * 10f, 0.1f) - 0.05f;
            pos.x += offset;
            transform.localPosition = pos;
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

        this.OnDamaged(10);
    }

    public void Setting(int row, int col/*, int type*/)
    {
        //this.row = row;
        this.col = col;
        if (type < 5 && type != 0)
            sprite.sprite = sprites[GameManager.Instance.ran];
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
            Debug.Log("OnDamaged");
            group.blockManager.RemovePos(this);
            gameObject.SetActive(false);
        }
    }
}
