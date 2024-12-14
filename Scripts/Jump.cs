using System;
using UnityEngine;

public class Jump : IState
{
    public string type { get { return "Jump"; } set { } }

    private MovementContext context;
    private float jumpSpeed = 12f;
    private float moveSpeed;

    float delta = 2;

    Action cutJumpingAction;

    public Jump(CharacterData data)
    {
        context = data.movementContext;

        moveSpeed = context.moveSpeed;

        data.inputHandler.OnJumpCancel += () => { cutJumpingAction?.Invoke(); };
    }

    public void Enter()
    {
        context.verticalSpeed = jumpSpeed;

        context.isJumping = true;

        cutJumpingAction = cutJumping;
    }

    public void Exit()
    {
        context.isJumping = false;

        cutJumpingAction = null;

        delta = 2;
    }

    public void Tick()
    {
    }

    public void Update()
    {
        context.verticalSpeed += context.gravity * CustomTime.Instance.deltaTime * delta;

        context.horizontalSpeed = context.inputDirection.x * moveSpeed;
    }

    private void cutJumping()
    {
        delta = 3f;
    }
}
