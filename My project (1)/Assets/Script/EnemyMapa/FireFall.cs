using System.Collections;
using UnityEngine;

public class FireFall : MonoBehaviour
{
    [SerializeField] Transform[] points;
    [SerializeField] GameObject firePrefab;  // Prefab do fogo que cai (com script FireDamage)
    [SerializeField] float cooldownTime = 3f;

    void Start()
    {
        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        int randomNumber = Random.Range(0, points.Length);
        Instantiate(firePrefab, points[randomNumber].position, Quaternion.identity);

        StartCoroutine(Cooldown());
    }
}
