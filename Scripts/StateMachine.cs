﻿using System.Collections.Generic;
using System;
using System.Linq;

[System.Serializable]
public class StateMachine
{
    List<StateTransition> stateTransitions;

    IState currentState;
    IState emptyState;

    public string currentStateType;
    private bool _isDropped = false;

    public string type { get; set; }

    public StateMachine()
    {
        stateTransitions = new List<StateTransition>();

        emptyState = new EmptyState();
    }

    public void SetState(IState newState)
    {
        if (_isDropped || newState.Equals(currentState))
        {
            return;
        }

        currentState?.Exit();
        newState?.Enter();
        currentState = newState;
        currentStateType = currentState.type;
    }

    private void SetState(StateTransitionData transitionData)
    {
        if (_isDropped || transitionData.to.Equals(currentState))
        {
            return;
        }

        currentState?.Exit();
        transitionData.onTransition?.Invoke();
        transitionData.to?.Enter();
        currentState = transitionData.to;
        currentStateType = currentState.type;
    }

    public void Update()
    {
        StateTransitionData transitionData = checkTransitions();

        if (transitionData != null)
        {
            SetState(transitionData);
        }

        currentState?.Update();
    }

    public void AddNormalTransition(StateTransition stateTransition)
    {
        stateTransitions.Add(stateTransition);
    }

    public void AddAnyTransition(StateTransition anyTransition)
    {
        stateTransitions.Add(anyTransition);
    }

    public void AddAnyTransitionTrigger(ref Action action, StateTransition transition)
    {
        action += () =>
        {
            if (transition.Condition() && !currentState.Equals(transition.To))
            {
                SetState(transition.To);
                transition.OnTransition?.Invoke();
            }
        };
    }

    public void AddNormalTransitionTrigger(ref Action action, StateTransition transition)
    {
        action += () =>
        {
            if (transition.Condition() && currentState.Equals(transition.From) && !currentState.Equals(transition.To))
            {
                SetState(transition.To);
                transition.OnTransition?.Invoke();
            }
        };
    }

    public void DropMachine(bool isDropped)
    {
        if (isDropped)
        {
            SetState(emptyState);
        }

        _isDropped = isDropped;
    }

    private StateTransitionData checkTransitions() // Fix here. if the state doesn't contain from, but not condition met, it's overriding the bottom.
    {
        foreach (var transition in stateTransitions.OrderByDescending(t => t.Priority))
        {
            if (transition.From != null)
            {
                if (transition.From.Equals(currentState))
                {
                    if (transition.Condition.Invoke())
                    {
                        return new StateTransitionData(transition.To, transition.OnTransition);
                    }

                    return null;
                }
            }

            else
            {
                if (transition.Condition.Invoke())
                {
                    return new StateTransitionData(transition.To, transition.OnTransition);
                }
            }
        }

        return null;
    }
}

public class StateTransition
{
    private IState _from;
    private IState _to;
    private Func<bool> _condition;
    private Action onTransition;
    private int _priority { get; set; }

    public StateTransition(IState from, IState to, Func<bool> condition, int priority)
    {
        _from = from;
        _to = to;
        _condition = condition;
        _priority = priority;
    }

    public IState From
    {
        get { return _from; }
    }

    public IState To
    {
        get { return _to; }
    }

    public Func<bool> Condition
    {
        get { return _condition; }
    }

    public Action OnTransition
    {
        get
        {
            return onTransition;
        }
    }
    public int Priority
    {
        get
        {
            return _priority;
        }
    }
    public StateTransition(IState from, IState to, Func<bool> condition)
    {
        _from = from;
        _to = to;
        _condition = condition;
    }

    public StateTransition(IState to, Func<bool> condition, int priority)
    {
        _to = to;
        _condition = condition;
        _priority = priority;
    }

    public StateTransition(IState to, Func<bool> condition)
    {
        _to = to;
        _condition = condition;
    }

    public StateTransition(IState from, IState to)
    {
        _from = from;
        _to = to;
    }

    public StateTransition(IState to)
    {
        _to = to;
    }

    public void SetOnTransition(Action transitionAction)
    {
        onTransition = transitionAction;
    }
}

public class StateTransitionData
{
    public IState to;
    public Action onTransition;

    public StateTransitionData(IState to, Action onTransition)
    {
        this.to = to;
        this.onTransition += onTransition;
    }
}