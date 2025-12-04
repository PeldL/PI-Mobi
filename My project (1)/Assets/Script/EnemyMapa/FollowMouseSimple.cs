using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    public Transform player;
    public CharacterController2D playerController;
    public float followDistance = 0.5f;
    public float followSpeed = 10f;

    void Update()
    {
        if (player == null || playerController == null)
            return;

        // Pega a direção do movimento do jogador
        Vector2 lookDirection = playerController.lastMotionVector;

        // Se estiver parado, mantém direção padrão
        if (lookDirection == Vector2.zero)
            lookDirection = Vector2.right;

        // Define posição desejada baseada na direção
        Vector3 targetPosition = player.position + (Vector3)(lookDirection.normalized * followDistance);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Rotaciona o objeto para a direção que o jogador está olhando (corrigido com +180)
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);
    }
}
