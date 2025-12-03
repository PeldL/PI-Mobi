using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class CharacterController2D : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    Animator animator;
    public float speed = 2f;
    Vector2 motionVector;
    public Vector2 lastMotionVector;
    public bool moving;
    public bool wasMoving; // Para verificar se o estado de movimento mudou
    public AudioSource audioSource; // Referência para o AudioSource
    public AudioClip walkingSound; // Som de caminhada

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Pega o AudioSource do objeto

        // Se não tiver AudioSource, adiciona um
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        motionVector = new Vector2(horizontal, vertical);
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);

        bool wasMovingPreviously = moving; // Guarda o estado anterior
        moving = horizontal != 0 || vertical != 0;
        animator.SetBool("moving", moving);

        if (horizontal != 0 || vertical != 0)
        {
            lastMotionVector = new Vector2(horizontal, vertical).normalized;
            animator.SetFloat("lastVertical", vertical);
            animator.SetFloat("lastHorizontal", horizontal);
        }

        // Controla o som de caminhada
        HandleWalkingSound(wasMovingPreviously);
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rigidbody2d.linearVelocity = motionVector * speed;
    }

    private void HandleWalkingSound(bool wasMovingPreviously)
    {
        // Se começou a se mover agora
        if (moving && !wasMovingPreviously)
        {
            if (walkingSound != null && audioSource != null)
            {
                audioSource.clip = walkingSound;
                audioSource.loop = true; // Faz o som loopar enquanto estiver andando
                audioSource.Play();
            }
        }
        // Se parou de se mover
        else if (!moving && wasMovingPreviously)
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}