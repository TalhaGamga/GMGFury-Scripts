using UnityEngine;

public class Spin : IState
{
    private MovementContext context;
    private Rigidbody2D rb;

    public float maxSpinSpeed = 20f;
    public float acceleration = 1f;

    public string type { get { return "Spin"; } set { } }
    ICharacterInputHandler inputHandler;
    Vector2 forceDirection;

    PhysicsMaterial2D bouncingMat;
    PhysicsMaterial2D nonBouncingMat;
    Collider2D collider;

    public Spin(CharacterData data)
    {
        context = data.movementContext;
        rb = data.rb;
        collider = data.collider;
        inputHandler = data.inputHandler;
        bouncingMat = data.bouncingMat;
        nonBouncingMat = data.nonBouncingMat;

        inputHandler.OnSpinInput += spinInput;
    }

    public void Enter()
    {
        inputHandler.OnSpinInput -= spinInput;
        inputHandler.OnSpinInput += cancelInput;

        context.physicsMovement = true;
        collider.sharedMaterial = bouncingMat;

        context.isSpinning = true;
    }

    public void Exit()
    {
        inputHandler.OnSpinInput += spinInput;
        inputHandler.OnSpinInput -= cancelInput;

        rb.velocity = Vector2.zero;

        context.physicsMovement = false;
        collider.sharedMaterial = nonBouncingMat;

        context.isSpinning = false;
    }

    public void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            forceDirection.x = -1;
            rb.AddForce(forceDirection.normalized, ForceMode2D.Force);
        }

        if (Input.GetKey(KeyCode.D))
        {
            forceDirection.x = 1;
            rb.AddForce(forceDirection.normalized, ForceMode2D.Force);
        }
    }

    private void spinInput()
    {
        context.spinInput = true;
    }

    private void cancelInput()
    {
        context.spinInput = false;
    }

    public void Tick()
    {
    }
}
