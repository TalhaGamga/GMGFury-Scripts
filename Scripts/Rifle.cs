using System;
using Unity.VisualScripting;
using UnityEngine;

public class Rifle : MonoBehaviour, IRangedWeapon
{
    [SerializeField] private StateMachine stateMachine;
    [SerializeField] private RifleContext context;
    [SerializeField] private Transform aimPoint;
    [SerializeField] private RuntimeAnimatorController rifleAnimator;

    private RifleAnimatorController animatorController;

    public void Initialize()
    {
        CharacterData data = GetComponentInParent<Character>().data;
        animatorController = new RifleAnimatorController(data);
        CharacterAnimatorManager animatorManager = GetComponentInParent<CharacterAnimatorManager>();
        animatorManager.SetCharAnimatorController(animatorController);
        animatorManager.SetRuntimeAnimatorController(rifleAnimator);

        stateMachine = new StateMachine();

        Idle idle = new Idle();
        Fire fire = new Fire();
        Reload reload = new Reload();

        StateTransition toReloadNecessarily = new StateTransition(reload, () => { return context.currentAmmoCount <= 0; }, 0);
        StateTransition toReloadIntentionally = new StateTransition(reload, () => { return true; });
        StateTransition idleToFire = new StateTransition(idle, fire, () => { return true; });
        StateTransition reloadToIdle = new StateTransition(reload, idle, () => { return !context.attackInput; });
        StateTransition fireToIdle = new StateTransition(fire, idle, () => { return context.currentAmmoCount > 0; });
        StateTransition reloadToFire = new StateTransition(reload, fire, () => { return context.attackInput; });

        Action fireInputAction = null;
        data.inputHandler.OnAttackInput = () =>
        {
            fireInputAction?.Invoke();
            context.attackInput = true;
        };

        Action fireCancelAction = null;
        data.inputHandler.OnAttackCancel = () =>
        {
            fireCancelAction?.Invoke();
            context.attackInput = false;
        };

        Action reloadInputAction = null;
        data.inputHandler.OnReloadInput = () => { reloadInputAction?.Invoke(); };

        Action reloadAnimationCompleted = null;
        animatorManager.OnReloadAnimationComplete = () => { reloadAnimationCompleted?.Invoke(); };

        stateMachine.AddNormalTransitionTrigger(ref fireInputAction, idleToFire);
        stateMachine.AddNormalTransitionTrigger(ref fireCancelAction, fireToIdle);
        stateMachine.AddAnyTransitionTrigger(ref reloadInputAction, toReloadIntentionally);
        stateMachine.AddNormalTransitionTrigger(ref reloadAnimationCompleted, reloadToIdle);
        stateMachine.AddNormalTransitionTrigger(ref reloadAnimationCompleted, reloadToFire);

        fire.OnStateEnter = () => { animatorController.TriggerFire(); };
        fire.OnStateExit = () => { animatorController.EndFire(); };

        reload.OnStateEnter = () => { animatorController.TriggerReload(); };

        stateMachine.SetState(idle);
    }

    private void Update()
    {
        stateMachine?.Update();
    }

    private class Idle : IState
    {
        public string type { get { return "Idle"; } set { } }

        public Action OnStateEnter;
        public Action OnStateExit;

        public Idle()
        {

        }

        public void Enter()
        {
            Debug.Log("Idle ente");
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

    private class Fire : IState
    {
        public string type { get { return "Fire"; } set { } }

        public Action OnStateEnter;
        public Action OnStateExit;

        public void Enter()
        {
            OnStateEnter?.Invoke();
        }

        public void Exit()
        {
            OnStateExit?.Invoke();
        }

        public void Tick()
        {
        }

        public void Update()
        {
        }
    }

    private class Reload : IState
    {
        public string type { get { return "Reload"; } set { } }

        public Action OnStateEnter;
        public Action OnStateExit;

        public void Enter()
        {
            OnStateEnter?.Invoke();
        }

        public void Exit()
        {
            OnStateExit?.Invoke();
        }

        public void Tick()
        {
        }

        public void Update()
        {
        }
    }

    private class RifleAnimatorController : SimpleCharacterAnimatorController
    {
        public RifleAnimatorController(CharacterData data) : base(data)
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

        public override void EndAttackAnimation()
        {
        }


        public override void OnAttackExit()
        {
        }

        public void TriggerFire()
        {
            animator.SetTrigger("Fire");
            animator.ResetTrigger("EndFire");
        }

        public void EndFire()
        {
            animator.SetTrigger("EndFire");
            animator.ResetTrigger("Fire");
        }

        public void TriggerReload()
        {
            animator.SetTrigger("Reload");
        }
    }

    [System.Serializable]
    private class RifleContext
    {
        public int ammoCapacity;
        public int currentAmmoCount;
        public bool attackInput;
    }
}