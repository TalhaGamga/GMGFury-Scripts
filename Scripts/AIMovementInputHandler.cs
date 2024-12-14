using System;
using UnityEngine;

public class AIMovementInputHandler : MonoBehaviour, ICharacterInputHandler
{
    public Action<Vector2> OnDirectToInput { get; set; }
    public Action OnMoveCancel { get; set; }
    public Action<Vector2> OnMovePerformed { get; set; }
    public Action OnJumpInput { get; set; }
    public Action OnRollInput { get; set; }
    public Action OnMoveInput { get; set; }
    public Action OnJumpCancel { get; set; }
    public Action OnTimeSlowInput { get; set; }
    public Action OnTriggeringDash { get; set; }
    public Action OnReleasingDash { get; set; }
    public Action OnSpinInput { get; set; }
    public Action OnCrouchInput { get; set; }
    public Action OnCrouchCancel { get; set; }
    public Action OnAttackInput { get; set; }
    public Action OnAttackCancel { get; set; }
    public Action OnReloadInput { get; set; }

    public void AddInputActions()
    {
    }

    public void RemoveInputActions()
    {
    }
}
