using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void RunOnMainThread(System.Action action)
    {
        if (instance != null)
        {
            instance.Run(action);
        }
    }

    private void Run(System.Action action)
    {
        if (action != null)
        {
            lock (this)
            {
                actions.Enqueue(action);
            }
        }
    }

    private Queue<System.Action> actions = new Queue<System.Action>();

    private void Update()
    {
        while (actions.Count > 0)
        {
            System.Action action = null;
            lock (this)
            {
                action = actions.Dequeue();
            }
            action?.Invoke();
        }
    }
}

