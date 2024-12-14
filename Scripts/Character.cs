using Assets.GAME.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour, IDamagable, ICollector, IInteractor
{
    public CharacterData data;
    public Action OnTakingDamage { get; set; }
    public Transform CollectPoint { get { return _collectPoint; } set { } }
    public bool isDead { get { return data.isDead; } set { } }

    [SerializeField] private CharacterMovementSystem movementSystem;
    [SerializeField] private Transform _collectPoint;
    [SerializeField] private Vector2 initialPoint;

    private Dictionary<MovementStateType, IState> movementStates;
    private LayerMask groundLayer;
    private bool isGrounded;
    private Transform groundSensorPoint;

    private void Awake()
    {
        InitializeCharacterData();
        InitializeMovementSystem();

        groundLayer = data.groundLayer;
        groundSensorPoint = data.groundSensorPoint;

        GetComponentInChildren<IInitializable>().Initialize();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGameRestart += restartCharacter;
    }

    private void OnDisable()
    {
        movementSystem.End();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameRestart -= restartCharacter;
        }
    }

    private void Update()
    {
        movementSystem.Update();

        checkGround();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<ICollectable>(out ICollectable collectable))
        {
            collectable.Collect(this);
        }
    }

    void InitializeCharacterData()
    {
        data.inputHandler = GetComponent<ICharacterInputHandler>();
        data.animator = GetComponentInChildren<Animator>();

        data.movementContext = new MovementContext();
        data.combatContext = new CombatContext();
    }

    void InitializeMovementSystem()
    {
        movementSystem = new CharacterMovementSystem(data);
        movementSystem.Initialize();

        Idle idle = new Idle(data);
        Run run = new Run(data);
        Jump jump = new Jump(data);
        Fall fall = new Fall(data);
        Roll roll = new Roll(data);
        Crouch crouch = new Crouch(data);
        RollingJump rollingJump = new RollingJump(data);

        movementStates = new Dictionary<MovementStateType, IState>();

        MovementContext movementContext = data.movementContext;

        StateTransition toJump = new StateTransition(jump, () => { return isGrounded && !movementContext.isCrouching && !data.isDead; });
        StateTransition toCrouch = new StateTransition(crouch, () => { return isGrounded && !movementContext.isJumping; });
        StateTransition crouchBuffer = new StateTransition(crouch, () => { return movementContext.isCrouching && movementContext.crouchInput && !data.isDead; }, 2); // Continues crouching if the character is crouching and inputting crouch.
        StateTransition toRoll = new StateTransition(roll, () => { return true; });
        StateTransition toRun = new StateTransition(run, () => { return isGrounded && Mathf.Abs(movementContext.inputDirection.x) > 0; }, 1);

        StateTransition crouchToRollingJump = new StateTransition(crouch, rollingJump, () => { return true; });

        StateTransition rollingJumpToFall = new StateTransition(rollingJump, fall, () => { return movementContext.verticalSpeed < 0 && !data.isDead; }, 3);

        StateTransition toFall = new StateTransition(fall, () => { return !(movementContext.isJumping) && !isGrounded && !data.isDead; }, 2);

        StateTransition rollBuffer = new StateTransition(roll, () => { return movementContext.isRolling && !data.isDead; }, 3); // Continues rolling if it's in roll state.

        StateTransition toIdle = new StateTransition(idle, () => { return isGrounded && movementContext.inputDirection.magnitude == 0 && !data.isDead; }, 1);

        StateTransition idleToRun = new StateTransition(idle, run, () => { return Mathf.Abs(movementContext.inputDirection.x) > 0 && !data.isDead; }, 1);
        StateTransition runToIdle = new StateTransition(run, idle, () => { return Mathf.Abs(movementContext.inputDirection.magnitude) == 0 && !data.isDead; }, 1);
        StateTransition jumpToFall = new StateTransition(jump, fall, () => { return movementContext.verticalSpeed < 2 && !data.isDead; }, 1);
        StateTransition fallToRun = new StateTransition(fall, run, () => { return isGrounded && Mathf.Abs(movementContext.inputDirection.x) > 0; }, 0);


        movementSystem.AddNormalTransition(idleToRun);
        movementSystem.AddNormalTransition(runToIdle);
        movementSystem.AddNormalTransition(jumpToFall);
        movementSystem.AddNormalTransition(rollingJumpToFall);
        movementSystem.AddNormalTransition(fallToRun);

        movementSystem.AddAnyTransition(toIdle);
        movementSystem.AddAnyTransition(toFall);
        movementSystem.AddAnyTransition(rollBuffer);
        movementSystem.AddAnyTransition(crouchBuffer);
        movementSystem.AddAnyTransition(toRun);

        Action jumpInputAction = null;
        data.inputHandler.OnJumpInput += () => jumpInputAction?.Invoke();
        movementSystem.AddAnyTransitionTrigger(ref jumpInputAction, toJump);

        Action crouchInputAction = null;
        data.inputHandler.OnCrouchInput += () => crouchInputAction?.Invoke();
        movementSystem.AddAnyTransitionTrigger(ref crouchInputAction, toCrouch);

        Action rollInputAction = null;
        data.inputHandler.OnRollInput += () => rollInputAction?.Invoke();
        movementSystem.AddAnyTransitionTrigger(ref rollInputAction, toRoll);

        movementSystem.AddNormalTransitionTrigger(ref jumpInputAction, crouchToRollingJump);

        movementSystem.SetState(idle);

        CharacterAnimatorManager animatorManager = GetComponentInChildren<CharacterAnimatorManager>();
        movementSystem.GetDeltaPositionFunc += animatorManager.GetDeltaPosition;

        animatorManager.OnRootMotionActivated += () => movementSystem.SetMoverAction?.Invoke("rootMover");
        animatorManager.OnRootMotionActivated += () => movementSystem.DropSystemAction?.Invoke(true);

        animatorManager.OnRootMotionDeactivated += () => movementSystem.SetMoverAction?.Invoke("directMover");
        animatorManager.OnRootMotionDeactivated += () => movementSystem.DropSystemAction?.Invoke(false);
    }

    private void checkGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundSensorPoint.position, 0.3f, groundLayer);

        data.isOnGround = isGrounded;
    }

    public bool AddMovementState(MovementStateType type, IState stateToAdd)
    {
        if (!movementStates.ContainsKey(type))
        {
            movementStates.Add(type, stateToAdd);
            return true;
        }

        return false;
    }

    public bool AddMovementStateTransition(MovementStateType from, MovementStateType to, Func<bool> condition, Action onTransitionAction = null, int priority = 0)
    {
        if (from == MovementStateType.None)
        {
            if (movementStates.TryGetValue(to, out IState toState))
            {
                StateTransition transition = new StateTransition(toState, condition, priority);

                if (onTransitionAction != null)
                {
                    transition.SetOnTransition(onTransitionAction);
                }

                movementSystem.AddAnyTransition(transition);

                return true;

            }
        }

        else
        {
            if (movementStates.TryGetValue(from, out IState fromState) && movementStates.TryGetValue(to, out IState toState))
            {
                StateTransition transition = new StateTransition(fromState, toState, condition, priority);

                if (onTransitionAction != null)
                {
                    transition.SetOnTransition(onTransitionAction);
                }

                movementSystem.AddNormalTransition(transition);

                return true;
            }
        }

        return false;
    }


    public void takeDamage()
    {
        data.hp--;
        if (data.hp <= 0)
        {
            data.isDead = true;
        }

        OnTakingDamage?.Invoke();
    }

    public void InjectState(IState state)
    {

    }

    private void restartCharacter()
    {
        data.isDead = false;
        transform.position = initialPoint;
        data.hp = 3f;
    }
}

// CrossFade

[System.Serializable]
public class CharacterData
{
    public Transform charTransform;
    public Transform modelTransform;
    public ICharacterInputHandler inputHandler;
    public Rigidbody2D rb;
    public CapsuleCollider2D collider;
    public PhysicsMaterial2D bouncingMat;
    public PhysicsMaterial2D nonBouncingMat;
    public Transform groundSensorPoint;
    public LayerMask groundLayer;
    public Animator animator;
    public MovementContext movementContext;
    public CombatContext combatContext;
    public bool isOnGround;
    public float hp;
    public bool isDead;
}