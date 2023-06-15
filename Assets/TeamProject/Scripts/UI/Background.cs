using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed;
    public int startIndex;
    public int endIndex;
    public Transform[] sprites;

    float viewheight;

    private void Awake()
    {
        viewheight = Camera.main.orthographicSize * 2;
    }

    void Update()
    {
        if (sprites[endIndex] != null && sprites[endIndex].position.y < viewheight)
        {
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            Vector3 frontSpritePos = sprites[endIndex].localPosition;
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * 10;

            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave - 1) == -1 ? sprites.Length - 1: startIndexSave - 1;
        }
    }
}
