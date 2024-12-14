using System;
using UnityEngine;

public interface ICharacterInputHandler
{
    public Action OnRollInput { get; set; }
    public Action<Vector2> OnDirectToInput { get; set; }
    public Action OnMoveInput { get; set; }
    public Action OnMoveCancel { get; set; }
    public Action OnJumpInput { get; set; }
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
}

public interface IState
{
    public string type { get; set; }
    void Enter();
    void Update();
    void Tick();
    void Exit();
}

public interface IWeapon : IInitializable
{

}

public interface ICollectable
{
    void Collect(ICollector collector);
}

public interface ICollector
{
    public Transform CollectPoint { get; set; }
}

public interface IInitializable
{
    public void Initialize();
}

public interface IDamagable
{
    public bool isDead { get; set; }
    Action OnTakingDamage { get; set; }
    void takeDamage();
}

public interface IInteractable
{
    void Interact(IInteractable interactable);
    void EndInteraction();
}

public interface IInteractor
{
    void InjectState(IState state);
}

public interface IMeleeWeapon : IWeapon
{

}

public interface IRangedWeapon : IWeapon
{
}

public interface ICharacterAnimatorController
{
    Action<bool> OnSettingRootMotion { get; set; }
    void OnAttackExit();
    void EndAttackAnimation();
    void OnUpdate();
    void OnAnimatorMove();
    void EnableLayer();
    void DisableLayer();
}

public interface ICharacterAnimatorManager
{
    Action OnReattackable { get; set; }
    Action OnAttackAnimationCompleted { get; set; }
    Action OnFireAnimationCompleted { get; set; }
    Action OnReloadAnimationComplete { get; set; }
    void SetCharAnimatorController(ICharacterAnimatorController animatorController);
    void SetRuntimeAnimatorController(RuntimeAnimatorController runtimeController);
}

public interface IMover
{
    void Move();
}