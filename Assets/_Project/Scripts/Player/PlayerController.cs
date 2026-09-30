using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;

    public Vector2 FacingDirection { get; private set; } = Vector2.down;

    Rigidbody2D rb;
    SpriteRenderer sprite;
    Animator animator;
    Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        sprite = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (GameManager.InputLocked) { input = Vector2.zero; return; }

        Vector2 rawInput = Vector2.zero;
        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            organizeInput(keyboard, ref rawInput);
        }

        input = rawInput.normalized;

        if (input != Vector2.zero)
        {
            // Guarda a última direção do movimento
            FacingDirection = input;
            
            // Inverte o sprite quando se move para a esquerda (X < 0)
            if (sprite && Mathf.Abs(input.x) > 0.01f)
            {
                sprite.flipX = input.x < 0f;
            }
        }

        // Passa a direção e o estado de movimento para o Animator
        if (animator != null)
        {
            animator.SetFloat("MoveX", FacingDirection.x);
            animator.SetFloat("MoveY", FacingDirection.y);
            animator.SetBool("isMoving", input != Vector2.zero);
        }
    }

    void organizeInput(Keyboard keyboard, ref Vector2 rawInput)
    {
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) rawInput.y += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) rawInput.y -= 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) rawInput.x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) rawInput.x += 1f;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * moveSpeed;
    }
}