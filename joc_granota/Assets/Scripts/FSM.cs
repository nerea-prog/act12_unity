using System;
using System.Collections.Generic;

public class State
{
    public Action OnEnter;
    public Action OnStay;
    public Action OnExit;
}

public class FSM<T> where T : Enum
{
    public T currentState;
    private readonly Dictionary<T, State> states;

    public FSM(T initState)
    {
        states = new Dictionary<T, State>();
        foreach (T e in Enum.GetValues(typeof(T)))
        {
            states.Add(e, new State());
        }

        currentState = initState;
    }

    public void Update()
    {
        states[currentState].OnStay?.Invoke();
    }

    public void ChangeState(T newState)
    {
        if (EqualityComparer<T>.Default.Equals(currentState, newState))
        {
            return;
        }

        states[currentState].OnExit?.Invoke();
        states[newState].OnEnter?.Invoke();
        currentState = newState;
    }

    public void SetOnStay(T state, Action f) => states[state].OnStay = f;
    public void SetOnEnter(T state, Action f) => states[state].OnEnter = f;
    public void SetOnExit(T state, Action f) => states[state].OnExit = f;
}
