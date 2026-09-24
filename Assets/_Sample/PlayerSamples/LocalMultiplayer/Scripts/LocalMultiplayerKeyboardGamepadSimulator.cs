using UnityEngine;
using UnityEngine.InputSystem;

    /// <summary>
    /// Sample-only component. Owns the lifecycle of the simulated Gamepad used for
    /// Player 2 when local multiplayer is tested with one physical Keyboard and Mouse.
    /// I/J/K/L are projected to the left stick and Numpad 8/5/4/6 to the right
    /// stick. Player 1 uses the physical Keyboard and Mouse through the normal input
    /// path. This component has no knowledge of Joining, Player Slots, or the
    /// Framework Session.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LocalMultiplayerKeyboardGamepadSimulator : MonoBehaviour
    {
        private const float SimulatedLookPulseInterval = 0.05f;

        private Gamepad _device2;
        private float _nextLookPulseTime;

        public Gamepad GetOrCreateDevice2()
        {
            _device2 ??= InputSystem.AddDevice<Gamepad>();
            return _device2;
        }

        private void OnDestroy()
        {
            RemoveDevice(ref _device2);
        }

        private void Update()
        {
            ApplyMovement(_device2, ReadMovement());
            ApplyLook(_device2, ReadLook());
        }

        private static Vector2 ReadMovement()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            return ReadVector(
                keyboard.iKey.isPressed,
                keyboard.kKey.isPressed,
                keyboard.jKey.isPressed,
                keyboard.lKey.isPressed);
        }

        private static Vector2 ReadLook()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            return ReadVector(
                keyboard.numpad8Key.isPressed,
                keyboard.numpad5Key.isPressed,
                keyboard.numpad4Key.isPressed,
                keyboard.numpad6Key.isPressed);
        }

        private static Vector2 ReadVector(bool up, bool down, bool left, bool right)
        {
            float x = 0f;
            float y = 0f;

            if (up) y += 1f;
            if (down) y -= 1f;
            if (left) x -= 1f;
            if (right) x += 1f;

            Vector2 value = new Vector2(x, y);
            return value.sqrMagnitude > 1f ? value.normalized : value;
        }

        private static void ApplyMovement(Gamepad device, Vector2 movement)
        {
            if (device == null || !device.added)
            {
                return;
            }

            InputSystem.QueueDeltaStateEvent(device.leftStick, movement);
        }

        private void ApplyLook(Gamepad device, Vector2 look)
        {
            if (device == null || !device.added)
            {
                return;
            }

            if (look.sqrMagnitude <= 0.0001f)
            {
                InputSystem.QueueDeltaStateEvent(device.rightStick, Vector2.zero);
                _nextLookPulseTime = 0f;
                return;
            }

            if (_nextLookPulseTime > Time.unscaledTime)
            {
                return;
            }

            InputSystem.QueueDeltaStateEvent(device.rightStick, look);
            _nextLookPulseTime = Time.unscaledTime + SimulatedLookPulseInterval;
        }

        private static void RemoveDevice(ref Gamepad device)
        {
            if (device != null && device.added)
            {
                InputSystem.RemoveDevice(device);
            }

            device = null;
        }
    }
