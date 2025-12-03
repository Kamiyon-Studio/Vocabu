using UnityEngine;
using UnityEngine.InputSystem;

using ServiceLocator;
using ServiceLocator.Services;

using EventSystem;
using Events.InputSystem;

namespace InputSystem {
	public class InputManager : MonoBehaviour, IInputSystem {

        private InputSystem_Actions inputActions;



        // =====================================================================
        //
        //                          Unity Lifecycle
        //
        // =====================================================================
        private void Awake() {
            if (ServiceRegistry.IsRegistered<IInputSystem>()) {
                Debug.LogWarning("[InputManager] IInputSystem service is already registered.");
                Destroy(gameObject);
                return;
            }

            ServiceRegistry.Register<IInputSystem>(this);
            inputActions = new InputSystem_Actions();
        }

        private void OnEnable() {
            inputActions.Enable();

            inputActions.Player.Click.performed += OnClick;
        }

        private void OnDisable() {
            inputActions.Disable();

            inputActions.Player.Click.performed -= OnClick;
        }

        private void OnDestroy() {
            ServiceRegistry.Unregister<IInputSystem>(this);
        }

        // =====================================================================
        //
        //                              Event Methods
        //
        // =====================================================================
        private void OnClick(InputAction.CallbackContext context) { EventBus.Publish(new Evt_OnClick()); }
    }
}