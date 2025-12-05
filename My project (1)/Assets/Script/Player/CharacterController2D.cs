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
    public bool wasMoving;
    public AudioSource audioSource;
    public AudioClip walkingSound;

    [Header("Mobile Controls")]
    public Joystick movementJoystick; // Usando a classe base Joystick

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Pega input do joystick ou teclado
        float horizontal = movementJoystick != null ? movementJoystick.Horizontal : Input.GetAxisRaw("Horizontal");
        float vertical = movementJoystick != null ? movementJoystick.Vertical : Input.GetAxisRaw("Vertical");

        motionVector = new Vector2(horizontal, vertical);
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);

        bool wasMovingPreviously = moving;
        moving = horizontal != 0 || vertical != 0;
        animator.SetBool("moving", moving);

        if (horizontal != 0 || vertical != 0)
        {
            lastMotionVector = new Vector2(horizontal, vertical).normalized;
            animator.SetFloat("lastVertical", vertical);
            animator.SetFloat("lastHorizontal", horizontal);
        }

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
        if (moving && !wasMovingPreviously)
        {
            if (walkingSound != null && audioSource != null)
            {
                audioSource.clip = walkingSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
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