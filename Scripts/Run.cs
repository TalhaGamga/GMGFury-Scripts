public class Run : IState
{
    MovementContext context;
    float runSpeed;
    float factor;

    public string type { get { return "Run"; } set { } }

    public Run(CharacterData data)
    {
        context = data.movementContext;
        runSpeed = context.moveSpeed;
        factor = context.moveSpeedFactor;
    }

    public void Enter()
    {
        context.isRunning = true;
    }

    public void Update()
    {
        context.horizontalSpeed = context.inputDirection.x * runSpeed * factor;
    }

    public void Tick()
    {

    }

    public void Exit()
    {
        context.isRunning = false;
    }
}
