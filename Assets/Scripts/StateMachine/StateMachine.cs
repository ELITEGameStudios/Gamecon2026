using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
// using Pathfinding;

public class StateMachine : MonoBehaviour
{


    [Header("Base Functional Information")]
    public Transform transform;
    public Rigidbody rigidbody;
    public Animator animator;
    public bool hasAnimator { get { return animator != null; } }


    [Header("State Machine")]
    public State currentState;
    public State defaultState;
    public List<State> states;


    void OnEnable()
    {
        OnUnityEnable();
    }
    
    void OnDisable()
    {
        OnUnityDisable();
    }

    // Start is called before the first frame update
    void Start()
    {
        ChooseDefaultState();
        OnStart();
    }

    public void SetState(State state)
    {
        TryAddState(state);

        // Debug.Log("state set to "+ state.name);
        if(currentState != null) currentState.End();
        currentState = state;
        currentState.OnReset();
        StateCheck();
        OnSetState();
    }

    public void TryAddState(State state)
    {
        if(states == null){states = new List<State>();}
        else if(states.Contains(state)){return;}
        states.Add(state);
    }


    void StateCheck()
    {
        if (currentState != null && !currentState.finished)
        {
            if (!currentState.started)
            {
                currentState.Start();
                currentState.started = true;
            }
            else
            {
                // If the state is active and started
                currentState.Update();
            }
        }
        else
        {
            // If the state is finished
            ChooseDefaultState();
        }
    }


    // Update is called once per frame
    void Update()
    {
        OnUpdate();
        
        foreach (State state in states){
            if(state != currentState){ state.InactiveUpdate(); };
        }

        StateCheck();
    }

    void LateUpdate()
    {
        OnLateUpdate();
        
        if (currentState != null && !currentState.finished)
        { currentState.LateUpdate(); }
    }

    public virtual void ChooseDefaultState(){
        if(defaultState != null){SetState(defaultState);}
    }

    void FixedUpdate()
    {
        OnFixedUpdate();

        if (currentState != null && !currentState.finished)
        { currentState.FixedUpdate(); }
        
        PostStateFixedUpdate();
    }

    protected virtual void OnSetState(){}
    protected virtual void OnLateUpdate() { }
    protected virtual void OnUpdate(){}
    protected virtual void OnStart(){}
    protected virtual void OnUnityEnable(){}
    protected virtual void OnUnityDisable(){}
    protected virtual void OnFixedUpdate(){}
    protected virtual void PostStateFixedUpdate(){}
    protected virtual void OnInactiveUpdate(){}
}   
    