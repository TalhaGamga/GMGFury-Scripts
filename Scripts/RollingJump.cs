using System;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Assets.GAME.Scripts
{
    public class RollingJump : IState
    {
        MovementContext context;
        float jumpSpeed = 8f;
        float moveSpeed;
        float delta = 2f;
        public string type { get { return "RollingJump"; } set { } }

        ICharacterInputHandler inputHandler;

        public Action OnStateEnter;
        public Action OnStateExit;

        public RollingJump(CharacterData data)
        {
            context = data.movementContext;
            inputHandler = data.inputHandler;

            moveSpeed = context.moveSpeed;
        }

        public void Enter()
        {
            context.verticalSpeed = jumpSpeed;

            context.isRollingJumping = true;

            OnStateEnter?.Invoke();
        }

        public void Exit()
        {
            context.isRollingJumping = false;

            OnStateExit?.Invoke();
        }

        public void Tick()
        {
        }

        public void Update()
        {
            context.verticalSpeed += context.gravity * CustomTime.Instance.deltaTime * delta;
            context.horizontalSpeed = context.inputDirection.x * moveSpeed;
        }
    }
}
