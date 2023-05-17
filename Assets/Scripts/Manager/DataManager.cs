using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingleTon<DataManager>
{
    public enum Level { Easy, Nomal, Hard };
    Level level;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        level = Level.Nomal;
    }

    public Level ReturnLevel()
    {
        return level;
    }
}
