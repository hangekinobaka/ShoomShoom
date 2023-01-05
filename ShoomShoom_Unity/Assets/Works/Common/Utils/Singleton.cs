using System;
using UnityEngine;

/// <summary>
/// Singleton Class
/// </summary>
/// <typeparam name="T">Class to be extended from MonoBehaviour</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {

        get
        {

            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);
                if (instance == null)
                {
                    Debug.LogError(t + ":Null");
                }
            }

            return instance;
        }

    }

    virtual protected void Awake()
    {
        CheckInstance();
    }

    protected bool CheckInstance()
    {
        if (instance == null)
        {
            instance = this as T;
            return true;
        }
        else if (Instance == this)
        {
            return true;
        }

        Destroy(this);
        return false;
    }
    public static bool IsNull()
    {
        return instance == null;
    }
}