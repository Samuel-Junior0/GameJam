using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerCombat : MonoBehaviour
{
    public enum WeaponType { Melee, Ranged }

    [Header("Configurações de Armas")]
    public WeaponType currentWeapon = WeaponType.Melee;

    [Header("Referências Visuais/Objetos")]
    public GameObject meleeObject;  // Objeto visual da faca (se houver)
    public GameObject gunObject;    // Objeto visual da arma (se houver)

    PlayerController controller;
    MeleeAttack melee;
    RangedWeapon ranged;
    Animator animator;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        melee = GetComponentInChildren<MeleeAttack>();
        ranged = GetComponentInChildren<RangedWeapon>();
        animator = GetComponentInChildren<Animator>();
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

        // 3. Troca via GAMEPAD / CONTROLE (Y / Triângulo, Bumpers LB/RB, ou D-Pad)
        if (gamepad != null)
        {
            bool togglePressed = gamepad.buttonNorth.wasPressedThisFrame ||  // Botão Y (Xbox) / Triângulo (PlayStation)
                                 gamepad.leftShoulder.wasPressedThisFrame || // LB / L1
                                 gamepad.rightShoulder.wasPressedThisFrame ||// RB / R1
                                 gamepad.dpad.up.wasPressedThisFrame ||       // D-Pad Para Cima
                                 gamepad.dpad.down.wasPressedThisFrame;      // D-Pad Para Baixo

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
        // Ativa/desativa os GameObjects das armas conforme a seleção
        if (meleeObject != null) meleeObject.SetActive(currentWeapon == WeaponType.Melee);
        if (gunObject != null) gunObject.SetActive(currentWeapon == WeaponType.Ranged);
    }

    void HandleAttackInput()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        var gamepad = Gamepad.current;

        // Aceita Teclado (J/K), Mouse (Esquerdo/Direito) ou Gamepad (Gatilhos RT/RB e Botão X/A)
        bool attackInput = (keyboard != null && (keyboard.jKey.wasPressedThisFrame || keyboard.kKey.wasPressedThisFrame)) ||
                          (mouse != null && (mouse.leftButton.wasPressedThisFrame || mouse.rightButton.wasPressedThisFrame)) ||
                          (gamepad != null && (gamepad.rightTrigger.wasPressedThisFrame || gamepad.buttonWest.wasPressedThisFrame));

        if (!attackInput) return;

        if (currentWeapon == WeaponType.Melee)
        {
            // Dispara o Trigger de facada/soco
            if (animator != null) animator.SetTrigger("Punch");

            if (melee != null) melee.TryAttack(controller.FacingDirection);
        }
        else if (currentWeapon == WeaponType.Ranged)
        {
            // Dispara o Trigger de tiro
            if (animator != null) animator.SetTrigger("Shoot");

            if (ranged != null) ranged.TryShoot(controller.FacingDirection);
        }
    }
}