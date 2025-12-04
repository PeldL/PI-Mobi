using UnityEngine;
public class TeleportEnemy : MonoBehaviour
{
    public Transform[] teleportPoints;
    public float teleportCooldown = 5f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= teleportCooldown)
        {
            Teleport();
            timer = 0;
        }
    }

    void Teleport()
    {
        int index = Random.Range(0, teleportPoints.Length);
        transform.position = teleportPoints[index].position;
    }
}