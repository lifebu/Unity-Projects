using UnityEngine;
using System;
using System.Collections.Generic;


public delegate void Callback();


/// <typeparam name="S">State enum</typeparam>
public class StateTransition<S> : System.IEquatable<StateTransition<S>>
{
    public S fromState { get; private set; }
    public S toState { get; private set; }

    public StateTransition() { }
    public StateTransition(S init, S end) { fromState = init; toState = end; }

    public bool Equals(StateTransition<S> other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return fromState.Equals(other.fromState) && toState.Equals(other.toState);
    }

    public override int GetHashCode()
    {
        if ((fromState == null || toState == null))
            return 0;

        unchecked
        {
            int hash = 17;
            hash = hash * 23 + fromState.GetHashCode();
            hash = hash * 23 + toState.GetHashCode();
            return hash;
        }
    }
}

/* TODO: Need special Transition from the Any-State to a state and a state to the Any-State!
 * Have to figure out how i go on to create the any-state!
 * Or I have stuff like an exit and entry callback?
*/

/* TODO: It should be possible to add a Function for Any Transition From and Any Transition To a state!
*/


/// <summary>
/// A generic Finite state machine
/// </summary>
/// <typeparam name="S"></typeparam>
public class FiniteStateMachine<S>
{
    public S currState { get; private set; }
    public S prevState { get; private set; }

    private bool _isLocked = false;
    public bool isLocked { set { _isLocked = value;
        if(_isLocked == false) Advance(prevState);} }

    private Dictionary<StateTransition<S>, Delegate>
        transitions;
    private Dictionary<S, Delegate>
        fromAnyTransitions;
    private Dictionary<StateTransition<S>, Delegate>
        autoTransitions;
    private Delegate anyTransition;
    private Delegate anyAutoTransition;



    public FiniteStateMachine()
    {
        transitions = new Dictionary<StateTransition<S>, Delegate>();
        fromAnyTransitions = new Dictionary<S, Delegate>();
        autoTransitions = new Dictionary<StateTransition<S>, Delegate>();
        anyTransition = null;
        anyAutoTransition = null;
    }

    public void Initialize(S state, Callback anyTrans, Callback anyAutoTrans) 
    { 
        currState = state;
        anyTransition = anyTrans;
        anyAutoTransition = anyAutoTrans;
    }

    public void Advance(S nextState)
    {
        if (_isLocked)
            return;

        StateTransition<S> transition = new StateTransition<S>(currState, nextState);

        // Check if the transition is valid (does the normal transition or the fromAnyTransition exist?)
        if (transitions.ContainsKey(transition) || fromAnyTransitions.ContainsKey(nextState))
        {
            //Debug.Log("[FMS] Advancing to " + nextState + " state...");

            prevState = currState;
            currState = nextState;

            Delegate d;
            transitions.TryGetValue(transition, out d);
            Delegate dFromAny;
            fromAnyTransitions.TryGetValue(nextState, out dFromAny);

            if (d != null) ((Callback)d)();
            if (dFromAny != null) ((Callback)d)();
            if (anyTransition != null) ((Callback)anyTransition)();
        }
        else
        {
            Debug.Log("[FSM] Cannot advance to " + nextState + " state from: " + currState + " state");
        }
    }

    public void AddTransition(S init, S end, Callback c)
    {
        StateTransition<S> tr = new StateTransition<S>(init, end);

        if (transitions.ContainsKey(tr))
        {
            Debug.Log("[FSM] Transition: " + tr.fromState + " - " + tr.toState + " exists already.");
            return;
        }
        transitions.Add(tr, c);
    }

    public void AddFromAnyTransition(S end, Callback c)
    {
        if (fromAnyTransitions.ContainsKey(end))
        {
            Debug.Log("[FSM] Transition: " + "AnyState" + " - " + end + " exists already.");
            return;
        }
        fromAnyTransitions.Add(end, c);
    }

    /// <summary>
    /// This AddTransitions is for AutoTransitions, that can be used
    /// for behaviour that will be called every frame, by the
    /// Update() function!
    /// </summary>
    public void AddAutoTransition(S initAndEnd, Callback c)
    {
        StateTransition<S> tr = new StateTransition<S>(initAndEnd, initAndEnd);

        if (autoTransitions.ContainsKey(tr))
        {
            Debug.Log("[FSM] AutoUpdateTransition: " + tr.fromState + " - " + tr.toState + " exists already.");
            return;
        }  
        autoTransitions.Add(tr, c);
    }        

    /// <summary>
    /// This Function should be called in the GameObject.Update() method
    /// to allow for AutoTransition functionality.
    /// This will call any Transition that has the same from- and to- State
    /// and is connected to the currentState
    /// </summary>
    public void Update()
    {
        Delegate delg;
        if (autoTransitions.TryGetValue(new StateTransition<S>(currState, currState), out delg))
        {
            // We found a delegate, so call it!
            ((Callback)delg)();
        }

        if (anyAutoTransition != null) ((Callback)anyAutoTransition)();
    }
}