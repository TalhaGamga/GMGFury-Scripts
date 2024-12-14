using UnityEngine;
public class Idle : IState
{
    public string type { get { return "Idle"; } set { } }

    private MovementContext context;

    public Idle(CharacterData data)
    {
        context = data.movementContext;
    }

    public void Enter()
    {
        context.horizontalSpeed = 0;
        context.isIdling = true;
    }

    public void Update()
    {
    }

    public void Tick()
    {
    }

    public void Exit()
    {
        context.isIdling = false;
    }
}