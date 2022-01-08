using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A special singleton compnent This will keep an object
/// persistent until call to be disabled.
/// </summary>
public class PersistentObject : MonoBehaviour
{
   
    public bool forceSingleton = false;
    public int objectID => PersistentObjectHierarchy.GetID(this);
    private static PersistentObject Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(Instance);
        }
        else
        {
            if (forceSingleton)
                Destroy(gameObject);
        }
    }

    public void Disable()
    {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(false);
    }

    public void Enable()
    {
        if (gameObject.activeSelf) return;

        gameObject.SetActive(true);
    }
}

public static class PersistentObjectHierarchy
{
    static Dictionary<int, PersistentObject> objectIdDictionary = new Dictionary<int, PersistentObject>();
    public static int GetID(PersistentObject obj) => GenerateID(obj);

    static int GenerateID(PersistentObject obj)
    {
        while (true)
        {
            var genID = Random.Range(1000, 9999);
            if (!objectIdDictionary.ContainsKey(genID) || objectIdDictionary.Count == 0)
            {
                objectIdDictionary.Add(genID, obj);
                return genID;
            }
        }
    }

    /// <summary>
    /// Checks if a certain object exists.
    /// It'll check the object, and it's uID
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool Exists(PersistentObject obj) => objectIdDictionary.ContainsValue(obj);
}