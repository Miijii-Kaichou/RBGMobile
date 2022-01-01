using UnityEngine;

public class Singleton<T> : MonoBehaviour
{
    protected static T Instance;
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)System.Convert.ChangeType(this, typeof(T));

            if (!transform.parent)
                DontDestroyOnLoad(this);
        }
        else
        {
            if (!transform.parent)
                Destroy(gameObject);
        }
    }

    public static bool IsNull => Instance == null;
}
