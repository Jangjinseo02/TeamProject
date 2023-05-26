using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : Item, IItem
{
    [SerializeField] GameObject[] itemObject;
    GameObject item;

    [SerializeField] Sprite[] ItemSprites;
    SpriteRenderer spriteRenderer;

    public void Awake()
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

        item = itemObject[Random.Range(0, itemObject.Length)];
        item.GetComponent<IItem>().Use(target);
        
        if (group != null)
            group.blockManager.RemovePos(this);
        gameObject.SetActive(false);
    }
}
