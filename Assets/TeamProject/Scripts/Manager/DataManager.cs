using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingleTon<DataManager>
{
    public enum Level { Easy, Nomal, Hard };

    public Level level { get; set; }

    private void Awake()
    {
        if (FindObjectOfType<DataManager>() != this)
            Destroy(this.gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    public Level ReturnLevel()
    {
        return level;
    }
}
