using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoBeam : MonoBehaviour
{
    float curTime = 0;

    Ufo ufo;

    public void OnEnable()
    {
        curTime = 0;
        ufo = GetComponentInParent<Ufo>();
        SoundManager.Instance.SfxPlay(SoundManager.Sfx.UfoBeam, true);

        StartCoroutine(EndTime());
    }

    private void Update()
    {
        TickCheck();
    }

    void TickCheck()
    {
        RaycastHit2D rayhit = Physics2D.Raycast(ufo.gameObject.transform.position, Vector2.down,
                                                100, LayerMask.GetMask("Player"));
        if (rayhit)
        {
            curTime += Time.deltaTime;

            if (curTime >= 0.4f)
            {
                GameObject playerObj = GameManager.Instance.player;
                playerObj.GetComponent<PlayerMoveMent>().PlayerStun();

                StopCoroutine(EndTime());
                Destroy(ufo.gameObject);
            }
        }
    }

    IEnumerator EndTime()
    {
        yield return new WaitForSeconds(8f);

        SoundManager.Instance.SfxStop(SoundManager.Sfx.UfoBeam);

        Destroy(ufo.gameObject);
    }
}
