using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] float timeBetweenSpawns;
    [SerializeField] Transform spawnPoint;
    [SerializeField] List<GameObject> weaponPrefabs;
    [SerializeField] List<GameObject> epicWeaponPrefabs;
    [SerializeField] GameObject epicFX;
    [SerializeField] float chanceOfEpic = 0.1f;
    [SerializeField] LayerMask weaponLayers;
    public float updateRate = 0.5f;

    private void Awake()
    {
        epicFX.SetActive(false);
        Invoke("Spawn", 0.1f);
        StartCoroutine(UpdateSpawner());
    }

    private IEnumerator UpdateSpawner()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(updateRate);
            bool canSpawn = true;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPoint.position, 0.5f, weaponLayers);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.tag == "Weapon")
                {
                    canSpawn = false;
                }
            }
            if (canSpawn)
            {
                epicFX.SetActive(false);
                yield return new WaitForSeconds(timeBetweenSpawns);
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        GameObject weapon = weaponPrefabs[Random.Range(0, weaponPrefabs.Count)];
        if (Random.Range(0f, 1f) < chanceOfEpic)
        {
            weapon = epicWeaponPrefabs[Random.Range(0, epicWeaponPrefabs.Count)];
            epicFX.SetActive(true);
        }
        Instantiate(weapon, spawnPoint.transform.position, Quaternion.identity);
    }
}
