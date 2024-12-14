using System;
using UnityEngine;

public class Roll : IState
{
    public string type { get { return "Roll"; } set { } }

    private MovementContext context;
    private float xDirection;
    private float rollDuration = 0.25f;
    private float rollSpeed = 15.0f;
    private float deltaSpeed = 4f;
    private ICharacterInputHandler inputHandler;
    private CapsuleCollider2D collider;
    private Action cutJumpingAction;
    private Vector2 initialSize = new Vector2(0.67f, 2);
    private Vector2 initialPos = new Vector2(0, 1);
    private Vector2 rollSize = new Vector2(0.67f, 1.25f);
    private Vector2 rollPos = new Vector2(0, 0.5f);

    public Roll(CharacterData data)
    {
        context = data.movementContext;
        inputHandler = data.inputHandler;

        inputHandler.OnRollInput = triggerRoll;
        collider = data.collider;
    }

    public void Enter()
    {
        xDirection = context.lookDirection.x;
        rollDuration = 0.25f;
        context.isRolling = true;

        collider.size = rollSize;
        collider.offset = rollPos;

        if (context.verticalSpeed < 0)
        {
            context.verticalSpeed = 0;
        }
    }

    public void Exit()
    {
        collider.size = initialSize;
        collider.offset = initialPos;

        cutJumpingAction = null;
    }

    public void Tick()
    {
    }

    public void Update()
    {
        rollDuration -= Time.deltaTime;

        if (rollDuration < 0)
        {
            context.isRolling = false;
            context.rollInput = false;
        }

        context.horizontalSpeed = xDirection * rollSpeed;

        if (context.verticalSpeed > 0)
        {
            context.verticalSpeed += context.gravity * CustomTime.Instance.deltaTime * deltaSpeed;
        }
    }

    private void triggerRoll()
    {
        context.rollInput = true;
    }
}