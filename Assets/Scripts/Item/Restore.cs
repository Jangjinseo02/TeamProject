using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restore : MonoBehaviour, IItem
{

    public void Set(GameObject target) 
    {
        gameObject.transform.position = target.gameObject.transform.position;
        gameObject.SetActive(true);
    }
    public void Use(GameObject target)
    {
        PlayerMoveMent player = target.GetComponent<PlayerMoveMent>();

        if (player != null)
        {
            player.oxygen += 20f;
            if (player.oxygen >= 100)
                player.oxygen = 100;

            UIManager.Instance.SettingAirImage();
            Destroy(gameObject);
            //gameObject (false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Block")
        {
            Destroy(gameObject);
        }
    }
}
