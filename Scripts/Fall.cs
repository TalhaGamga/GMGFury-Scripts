using UnityEngine;

public class Fall : IState
{
    public string type { get { return "Fall"; } set { } }

    private MovementContext context;
    private float moveSpeed;
    private float interpolation = 0.075f;
    private float fallSpeed = 1.7f;

    public Fall(CharacterData data)
    {
        context = data.movementContext;

        moveSpeed = context.moveSpeed;
    }

    public void Enter()
    {
        context.isFalling = true;
    }

    public void Exit()
    {
        context.verticalSpeed = 0;
        context.isFalling = false;
    }

    public void Tick()
    {
    }

    public void Update()
    {
        context.verticalSpeed += context.gravity * CustomTime.Instance.deltaTime * fallSpeed;

        if (Mathf.Abs(context.horizontalSpeed) > moveSpeed)
        {
            context.horizontalSpeed = Mathf.Lerp(context.horizontalSpeed, context.inputDirection.x * moveSpeed, interpolation);
        }

        else
        {
            context.horizontalSpeed = context.inputDirection.x * moveSpeed;
        }
    }
}