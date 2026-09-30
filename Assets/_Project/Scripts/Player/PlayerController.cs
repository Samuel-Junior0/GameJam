using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;

    public Vector2 FacingDirection { get; private set; } = Vector2.down;

    // Enquanto true, o movimento NÃO muda a direção do personagem (usado durante o ataque).
    // Trava pelo tempo mínimo (cobre o(s) frame(s) até o Animator entrar no state de ataque)
    // E enquanto o Animator estiver num state com a tag "Attack" (dura o clipe inteiro).
    public bool IsAimLocked => Time.time < aimLockUntil || IsAttackAnimationPlaying();

    bool IsAttackAnimationPlaying()
    {
        if (animator == null) return false;

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")) return true;

        // Durante a transição de entrada no ataque o state atual ainda é o antigo
        return animator.IsInTransition(0) && animator.GetNextAnimatorStateInfo(0).IsTag("Attack");
    }

    Rigidbody2D rb;
    SpriteRenderer sprite;
    Animator animator;
    Vector2 input;
    float aimLockUntil;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        sprite = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (GameManager.InputLocked)
        {
            input = Vector2.zero;
            UpdateAnimator();   // evita ficar "andando parado" durante diálogo/derrota
            return;
        }

        Vector2 rawInput = Vector2.zero;
        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            organizeInput(keyboard, ref rawInput);
        }

        input = rawInput.normalized;

        // Só muda a direção pelo movimento se não estiver no meio de um ataque
        if (input != Vector2.zero && !IsAimLocked)
        {
            FacingDirection = input;

            if (sprite && Mathf.Abs(input.x) > 0.01f)
            {
                sprite.flipX = input.x < 0f;
            }
        }

        UpdateAnimator();
    }

    void organizeInput(Keyboard keyboard, ref Vector2 rawInput)
    {
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) rawInput.y += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) rawInput.y -= 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) rawInput.x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) rawInput.x += 1f;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetFloat("MoveX", FacingDirection.x);
        animator.SetFloat("MoveY", FacingDirection.y);
        animator.SetBool("isMoving", input != Vector2.zero);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * moveSpeed;
    }

    // lockDuration > 0: mantém essa direção (mira do ataque) por esse tempo, ignorando o movimento.
    public void SetFacingDirection(Vector2 direction, float lockDuration = 0f)
    {
        if (direction == Vector2.zero) return;

        FacingDirection = direction.normalized;
        aimLockUntil = Time.time + lockDuration;

        if (sprite && Mathf.Abs(FacingDirection.x) > 0.01f)
        {
            sprite.flipX = FacingDirection.x < 0f;
        }

        UpdateAnimator();
    }
}