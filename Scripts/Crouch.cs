using UnityEngine;

public class Crouch : IState
{
    public string type { get { return "Crouch"; } set { } }

    MovementContext context;
    ICharacterInputHandler inputHandler;

    float crouchingWalkSpeed = 2;

    CapsuleCollider2D collider;
    Vector2 initialSize = new Vector2(0.67f, 2);
    Vector2 initialPos = new Vector2(0, 1);

    Vector2 crouchedSize = new Vector2(0.67f, 1.25f);
    Vector2 crouchedPos = new Vector2(0, 0.5f);
    public Crouch(CharacterData data)
    {
        context = data.movementContext;
        inputHandler = data.inputHandler;

        collider = data.collider;

        inputHandler.OnCrouchInput += crouchInput;
        inputHandler.OnCrouchCancel += crouchCancel;
    }

    public void Enter()
    {
        context.isCrouching = true;
        collider.size = crouchedSize;
        collider.offset = crouchedPos;
    }

    public void Exit()
    {
        context.isCrouching = false;
        collider.size = initialSize;
        collider.offset = initialPos;
    }

    public void Tick()
    {
    }

    public void Update()
    {
        context.horizontalSpeed = context.inputDirection.x * crouchingWalkSpeed;
    }

    private void crouchInput()
    {
        context.crouchInput = true;
    }

    private void crouchCancel()
    {
        context.crouchInput = false;
    }
}