using System;
using System.Threading.Tasks;
using UnityEngine;

public class Dash : IState
{
    MovementContext context;
    ICharacterInputHandler inputHandler;

    Vector2 dashDirection;
    Vector2 dashInitPoint;

    bool isReleased = false;
    bool isSetting = false;

    readonly float dashDuration = 0.1f;

    float currentDashDuration = 0;
    float speed;

    public Action<Vector2> OnSettingDash;

    public string type { get { return "Dash"; } set { } }

    public Dash(CharacterData data)
    {
        context = data.movementContext;
        inputHandler = data.inputHandler;

        inputHandler.OnTriggeringDash += triggerDashing;

        //OnSettingDash += new DashLine(data.lineRenderer).setDash;
    }

    public void Enter()
    {
        Vector2 releasePoint = Input.mousePosition;
        dashDirection = (releasePoint - dashInitPoint).normalized;
        context.lookDirection.x = (dashDirection.x > 0) ? 1 : -1;

        OnSettingDash?.Invoke(new Vector2(0, 0));

        context.isDashing = true;
    }

    public void Exit()
    {
        inputHandler.OnReleasingDash -= releaseDash;
        reset();
        context.isDashing = false;
    }

    public void Update()
    {
        if (isReleased)
        {
            if (currentDashDuration < dashDuration)
            {
                applyDash();
                return;
            }

            Exit();
        }
    }

    private void triggerDashing()
    {
        isSetting = true;
        dashInitPoint = Input.mousePosition;

        settingDash();

        inputHandler.OnReleasingDash += releaseDash;
    }

    private void releaseDash()
    {
        context.dashReleased = true;
        speed = 50f;

        isReleased = true;
    }


    private void applyDash()
    {
        currentDashDuration += Time.deltaTime;

        context.horizontalSpeed = dashDirection.x * (speed);
        context.verticalSpeed = dashDirection.y * 50f;
    }

    private void reset()
    {
        context.verticalSpeed = 0;
        currentDashDuration = 0;
        context.dashReleased = false;
        isReleased = false;

        context.lookDirection.x = (context.inputDirection.magnitude > 0) ? context.inputDirection.x : context.lookDirection.x;
    }

    private async void settingDash()
    {
        while (isSetting && !isReleased)
        {
            Vector2 currentMousePoint = Input.mousePosition;
            Vector2 currentDashDirection = (dashInitPoint - currentMousePoint).normalized;
            OnSettingDash?.Invoke(currentDashDirection);
            await Task.Delay(1);
        }
    }

    public void Tick()
    {

    }
}