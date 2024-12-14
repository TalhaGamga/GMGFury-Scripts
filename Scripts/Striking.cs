using System;
using UnityEngine;

public class Striking : MonoBehaviour, IWeapon
{
    [SerializeField] private StateMachine stateMachine;
    [SerializeField] private static StrikingContext context;
    [SerializeField] private Transform punchDealer;
    [SerializeField] private RuntimeAnimatorController runtimeAnimatorController;
    private StrikingAnimatorController animatorController;

    private void Update()
    {
        stateMachine?.Update();
    }

    public void Initialize()
    {
        CharacterData data = GetComponentInParent<Character>().data;
        context = new StrikingContext();

        animatorController = new StrikingAnimatorController(data);
        CharacterAnimatorManager animatorManager = GetComponentInParent<CharacterAnimatorManager>();
        animatorManager.SetRuntimeAnimatorController(runtimeAnimatorController);
        animatorManager.SetCharAnimatorController(animatorController);

        stateMachine = new StateMachine();

        Attack groundedAttack = new Attack();
        Attack airborneAttack = new Attack();
        Idle idle = new Idle();

        airborneAttack.OnStateEnter += enterAirborneAttack;
        airborneAttack.OnStateExit += endAirborneAttack;
        airborneAttack.OnCombo += triggerAttackAndSetUnattackable;

        groundedAttack.OnStateEnter += enterGroundedAttack;
        groundedAttack.OnStateExit += endGroundedAttack;
        groundedAttack.OnCombo += triggerAttackAndSetUnattackable;

        StateTransition toGroundedAttack = new StateTransition(groundedAttack, () => { return data.isOnGround && !data.isDead; });
        toGroundedAttack.SetOnTransition(() =>
        {
            animatorController.OnSettingRootMotion(true);
            SetIsAirborneAttack(false);
            triggerAttackAndSetUnattackable();
        });

        StateTransition toAirborneAttack = new StateTransition(airborneAttack, () => { return data.movementContext.isRollingJumping && !data.isDead; });
        toAirborneAttack.SetOnTransition(() =>
        {
            animatorController.OnSettingRootMotion(true);
            SetIsAirborneAttack(true);
            triggerAttackAndSetUnattackable();
        });

        StateTransition groundedAttackToIdle = new StateTransition(groundedAttack, idle, () => { return !data.isDead; });
        groundedAttackToIdle.SetOnTransition(() =>
        {
            animatorController.OnSettingRootMotion(false);
        });

        StateTransition groundedAttackToIdleOnContactWithGroundIsLost = new StateTransition(groundedAttack, idle, () => { return !data.isOnGround && !animatorController.GetBool("OnShoryuken") && !data.isDead; }, 0);
        groundedAttackToIdleOnContactWithGroundIsLost.SetOnTransition(() => { animatorController.OnSettingRootMotion(false); });

        StateTransition airborneAttackToIdle = new StateTransition(airborneAttack, idle, () => { return !data.isDead; });
        airborneAttackToIdle.SetOnTransition(() => { animatorController.OnSettingRootMotion(false); });

        Action attackInputAction = null;
        data.inputHandler.OnAttackInput = () => attackInputAction?.Invoke();
        attackInputAction += () => groundedAttack.OnAttackComboRequest?.Invoke();
        attackInputAction += () => airborneAttack.OnAttackComboRequest?.Invoke();

        animatorManager.OnReattackable += () => { context.isAttackable = true; };

        stateMachine.AddAnyTransitionTrigger(ref attackInputAction, toGroundedAttack);
        stateMachine.AddAnyTransitionTrigger(ref attackInputAction, toAirborneAttack);

        stateMachine.AddAnyTransition(groundedAttackToIdleOnContactWithGroundIsLost);

        Action attackAnimationCompleted = null;
        animatorManager.OnAttackAnimationCompleted += () => attackAnimationCompleted?.Invoke();
        stateMachine.AddNormalTransitionTrigger(ref attackAnimationCompleted, groundedAttackToIdle);
        stateMachine.AddNormalTransitionTrigger(ref attackAnimationCompleted, airborneAttackToIdle);

        stateMachine.SetState(idle);

        animatorManager.OnAttacked += dealDamage;
    }

    private void dealDamage()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(punchDealer.position, 2f);

        foreach (var collider in colliders)
        {
            if (!collider.gameObject.CompareTag("Player"))
            {
                if (collider.TryGetComponent<IDamagable>(out IDamagable damagable))
                {
                    damagable.takeDamage();
                }
            }
        }
    }

    private void triggerAttackAndSetUnattackable()
    {
        if (context.isAttackable)
        {
            animatorController.TriggerAttack();
            context.isAttackable = false;
        }
    }

    private void SetIsAirborneAttack(bool isAirborne)
    {
        animatorController.SetIsAirborne(isAirborne);
    }

    private void enterGroundedAttack()
    {
    }

    private void enterAirborneAttack()
    {
    }

    private void endAirborneAttack()
    {
        context.isAttackable = true;

        animatorController.EndAttackAnimation();
    }

    private void endGroundedAttack()
    {
        context.isAttackable = true;

        animatorController.EndAttackAnimation();
    }

    private class Attack : IState
    {
        public string type { get { return "Attack"; } set { } }

        public Action OnStateEnter;
        public Action OnStateExit;
        public Action OnAttackComboRequest;
        public Action OnCombo;

        public void Enter()
        {
            OnStateEnter?.Invoke();

            OnAttackComboRequest += ComboAttack;
        }

        public void Exit()
        {
            OnStateExit?.Invoke();

            OnAttackComboRequest -= ComboAttack;
        }

        public void Tick()
        {
        }

        public void Update()
        {
        }

        public void LoadData(StrikingAttackSO data)
        {
            data.CallData();
        }

        private void ComboAttack()
        {
            OnCombo?.Invoke();
        }
    }

    private class Idle : IState
    {
        public string type { get { return "Idle"; } set { } }

        public void Enter()
        {

        }

        public void Exit()
        {

        }

        public void Tick()
        {
        }

        public void Update()
        {
        }
    }

    private class StrikingAnimatorController : SimpleCharacterAnimatorController
    {
        public StrikingAnimatorController(CharacterData data) : base(data)
        {
        }

        public void TriggerAttack()
        {
            animator.SetTrigger("AttackTrigger");
        }

        public override void EndAttackAnimation()
        {
            animator.SetBool("OnAttack", false);
        }

        public void SetRootMotion(bool isOnRoot)
        {
            OnSettingRootMotion?.Invoke(isOnRoot);
        }

        public void SetIsAirborne(bool isAirborne)
        {
            animator.SetBool("IsAirborne", isAirborne);
        }

        public bool GetBool(string param)
        {
            return animator.GetBool(param);
        }

        public override void OnAttackExit()
        {
        }

        public override void EnableLayer()
        {
        }

        public override void DisableLayer()
        {
        }

        public override void OnAnimatorMove()
        {
        }
    }

    [System.Serializable]
    private class StrikingContext
    {
        public bool isAttackable = true;
    }
}