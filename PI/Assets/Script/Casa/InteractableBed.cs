using UnityEngine;

/// <summary>
/// Classe base para qualquer objeto interativo.
/// </summary>
public abstract class InteractableBed : MonoBehaviour
{
    public float interactionRange = 2f;
    public KeyCode interactionKey = KeyCode.E;
    protected Transform player;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        if (player == null) return;

        if (Vector3.Distance(player.position, transform.position) <= interactionRange)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                Interact();
            }
        }
    }

    public abstract void Interact();
}
