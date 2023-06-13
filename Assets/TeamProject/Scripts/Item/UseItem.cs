using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : Item, IItem
{
    [SerializeField] GameObject[] itemObject;
    GameObject item;

    [SerializeField] Sprite[] ItemSprites;
    
    CircleCollider2D circle;
    Animator anim;

    bool removePos = false;

    public override void Awake()
    {
        base.Awake();

        anim = GetComponent<Animator>();
        circle = GetComponent<CircleCollider2D>();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        circle.enabled = true;
        removePos = false;
    }

    public void Use(GameObject target)
    {
        if (isUse)
            return;

        if (GameManager.Instance.level.Equals(GameManager.Level.Tutorial))
        {
            target.GetComponent<PlayerMoveMent>().isTutorial = true;
            InputManager.Instance.isAttack = true;

            //튜토리얼 스크립트 출력
            //player.isTutorial = false, InputManager.Instance.isAttack = false;
        }
        else
        {
            circle.enabled = false;

            int ran = Random.Range(0, itemObject.Length);

            StartCoroutine(EffectRoutine());

            if (ran <= (int)ObjectManager.item.Bigbang)
            {
                item = ObjectManager.Instance.GetItem(ran);
                item.transform.position = new Vector2(0, 100);
            }
            else
                item = itemObject[ran];

            item.SetActive(true);
            item.GetComponent<IItem>().Use(target);
        }
        
        GameManager.Instance.PickUp(gameObject);
        StartCoroutine(GetRoutine());
    }

    IEnumerator EffectRoutine()
    {
        effect = ObjectManager.Instance.GetEffect((int)ObjectManager.effect.AirCore);
        effect.transform.position = GameManager.Instance.player.transform.position;
        effect.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        ObjectManager.Instance.ReturnEffect(effect, (int)ObjectManager.effect.AirCore);
    }

    IEnumerator GetRoutine()
    {
        while (GameManager.Instance.pick != null)
        {
            if (anim.GetBool("Pick") && !removePos)
            {
                removePos = true;
                if (group != null)
                    group.blockManager.RemovePos(this);
            }
            yield return null;
        }

        anim.SetBool("Pick", false);
        removePos = false;
        if (group != null)
            transform.parent = group.blockManager.transform;
        gameObject.SetActive(false);
    }
}
