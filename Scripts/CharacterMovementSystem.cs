using System;
using UnityEngine;

[System.Serializable]
public class CharacterMovementSystem
{
    public Func<Vector3> GetDeltaPositionFunc;
    public Action<string> SetMoverAction;
    public Action<bool> DropSystemAction;

    [SerializeField] StateMachine stateMachine;

    private MovementContext context;
    private Action performMove;
    private Rigidbody2D rb;
    private Transform orientation;
    [SerializeField] private bool _isTransitionable;

    IState empty;

    public CharacterMovementSystem(CharacterData data)
    {
        stateMachine = new StateMachine();
        rb = data.rb;
        context = data.movementContext;
        orientation = data.modelTransform;
        data.inputHandler.OnDirectToInput += setInputDirection;
        empty = new EmptyState();
    }

    public void Update()
    {
        stateMachine?.Update();

        performMove();
    }

    public void Initialize()
    {
        performMove = directMove;

        SetMoverAction = setMover;
        DropSystemAction = dropSystemMachine;

        context.lookDirection = Vector2.right;
    }

    public void End()
    {
    }

    public void AddNormalTransition(StateTransition stateTransition)
    {
        stateMachine.AddNormalTransition(stateTransition);
    }

    public void AddAnyTransition(StateTransition anyTransition)
    {
        stateMachine.AddAnyTransition(anyTransition);
    }

    public void AddAnyTransitionTrigger(ref Action action, StateTransition anyTransition)
    {
        stateMachine.AddAnyTransitionTrigger(ref action, anyTransition);
    }

    public void AddNormalTransitionTrigger(ref Action action, StateTransition anyTransition)
    {
        stateMachine.AddNormalTransitionTrigger(ref action, anyTransition);
    }

    private void setInputDirection(Vector2 inputDirection)
    {
        context.inputDirection = inputDirection;
        context.lookDirection.y = inputDirection.y;

        if (inputDirection.magnitude > 0)
        {
            context.lookDirection.x = inputDirection.x;
        }
    }

    private void directMove()
    {
        if (!context.physicsMovement)
        {
            rb.velocity = context.movementVelocity * CustomTime.Instance.timeFactor;
            orientation.rotation = Quaternion.Euler(0, context.lookDirection.x * 90, 0);
        }
    }

    private void rootMove()
    {
        Vector3 velocity = GetDeltaPositionFunc.Invoke() / Time.deltaTime;
        rb.velocity = velocity;

        orientation.rotation = Quaternion.Euler(0, context.lookDirection.x * 90, 0);
    }

    private void setMover(string mover)
    {
        if (mover == "directMover")
        {
            performMove = directMove;
            rb.gravityScale = 9.8f;
        }

        if (mover == "rootMover")
        {
            performMove = rootMove;

            context.horizontalSpeed = 0;
            context.verticalSpeed = 0;
            rb.gravityScale = 0;
        }
    }

    private void dropSystemMachine(bool isDropped)
    {
        stateMachine.DropMachine(isDropped);
    }

    public void SetState(IState newState)
    {
        stateMachine.SetState(newState);
    }
}

[Serializable]
public class MovementContext
{
    public Transform orientation;
    public float gravity;
    public Vector2 lookDirection;
    public Vector2 inputDirection;
    public float moveSpeedFactor = 1;
    public float moveSpeed = 5;
    public float horizontalSpeed;
    public float verticalSpeed;

    public bool dashReleased;
    public bool rollInput;
    public bool jumpInput;
    public bool crouchInput;
    public bool spinInput;
    public bool rollingJumpInput;

    public bool isJumping;
    public bool isRollingJumping;
    public bool isRunning;
    public bool isFalling;
    public bool isRolling;
    public bool isDashing;
    public bool isSpinning;
    public bool isIdling;
    public bool isCrouching;

    public bool physicsMovement = false;

    public Vector3 movementVelocity { get { return new Vector2(horizontalSpeed, verticalSpeed); } }

    public string currentStateType;

    public MovementContext()
    {
        gravity = -9.18f;
    }
}

public enum MovementStateType
{
    Idle,
    Run,
    Jump,
    Fall,
    Dash,
    Fly,
    Roll,
    Spin,
    Crouch,
    None
}