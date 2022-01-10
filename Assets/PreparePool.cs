using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PreparePool : MonoBehaviour
{
    public GameObject networkManager;

    public List<GameObject> prefabs;

    DefaultPool pool;

    private void Start()
    {
        pool = PhotonNetwork.PrefabPool as DefaultPool;
        if (pool != null && prefabs != null)
        {
            foreach (GameObject prefab in prefabs)
            {
                pool.ResourceCache.Add(prefab.name, prefab);
                Debug.Log("Cached");
            }
        }
    }
}