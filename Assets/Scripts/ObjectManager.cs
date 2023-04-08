using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : SingleTon<ObjectManager>
{
    [SerializeField] private GameObject[] blockPrefabs;
    [SerializeField] private GameObject blockPool;
    private Dictionary<GameObject, List<GameObject>> objectPool = new Dictionary<GameObject, List<GameObject>>();

    int clearBlock = 110;
    int nomalBlock = 400;
    int specBlock = 50;
    int monsterCount = 15;

    private void Awake()
    {
        foreach(GameObject prefab in blockPrefabs)
        {
            objectPool[prefab] = new List<GameObject>();
        }
    }

    private void Start()
    {
        for (int j = 0; j < blockPrefabs.Length; j++)
        {
            if (j < 2)
            {
                for (int i = 0; i < clearBlock; i++)
                {
                    CreateBlock(j);
                }
            }
            else if (j < 6)
            {
                for (int i = 0; i < nomalBlock; i++)
                {
                    CreateBlock(j);
                }
            }
            else if (j < 8)
            {
                for (int i = 0; i < specBlock; i++)
                {
                    CreateBlock(j);
                }
            }
            else if (j >= 8)
            {
                for (int i = 0; i < monsterCount; i++)
                {
                    CreateBlock(j);
                }
            }
        }
    }

    //오브젝트 생성 및 오브젝트 풀에 저장
    public GameObject CreateBlock(int blockType) 
    {
        GameObject prefab = blockPrefabs[blockType];
        GameObject block = Instantiate(prefab);
        block.transform.parent = blockPool.transform;
        block.SetActive(false);
        objectPool[prefab].Add(block);
        return block;
    }

    public void ReturnBlock(GameObject block)
    {
        block.SetActive(false);
        GameObject prefab = blockPrefabs[block.GetComponent<Block>().type];
        objectPool[prefab].Add(block);
    }

    public GameObject GetBlock(int blockType)
    {
        GameObject prefab = blockPrefabs[blockType];
        if(objectPool[prefab].Count == 0)
        {
            objectPool[prefab].Add(CreateBlock(blockType));
        }

        GameObject block = objectPool[prefab][0];
        objectPool[prefab].RemoveAt(0);
        block.SetActive(true);

        return block;
    }

}
