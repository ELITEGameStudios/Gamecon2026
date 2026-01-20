using UnityEngine;

[System.Serializable]
public abstract class State
{
    protected StateMachine host;
    public string name;
    public StateMachine GetHost() { return host; }
    [HideInInspector]public Rigidbody rigidbody;
    [HideInInspector]public Animator animator;
    [HideInInspector]public Transform transform;
    public bool finished, started;

    public State(StateMachine stateMachine) // Always include super(StateMachine) in any child class constructors
    {
        host = stateMachine;
        stateMachine.TryAddState(this);
        OnReset();
    }

    public virtual void OnReset() // Called When the state object is first created and when resetting the state to be used again. Put all reset code here
    {
        // Ported variables from StateMachine
        rigidbody = host.rigidbody;
        animator = host.animator;
        transform = host.transform;
        host.TryAddState(this);

        finished = false;
        started = false;
    }

    public abstract void Start(); // Called When the state object becomes active
    public abstract void Update(); // Called every frame while the object is active
    public virtual void LateUpdate(){} // Called every frame while the object is active
    public virtual void FixedUpdate() {} // Called every physics frame while the object is active
    public virtual void InactiveUpdate() {} // Called every frame when the state is not active
    public virtual void OnCollisionEnter(Collision collision) {} // Called every physics frame while the object is active
    public virtual void OnCollisionStay(Collision collision) {} // Called every physics frame while the object is active
    public virtual void OnCollisionExit(Collision collision) {} // Called every physics frame while the object is active

    public virtual void End(bool interrupted = false) // Called once the state declares it is finished its task
    {
        // if(finished){ return; }
        finished = true;
    }
}