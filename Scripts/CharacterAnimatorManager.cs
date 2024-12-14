using System;
using UnityEngine;

// Responsible from manaing booleans and storing-assigning animation events.
public class CharacterAnimatorManager : MonoBehaviour, ICharacterAnimatorManager
{
    public Action OnAttackAnimationCompleted { get; set; }
    public Action OnReattackable { get; set; }
    public Action OnFireAnimationCompleted { get; set; }
    public Action OnReloadAnimationComplete { get; set; }

    public Action OnRootMotionActivated;
    public Action OnRootMotionDeactivated;
    public Action OnAttacked;
    public Action OnUppercutPunched;
    public Action OnDownercutPunched;
    public Action OnShoryuken;

    [SerializeField] private Animator animator;
    private ICharacterAnimatorController animatorController;

    private void OnDisable()
    {
        if (animatorController == null) return;
        animatorController.OnSettingRootMotion -= SetRootMotion;
    }

    void Update()
    {
        animatorController?.OnUpdate();
    }

    private void OnAnimatorMove()
    {
        animatorController?.OnAnimatorMove();
    }

    public void SetRuntimeAnimatorController(RuntimeAnimatorController runtimeController)
    {
        animator.runtimeAnimatorController = runtimeController;
    }

    public void SetCharAnimatorController(ICharacterAnimatorController newAnimatorController)
    {
        animatorController = newAnimatorController;
        animatorController.OnSettingRootMotion += SetRootMotion;
    }

    public void PistolWorn()
    {
        animator.SetBool("PistolWorn", true);
    }

    public void Refirable()
    {
        animator.SetBool("Firable", true);
        animator.SetBool("Fired", false);
    }

    public void Reloaded()
    {
        animator.SetBool("Reloaded", true);
    }

    public void RifleWorn()
    {
        animator.SetBool("RifleWorn", true);
    }

    public void SetReattackable()
    {
        OnReattackable?.Invoke();
    }

    public void AttackCompleted()
    {
        animator.SetBool("AttackCompleted", true);
        animator.SetBool("Reattackable", false);
        OnAttackAnimationCompleted?.Invoke();
    }

    public void AttackCompletedReset()
    {
        animator.SetBool("AttackCompleted", false);
    }

    public void OnAttackEntered()
    {
        animator.SetBool("OnAttack", true);
    }

    public void SetRootMotion(bool isOnRoot)
    {
        animator.applyRootMotion = isOnRoot;

        if (isOnRoot)
        {
            OnRootMotionActivated?.Invoke();
        }

        else
        {
            OnRootMotionDeactivated?.Invoke();
        }
    }

    public Vector3 GetDeltaPosition()
    {
        return animator.deltaPosition;
    }

    public void InvokeOnDirectPunched()
    {
        OnAttacked?.Invoke();
    }

    public void InvokeOnUppercutPunched()
    {
        OnAttacked?.Invoke();
    }
    public void InvokeOnDownercutPunched()
    {
        OnAttacked?.Invoke();
    }
    public void InvokeOnShoryukenPunch()
    {
        OnAttacked?.Invoke();
    }

    public void InvokeOnShoryukenKick()
    {
        OnAttacked?.Invoke();
    }

    public void DealAttack()
    {
        OnAttacked?.Invoke();
    }

    public void FireCompleted()
    {
        OnFireAnimationCompleted?.Invoke();
    }

    public void ReloadCompleted()
    {
        OnReloadAnimationComplete?.Invoke();
    }

    public void InvokeOnAttacked()
    {
        OnAttacked?.Invoke();
    }

    public void Death()
    {
        GameManager.Instance.OnGameRestart?.Invoke();
        animator.SetBool("isDead", false);
    }
}