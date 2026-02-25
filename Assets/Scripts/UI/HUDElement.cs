using TMPro;
using UnityEngine;

public class HUDElement : MonoBehaviour
{
    [SerializeField] protected string ReadyStateString = "Ready";
    [SerializeField] protected string ActivateTriggerString = "Activate";
    [SerializeField] protected bool isReady;
    [SerializeField] protected Animator animator;
    [SerializeField] protected TMP_Text keybindText;

    void Awake()
    {
       SetReady(false);
    }

    public void SetKeybind(KeyCode keybind)
    {
        keybindText.text = keybind.ToString();
    }

    public virtual void Activate()
    {
        isReady = false;
        animator.SetTrigger(ActivateTriggerString);
        animator.SetBool(ReadyStateString, isReady);
    }

    public virtual void SetReady(bool isReady)
    {
        this.isReady = true;
        animator.SetBool(ReadyStateString, isReady);
    }

    void Update(){ OnUpdate();}

    protected virtual void OnUpdate(){}
}
