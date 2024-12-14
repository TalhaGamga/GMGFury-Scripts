using System;
using UnityEngine;

public abstract class SimpleCharacterAnimatorController : ICharacterAnimatorController
{
    protected MovementContext movementContext;
    protected CombatContext combatContext;
    protected Animator animator;

    public Action<bool> OnSettingRootMotion { get; set; }

    public SimpleCharacterAnimatorController(CharacterData data)
    {
        movementContext = data.movementContext;
        animator = data.animator;
    }

    public abstract void OnAttackExit();

    public abstract void EndAttackAnimation();

    public virtual void OnUpdate()
    {
        animator.SetBool("Idling", movementContext.isIdling);
        animator.SetBool("Running", movementContext.isRunning);
        animator.SetBool("Jumping", movementContext.isJumping);
        animator.SetBool("RollingJumping", movementContext.isRollingJumping);
        animator.SetBool("Falling", movementContext.isFalling);
        animator.SetBool("Rolling", movementContext.isRolling);
        animator.SetBool("Dashing", movementContext.isDashing);
        animator.SetBool("Crouching", movementContext.isCrouching && movementContext.horizontalSpeed == 0);
        animator.SetBool("CrouchedWalking", movementContext.isCrouching && Mathf.Abs(movementContext.horizontalSpeed) > 0);
        animator.SetBool("Spinning", movementContext.isSpinning);
    }

    public abstract void EnableLayer();

    public abstract void DisableLayer();

    public abstract void OnAnimatorMove();
}