using System.Collections.Generic;
using System.Linq;

public class CharacterCombatSystem
{
    CombatContext context;

    IState currentState;

    List<StateTransition> stateTransitions;
    List<StateTransition> anyTransitions;

    CharacterData data;

    public CharacterCombatSystem(CharacterData data)
    {
        stateTransitions = new List<StateTransition>();
        anyTransitions = new List<StateTransition>();

        context = data.combatContext;
    }

    public void Update()
    {
        StateTransitionData transition = checkTransitions();
        if (transition != null)
        {
            SetState(transition.to);
            transition.onTransition?.Invoke();
        }

        currentState?.Update();
    }

    public void Initialize()
    {

    }

    public void End()
    {

    }

    public void AddNormalTransition(StateTransition stateTransition)
    {
        stateTransitions.Add(stateTransition);
    }

    public void AddAnyTransition(StateTransition anyTransition)
    {
        anyTransitions.Add(anyTransition);
    }

    public void SetState(IState newState)
    {
        if (newState.Equals(currentState))
        {
            return;
        }

        currentState?.Exit();
        newState?.Enter();
        currentState = newState;
        context.currentStateType = newState.type;
    }

    private StateTransitionData checkTransitions()
    {
        foreach (var transition in stateTransitions.Where(t => t.From.Equals(currentState)).OrderByDescending(t => t.Priority))
        {
            if (transition.From.Equals(currentState) && transition.Condition.Invoke())
            {
                return new StateTransitionData(transition.To, transition.OnTransition);
            }
        }


        foreach (var transition in anyTransitions.OrderByDescending(t => t.Priority))
        {
            if (transition.Condition.Invoke())
            {
                return new StateTransitionData(transition.To, transition.OnTransition);
            }
        }

        return null;
    }
}

[System.Serializable]
public class CombatContext
{
    public string currentStateType;

    public bool isAttacking;
    public bool isIdling;
}