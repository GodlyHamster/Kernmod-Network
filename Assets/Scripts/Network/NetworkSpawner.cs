using System.Collections.Generic;
using UnityEngine;

public class NetworkSpawner : MonoBehaviour
{
    public static NetworkSpawner instance;

    [SerializeField]
    private GameObject playerPrefab;

    private Dictionary<int, GameObject> spawnedObjects = new Dictionary<int, GameObject>();

    private int spawnCount = 0;

    private void Awake()
    {
        instance = this;
    }

    public int SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, gameObject.scene) as GameObject;
        spawnedObjects.Add(spawnCount, player);
        return spawnCount++;
    }

    public GameObject GetObject(int objectId)
    {
        return spawnedObjects[objectId];
    }
}
