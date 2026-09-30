using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerCombat : MonoBehaviour
{
    public enum WeaponType { Melee, Ranged }

    [Header("Configurações de Armas")]
    public WeaponType currentWeapon = WeaponType.Melee;

    [Header("Referências Visuais/Objetos")]
    public GameObject meleeObject;  // Objeto/Hitbox da faca/soco
    public GameObject gunObject;    // Objeto da arma (Gun)
    [Tooltip("Opcional: pivô (objeto vazio no centro do Player) que gira na direção do golpe. " +
             "Coloque o meleeObject como filho dele, deslocado à frente.")]
    public Transform meleePivot;

    [Header("Mira durante o ataque")]
    [Tooltip("Quanto tempo a direção do golpe fica travada. Ideal: duração do clipe Punch.")]
    public float meleeAimLock = 0.3f;
    [Tooltip("Ideal: duração do clipe Shoot.")]
    public float rangedAimLock = 0.25f;

    PlayerController controller;
    MeleeAttack melee;
    RangedWeapon ranged;
    Animator animator;
    Camera cam;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        melee = GetComponentInChildren<MeleeAttack>(true);
        ranged = GetComponentInChildren<RangedWeapon>(true);
        animator = GetComponentInChildren<Animator>();
        cam = Camera.main;
    }

    void Start()
    {
        UpdateWeaponVisuals();
    }

    void Update()
    {
        if (GameManager.InputLocked) return;

        HandleWeaponSwitching();
        HandleAttackInput();
    }

    void HandleWeaponSwitching()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        var gamepad = Gamepad.current;

        // 1. Troca via TECLADO (Teclas 1 e 2 ou TAB)
        if (keyboard != null)
        {
            if (keyboard.digit1Key.wasPressedThisFrame || keyboard.numpad1Key.wasPressedThisFrame)
            {
                SelectWeapon(WeaponType.Melee);
            }
            else if (keyboard.digit2Key.wasPressedThisFrame || keyboard.numpad2Key.wasPressedThisFrame)
            {
                if (CanUseFirearm()) SelectWeapon(WeaponType.Ranged);
            }
            else if (keyboard.tabKey.wasPressedThisFrame)
            {
                ToggleWeapon();
            }
        }

        // 2. Troca via SCROLL DO MOUSE
        if (mouse != null)
        {
            float scroll = mouse.scroll.ReadValue().y;
            if (scroll > 0.1f)
            {
                SelectWeapon(WeaponType.Melee);
            }
            else if (scroll < -0.1f && CanUseFirearm())
            {
                SelectWeapon(WeaponType.Ranged);
            }
        }

        // 3. Troca via GAMEPAD / CONTROLE
        if (gamepad != null)
        {
            bool togglePressed = gamepad.buttonNorth.wasPressedThisFrame ||
                                 gamepad.leftShoulder.wasPressedThisFrame ||
                                 gamepad.rightShoulder.wasPressedThisFrame ||
                                 gamepad.dpad.up.wasPressedThisFrame ||
                                 gamepad.dpad.down.wasPressedThisFrame;

            if (togglePressed)
            {
                ToggleWeapon();
            }
        }
    }

    void ToggleWeapon()
    {
        if (currentWeapon == WeaponType.Melee && CanUseFirearm())
        {
            SelectWeapon(WeaponType.Ranged);
        }
        else
        {
            SelectWeapon(WeaponType.Melee);
        }
    }

    void SelectWeapon(WeaponType type)
    {
        if (type == WeaponType.Ranged && !CanUseFirearm()) return;

        currentWeapon = type;
        UpdateWeaponVisuals();
    }

    bool CanUseFirearm()
    {
        return GameManager.Instance != null && GameManager.Instance.HasFirearm;
    }

    void UpdateWeaponVisuals()
    {
        if (meleeObject != null) meleeObject.SetActive(currentWeapon == WeaponType.Melee);
        if (gunObject != null) gunObject.SetActive(currentWeapon == WeaponType.Ranged);
    }

    // Direção da mira: analógico direito (controle) > mouse > última direção do personagem.
    // Não depende do WeaponAim, então funciona mesmo com a arma de fogo desativada.
    Vector2 GetAimDirection()
    {
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            Vector2 stick = gamepad.rightStick.ReadValue();
            if (stick.sqrMagnitude > 0.25f) return stick.normalized;
        }

        var mouse = Mouse.current;
        if (mouse != null && cam != null)
        {
            Vector3 mouseWorld = cam.ScreenToWorldPoint(mouse.position.ReadValue());
            Vector2 dir = (Vector2)mouseWorld - (Vector2)transform.position;
            if (dir.sqrMagnitude > 0.01f) return dir.normalized;
        }

        return controller.FacingDirection;
    }

    void AimMeleeVisual(Vector2 direction)
    {
        if (meleePivot == null) return;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        meleePivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void HandleAttackInput()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        var gamepad = Gamepad.current;

        bool attackInput = (keyboard != null && (keyboard.jKey.wasPressedThisFrame || keyboard.kKey.wasPressedThisFrame)) ||
                           (mouse != null && (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame)) ||
                           (gamepad != null && (gamepad.rightTrigger.wasPressedThisFrame || gamepad.buttonWest.wasPressedThisFrame));

        if (!attackInput) return;

        Vector2 aimDirection = GetAimDirection();

        if (currentWeapon == WeaponType.Melee)
        {
            // Se o facão está em cooldown, nada acontece (nem animação, nem troca de direção)
            if (melee != null && !melee.TryAttack(aimDirection)) return;

            AimMeleeVisual(aimDirection);
            controller.SetFacingDirection(aimDirection, meleeAimLock);
            if (animator != null) animator.SetTrigger("Punch");
        }
        else
        {
            // Em recarga ou cooldown: não toca a animação de tiro
            if (ranged != null && !ranged.TryShoot(aimDirection)) return;

            controller.SetFacingDirection(aimDirection, rangedAimLock);
            if (animator != null) animator.SetTrigger("Shoot");
        }
    }
}