using UnityEngine;

public class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this as T)
        {
            Debug.LogWarning($"Another instance of {GetType()} already exists. Destroying this.");
            Destroy(this);
        }
        else
            Instance = this as T;
    }
}
