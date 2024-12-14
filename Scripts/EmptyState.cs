public class EmptyState : IState
{
    public string type { get { return "Empty"; } set { } }

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