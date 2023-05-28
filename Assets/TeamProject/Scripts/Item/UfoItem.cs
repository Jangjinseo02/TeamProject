using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoItem : Item, IItem
{
    [SerializeField] GameObject ufoPre;

    public void Use(GameObject target)
    {
        if (isUse)
            return;

        PlayerMoveMent player = target.GetComponent<PlayerMoveMent>();

        if (player != null && !isUse)
        {
            GameObject ufo = Instantiate(ufoPre);

            ufo.SetActive(true);

            //Ãß°¡
            if (group != null)
            {
                group.blockManager.RemovePos(this);
            }
            gameObject.SetActive(false);
        }
    }
}
