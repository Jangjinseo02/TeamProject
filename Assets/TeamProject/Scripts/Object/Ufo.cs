using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ufo : MonoBehaviour
{
    GameObject playerObj;
    UfoBeam ufoBeam;

    private void Start()
    {
        playerObj = GameManager.Instance.player;
        ufoBeam = GetComponentInChildren<UfoBeam>();

        ufoBeam.gameObject.SetActive(false);

        StartCoroutine(MoveUpdate());
    }

    IEnumerator MoveUpdate()
    {
        float startTime = 0;
        float maxTime = 4f;

        while (startTime <= maxTime)
        {
            gameObject.transform.position = playerObj.transform.position + Vector3.up;
            startTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        ufoBeam.gameObject.SetActive(true);
    }
}
