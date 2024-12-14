using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class CharacterInputHandler : MonoBehaviour, ICharacterInputHandler
{
    private PlayerControls playerControls;
    public Action<Vector2> OnDirectToInput { get; set; }
    public Action OnMoveInput { get; set; }
    public Action OnMoveCancel { get; set; }
    public Action OnJumpInput { get; set; }
    public Action OnJumpCancel { get; set; }
    public Action OnRollInput { get; set; }
    public Action OnTimeSlowInput { get; set; }
    public Action OnTriggeringDash { get; set; }
    public Action OnReleasingDash { get; set; }
    public Action OnSpinInput { get; set; }
    public Action OnCrouchInput { get; set; }
    public Action OnCrouchCancel { get; set; }
    public Action OnAttackInput { get; set; }
    public Action OnAttackCancel { get; set; }
    public Action OnReloadInput { get; set; }

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls?.Enable();
    }

    private void OnEnable()
    {
        AddInputActionCallbacks();
    }

    private void OnDisable()
    {
        RemoveInputActionCallbacks();
    }

    private void AddInputActionCallbacks()
    {
        playerControls.Character.Movement.started += inputMovement;
        playerControls.Character.Movement.started += requestMove;

        playerControls.Character.Movement.canceled += inputMovement;

        playerControls.Character.Movement.canceled += cancelMovementInput;

        playerControls.Character.Jump.started += inputJump;
        playerControls.Character.Jump.canceled += cancelJump;

        playerControls.Character.Roll.started += inputRoll;

        playerControls.Character.TimeSlow.started += inputTimeSlow;

        playerControls.Character.Dash.started += settingDash;
        playerControls.Character.Dash.canceled += releasingDash;

        playerControls.Character.Spin.started += spin;

        playerControls.Character.Crouch.started += inputCrouch;
        playerControls.Character.Crouch.canceled += cancelCrouch;

        playerControls.Character.Attack.started += inputAttack;
        playerControls.Character.Attack.canceled += cancelAttack;

        playerControls.Character.Reload.started += reloadInput;
    }

    private void RemoveInputActionCallbacks()
    {
        playerControls.Character.Movement.started -= inputMovement;
        playerControls.Character.Movement.started -= requestMove;

        playerControls.Character.Movement.canceled -= inputMovement;

        playerControls.Character.Movement.canceled -= cancelMovementInput;

        playerControls.Character.Jump.started -= inputJump;
        playerControls.Character.Jump.canceled -= cancelJump;

        playerControls.Character.Roll.started -= inputRoll;

        playerControls.Character.TimeSlow.started -= inputTimeSlow;

        playerControls.Character.Dash.started -= settingDash;
        playerControls.Character.Dash.canceled -= releasingDash;

        playerControls.Character.Spin.started -= spin;

        playerControls.Character.Crouch.started -= inputCrouch;
        playerControls.Character.Crouch.canceled -= cancelCrouch;

        playerControls.Character.Attack.started -= inputAttack;
        playerControls.Character.Attack.canceled -= cancelAttack;

        playerControls.Character.Reload.started -= reloadInput;
    }

    private void inputRoll(InputAction.CallbackContext context)
    {
        OnRollInput?.Invoke();
    }

    private void inputMovement(InputAction.CallbackContext context)
    {
        Vector2 moveDirection = context.ReadValue<Vector2>();
        OnDirectToInput?.Invoke(moveDirection);
    }

    private void cancelMovementInput(InputAction.CallbackContext context)
    {
        OnMoveCancel?.Invoke();
    }

    private void inputJump(InputAction.CallbackContext context)
    {
        OnJumpInput?.Invoke();
    }
    private void cancelJump(InputAction.CallbackContext context)
    {
        OnJumpCancel?.Invoke();
    }

    private void requestMove(InputAction.CallbackContext context)
    {
        OnMoveInput?.Invoke();
    }

    private void inputTimeSlow(InputAction.CallbackContext context)
    {
        OnTimeSlowInput?.Invoke();
    }

    private void settingDash(InputAction.CallbackContext context)
    {
        OnTriggeringDash?.Invoke();
    }

    private void releasingDash(InputAction.CallbackContext context)
    {
        OnReleasingDash?.Invoke();
    }

    private void spin(InputAction.CallbackContext context)
    {
        OnSpinInput?.Invoke();
    }

    private void inputCrouch(InputAction.CallbackContext context)
    {
        OnCrouchInput?.Invoke();
    }
    private void cancelCrouch(InputAction.CallbackContext context)
    {
        OnCrouchCancel?.Invoke();
    }

    private void inputAttack(InputAction.CallbackContext context)
    {
        OnAttackInput?.Invoke();
    }

    private void cancelAttack(InputAction.CallbackContext context)
    {
        OnAttackCancel?.Invoke();
    }

    private void reloadInput(InputAction.CallbackContext context)
    {
        OnReloadInput?.Invoke();
    }
}