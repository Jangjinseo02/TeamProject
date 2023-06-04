using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : Item, IItem
{
    [SerializeField] GameObject[] itemObject;
    GameObject item;

    [SerializeField] Sprite[] ItemSprites;
    SpriteRenderer spriteRenderer;

    public override void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void SpriteSetting(int monsterType)
    {
        spriteRenderer.sprite = ItemSprites[monsterType];
    }

    public void Use(GameObject target)
    {
        if (isUse)
            return;

        int ran = Random.Range(0, 7);

        if (ran <= (int)ObjectManager.item.Bigbang)
        {
            item = ObjectManager.Instance.GetItem(ran);
            item.transform.position = new Vector2(0, 100);
        }
        else
            item = itemObject[ran];

        item.SetActive(true);
        item.GetComponent<IItem>().Use(target);
        
        if (group != null)
            group.blockManager.RemovePos(this);
        gameObject.SetActive(false);
    }
}
