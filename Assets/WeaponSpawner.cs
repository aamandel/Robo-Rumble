using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] Transform spawnPoint;
    [SerializeField] List<GameObject> weaponPrefabs;
    [SerializeField] LayerMask weaponLayers;
    private bool trySpawn;

    private void Start()
    {
        TrySpawn();
    }
    // Update is called once per frame
    void Update()
    {
        if (trySpawn)
        {
            trySpawn = false;
            Invoke("TrySpawn", timeBetweenSpawns);
        }
    }

    private void TrySpawn()
    {
        trySpawn = true;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPoint.position, 0.5f, weaponLayers);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "Weapon")
            {
                return;
            }
        }
        GameObject weapon = weaponPrefabs[Random.Range(0, weaponPrefabs.Count)];
        Instantiate(weapon, spawnPoint.transform.position, Quaternion.identity);
    }
}
