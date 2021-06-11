using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
public class InputReader : ScriptableObject, GMPlayInput.IGameplayActions
{

    public UnityAction jumpEvent = delegate { };
    public UnityAction jumpCanceledEvent = delegate { };
    public UnityAction<Vector2> moveEvent = delegate { };
    public event UnityAction swapEvent = delegate { };

    public event UnityAction startRunningEvent = delegate { };
    public event UnityAction stopRunningEvent = delegate { };

    private GMPlayInput _gameInput;

    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GMPlayInput();
            _gameInput.Gameplay.SetCallbacks(this);
        }

        EnableGameplayInput();
    }

    public void OnCut(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    //ToDo: Probably not
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            jumpEvent.Invoke();
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            jumpCanceledEvent.Invoke();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveEvent.Invoke(context.ReadValue<Vector2>());
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                startRunningEvent.Invoke();
                break;
            case InputActionPhase.Canceled:
                stopRunningEvent.Invoke();
                break;
        }
    }

    public void OnSwap(InputAction.CallbackContext context) { if (context.performed) { swapEvent.Invoke(); } }

    public void EnableGameplayInput() { _gameInput.Gameplay.Enable(); }

    public void DisableAllInput() { _gameInput.Gameplay.Disable(); }


}
