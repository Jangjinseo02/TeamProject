using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : Block, IItem
{
    [SerializeField] GameObject[] itemObject;
    GameObject item;

    public void Use(GameObject target)
    {
        item = itemObject[Random.Range(0, itemObject.Length)];
        item.GetComponent<IItem>().Use(target);
        //ObjectManager.Instance.ReturnBlock(this.gameObject);
        if (group != null)
            group.blockManager.RemovePos(this);
        gameObject.SetActive(false);
    }
}
