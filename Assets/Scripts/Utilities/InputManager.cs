using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Buffers")]
    [SerializeField] BufferHelper jumpBuffer;
    [SerializeField] BufferHelper dashBuffer;
    [SerializeField] BufferHelper blinkBuffer;
    [SerializeField] BufferHelper recallBuffer;
    [SerializeField] BufferHelper fireBuffer;

    [Header("Input Action References")]
    [SerializeField] InputActionReference movementAxis;
    [SerializeField] InputActionReference lookAxis;
    [SerializeField] InputActionReference fireReference;


    SettingsMenu settingsMenu;
    public bool MovementInputLastFrame { private set; get; }

    public bool IsJumpBuffered() => jumpBuffer.Buffered;
    public bool IsDashBuffered() => dashBuffer.Buffered;
    public bool IsBlinkBuffered() => blinkBuffer.Buffered;
    public bool IsRecallBuffered() => recallBuffer.Buffered;
    public bool IsFireBuffered() => fireBuffer.Buffered;
    public void OnJumpPerformed() => jumpBuffer.Consume();
    public void OnDashPerformed() => dashBuffer.Consume();
    public void OnBlinkPerformed() => blinkBuffer.Consume();
    public void OnRecallPerformed() => recallBuffer.Consume();

    public void OnFirePerformed()
    {
        fireBuffer.Consume();
        blinkBuffer.Consume();
        recallBuffer.Consume();
    }
    /// <summary>
    /// Resets the buffers associated with getting the knife back: blink and recall.
    /// </summary>
    public void ClearPickupBuffers()
    {
        recallBuffer.Consume();
        blinkBuffer.Consume();
    }
    public Vector2 GetLookDirection() => lookAxis.action.ReadValue<Vector2>();

    private void Start()
    {
        if (settingsMenu == null) settingsMenu = FindFirstObjectByType<SettingsMenu>();
    }
    public Vector2 GetMovementDirection()
    {
        var movement = movementAxis.action.ReadValue<Vector2>();
        if (movement.magnitude < settingsMenu.currentSettings.movementDeadzone) return Vector2.zero;
        return movement.normalized;
    }

    private void FixedUpdate()
    {
        MovementInputLastFrame = GetMovementDirection() != Vector2.zero;

        Debug.Log("Fire pressed == " + fireReference.action.IsPressed());
    }
}

