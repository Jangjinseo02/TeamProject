using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : SingleTon<ObjectManager>
{
    public enum effect { Nomal = 0, Hard, Glass, Meteor, AirCore, MonsterItem, BigBang }
    public enum item { Restore = 0, Recovery, Bigbang }

    [SerializeField] private GameObject[] blockPrefabs;
    [SerializeField] private GameObject[] effectPrefabs;
    [SerializeField] private GameObject[] itemPrefabs;

    [SerializeField] private GameObject blockPool;
    [SerializeField] private GameObject effectPool;
    [SerializeField] private GameObject itemPool;

    private Dictionary<GameObject, List<GameObject>> objectPool = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, List<GameObject>> effectObjectPool = new Dictionary<GameObject, List<GameObject>>();
    private Dictionary<GameObject, List<GameObject>> itemObjectPool = new Dictionary<GameObject, List<GameObject>>();

    int clearBlock = 350;
    int nomalBlock = 2000;
    int specBlock = 300;
    int monsterCount = 50;
    int itemCount = 5;

    int effectCount = 75;

    private void Awake()
    {
        foreach (GameObject prefab in blockPrefabs)
            objectPool[prefab] = new List<GameObject>();
        foreach (GameObject prefab in effectPrefabs)
            effectObjectPool[prefab] = new List<GameObject>();
        foreach (GameObject prefab in itemPrefabs)
            itemObjectPool[prefab] = new List<GameObject>();
    }

    private void Start()
    {
        for (int j = 0; j < blockPrefabs.Length; j++)
        {
            if (j < 1)
            {
                for (int i = 0; i < clearBlock; i++)
                    CreateBlock(j);
            }
            else if (j < 5)
            {
                for (int i = 0; i < nomalBlock; i++)
                    CreateBlock(j);
            }
            else if (j < 8)
            {
                for (int i = 0; i < specBlock; i++)
                    CreateBlock(j);
            }
            else if (j < 11)
            {
                for (int i = 0; i < monsterCount; i++)
                    CreateBlock(j);
            }
            else if (j >= 11)
            {
                for (int i = 0; i < itemCount; i++)
                    CreateBlock(j);
            }
        }

        for (int j = 0; j < effectPrefabs.Length; j++)
        {
            for (int i = 0; i < effectCount; i++)
            {
                if (j < 4)
                    CreateEffect(j);
            }
        }

        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            if (i < 5)
                CreateItem(i);
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

    public GameObject CreateEffect(int value)
    {
        GameObject prefab = effectPrefabs[value];
        GameObject effect = Instantiate(prefab);
        effect.transform.parent = effectPool.transform;
        effect.SetActive(false);
        effectObjectPool[prefab].Add(effect);
        return effect;
    }
    public GameObject CreateItem(int value)
    {
        GameObject prefab = itemPrefabs[value];
        GameObject item = Instantiate(prefab);
        item.transform.parent = itemPool.transform;
        item.SetActive(false);
        itemObjectPool[prefab].Add(item);
        return item;
    }

    public void ReturnBlock(GameObject block)
    {
        block.transform.parent = blockPool.transform;
        block.SetActive(false);
        GameObject prefab = blockPrefabs[block.GetComponent<Block>().type];
        objectPool[prefab].Add(block);
    }

    public void ReturnEffect(GameObject effect, int value)
    {
        effect.SetActive(false);
        //effect.transform.parent = effectPool.transform;
        GameObject prefab = effectPrefabs[value];
        effectObjectPool[prefab].Add(effect);
    }
    public void ReturnItem(GameObject item, int value)
    {
        item.SetActive(false);
        //effect.transform.parent = effectPool.transform;
        GameObject prefab = itemPrefabs[value];
        itemObjectPool[prefab].Add(item);
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
        //block.SetActive(true);

        return block;
    }

    public GameObject GetEffect(int value)
    {
        GameObject prefab = effectPrefabs[value];
        if (effectObjectPool[prefab].Count == 0)
        {
            effectObjectPool[prefab].Add(CreateEffect(value));
        }

        GameObject effect = effectObjectPool[prefab][0];
        effectObjectPool[prefab].RemoveAt(0);
        //block.SetActive(true);

        return effect;
    }

    public GameObject GetItem(int value)
    {
        GameObject prefab = itemPrefabs[value];
        if (itemObjectPool[prefab].Count == 0)
        {
            itemObjectPool[prefab].Add(CreateItem(value));
        }

        GameObject item = itemObjectPool[prefab][0];
        itemObjectPool[prefab].RemoveAt(0);
        //block.SetActive(true);

        return item;
    }

}
