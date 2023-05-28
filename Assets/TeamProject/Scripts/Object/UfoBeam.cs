using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoBeam : MonoBehaviour
{

    float curTime = 0;

    BoxCollider2D boxCol;
    Ufo ufo;

    public void OnEnable()
    {
        curTime = 0;
        boxCol = GetComponent<BoxCollider2D>();
        ufo = GetComponentInParent<Ufo>();

        StartCoroutine(EndTime());
    }

    IEnumerator EndTime()
    {
        yield return new WaitForSeconds(8f);

        Destroy(ufo.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            curTime += Time.deltaTime;
            Debug.Log(curTime);

            //boxCol.offset = new Vector2(0, Random.Range(curOffset, startOffset));

            if (curTime >= 0.4f)
            {
                //플레이어 기절
                Debug.Log("플레이어 기절");

                GameObject playerObj = GameManager.Instance.player;
                playerObj.GetComponent<PlayerMoveMent>().PlayerStun();
                boxCol.enabled = false;

                StopCoroutine(EndTime());
                Destroy(ufo.gameObject);
            }
        }
    }
}
