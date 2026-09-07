using UnityEngine;
using UnityEngine.InputSystem;

    /// <summary>
    /// Sample-only component. Owns the lifecycle of the two simulated Gamepads used
    /// for single-Keyboard local testing and projects the physical Keyboard (WASD /
    /// Arrow Keys) into their left stick, so a user with only one physical Keyboard
    /// can drive both simulated Players. It has no knowledge of Joining, tutorial
    /// state, Player Slots, or the Framework Session.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LocalMultiplayerKeyboardGamepadSimulator : MonoBehaviour
    {
        private Gamepad _device1;
        private Gamepad _device2;

        /// <summary>
        /// Returns the virtual Gamepad driven by WASD, creating it lazily on first
        /// request. Always returns the same added Gamepad instance once created.
        /// </summary>
        public Gamepad GetOrCreateDevice1()
        {
            _device1 ??= InputSystem.AddDevice<Gamepad>();
            return _device1;
        }

        /// <summary>
        /// Returns the virtual Gamepad driven by Arrow Keys, creating it lazily on
        /// first request. Always returns the same added Gamepad instance once
        /// created, distinct from <see cref="GetOrCreateDevice1"/>.
        /// </summary>
        public Gamepad GetOrCreateDevice2()
        {
            _device2 ??= InputSystem.AddDevice<Gamepad>();
            return _device2;
        }

        private void OnDestroy()
        {
            RemoveDevice(ref _device1);
            RemoveDevice(ref _device2);
        }

        private void Update()
        {
            ApplyMovement(_device1, ReadDevice1Movement());
            ApplyMovement(_device2, ReadDevice2Movement());
        }

        private static Vector2 ReadDevice1Movement()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            return ReadMovement(
                keyboard.wKey.isPressed,
                keyboard.sKey.isPressed,
                keyboard.aKey.isPressed,
                keyboard.dKey.isPressed);
        }

        private static Vector2 ReadDevice2Movement()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            return ReadMovement(
                keyboard.upArrowKey.isPressed,
                keyboard.downArrowKey.isPressed,
                keyboard.leftArrowKey.isPressed,
                keyboard.rightArrowKey.isPressed);
        }

        private static Vector2 ReadMovement(bool up, bool down, bool left, bool right)
        {
            float x = 0f;
            float y = 0f;

            if (up)
            {
                y += 1f;
            }

            if (down)
            {
                y -= 1f;
            }

            if (left)
            {
                x -= 1f;
            }

            if (right)
            {
                x += 1f;
            }

            Vector2 movement = new Vector2(x, y);
            return movement.sqrMagnitude > 1f ? movement.normalized : movement;
        }

        private static void ApplyMovement(Gamepad device, Vector2 movement)
        {
            if (device == null || !device.added)
            {
                return;
            }

            InputSystem.QueueDeltaStateEvent(device.leftStick, movement);
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
