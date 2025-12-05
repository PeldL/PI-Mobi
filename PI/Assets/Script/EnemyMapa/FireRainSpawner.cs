using UnityEngine;

public class FireRainSpawner : MonoBehaviour
{
    public GameObject firePrefab;
    public float intervalo = 1f;
    public float larguraMapa = 10f;
    public float alturaSpawn = 5f;

    void Start()
    {
        InvokeRepeating("SpawnFire", 0f, intervalo);
    }

    void SpawnFire()
    {
        float x = Random.Range(-larguraMapa / 2, larguraMapa / 2);
        Vector2 spawnPosition = new Vector2(x, alturaSpawn);
        Instantiate(firePrefab, spawnPosition, Quaternion.identity);
    }
}
