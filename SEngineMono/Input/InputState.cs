using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEngineMono.Input
{
    /// <summary>
    /// Enum that indicates the state of a button.
    /// </summary>
    public enum EInputState
    {
        Pressed, //Just pressed
        Released, //Just released
        Active, //Still pressed
        Inactive, //Still released
        Unknown
    }

    /// <summary>
    /// Enum for defining mouse buttons.
    /// </summary>
    public enum EMouseButtons
    {
        LeftButton,
        MiddleButton,
        RightButton
    }

    /// <summary>
    /// Class that handles input.
    /// </summary>
    public sealed class InputState
    {
        public static readonly InputState Instance = new InputState();

        private KeyboardState oldKeyState;
        private MouseState oldMouseState;

        /// <summary>
        /// Constructor. Private as InputState is a singleton.
        /// Initializes all inputs.
        /// </summary>
        private InputState()
        {
            update();
        }

        /// <summary>
        /// Updates all input devices.
        /// </summary>
        public void update()
        {
            updateKeyboardState();
            updateMouseState();
        }

        /// <summary>
        /// Updates the keyboard state.
        /// </summary>
        public void updateKeyboardState()
        {
            oldKeyState = Keyboard.GetState();
        }

        /// <summary>
        /// Updates the mouse state.
        /// </summary>
        public void updateMouseState()
        {
            oldMouseState = Mouse.GetState();
        }

        #region Keyboard
        /// <summary>
        /// Gets the state for the given key.
        /// </summary>
        /// <param name="key">The key to get the state for.</param>
        /// <returns>The key state.</returns>
        public EInputState getKeyState(Keys key)
        {
            KeyboardState newKeyState = Keyboard.GetState();

            if (oldKeyState.IsKeyUp(key) && newKeyState.IsKeyDown(key))
            {
                return EInputState.Pressed;
            }
            else if (oldKeyState.IsKeyDown(key) && newKeyState.IsKeyUp(key))
            {
                return EInputState.Released;
            }
            else if (oldKeyState.IsKeyDown(key) && newKeyState.IsKeyDown(key))
            {
                return EInputState.Active;
            }
            else if (oldKeyState.IsKeyUp(key) && newKeyState.IsKeyUp(key))
            {
                return EInputState.Inactive;
            }

            return EInputState.Unknown;
        }

        /// <summary>
        /// Returns whether the given key is newly pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Newly pressed yes or no.</returns>
        public bool isKeyPressed(Keys key)
        {
            return getKeyState(key) == EInputState.Pressed;
        }

        /// <summary>
        /// Returns whether the given key is just released.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Just released yes or no.</returns>
        public bool isKeyReleased(Keys key)
        {
            return getKeyState(key) == EInputState.Released;
        }

        /// <summary>
        /// Returns whether the given key is still pressed.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Still pressed yes or no.</returns>
        public bool isKeyActive(Keys key)
        {
            return getKeyState(key) == EInputState.Active;
        }

        /// <summary>
        /// Returns whether the given key is still released.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>Still released yes or no.</returns>
        public bool isKeyInactive(Keys key)
        {
            return getKeyState(key) == EInputState.Inactive;
        }
        #endregion

        #region Mouse
        /// <summary>
        /// Gets the state for the given mouse button.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>The input state of the given mouse button.</returns>
        public EInputState getMouseState(EMouseButtons button)
        {
            MouseState newMouseState = Mouse.GetState();

            ButtonState oldButtonState;
            ButtonState newButtonState;

            switch (button)
            {
                case EMouseButtons.LeftButton:
                    oldButtonState = oldMouseState.LeftButton;
                    newButtonState = newMouseState.LeftButton;
                    break;

                case EMouseButtons.MiddleButton:
                    oldButtonState = oldMouseState.MiddleButton;
                    newButtonState = newMouseState.MiddleButton;
                    break;

                case EMouseButtons.RightButton:
                    oldButtonState = oldMouseState.RightButton;
                    newButtonState = newMouseState.RightButton;
                    break;

                default:
                    return EInputState.Unknown;
            }

            if (oldButtonState == ButtonState.Released && newButtonState == ButtonState.Pressed)
            {
                return EInputState.Pressed;
            }
            else if (oldButtonState == ButtonState.Pressed && newButtonState == ButtonState.Released)
            {
                return EInputState.Released;
            }
            else if (oldButtonState == ButtonState.Pressed && newButtonState == ButtonState.Pressed)
            {
                return EInputState.Active;
            }
            else if (oldButtonState == ButtonState.Released && newButtonState == ButtonState.Released)
            {
                return EInputState.Inactive;
            }

            return EInputState.Unknown;
        }

        /// <summary>
        /// Gets the current mouse position.
        /// </summary>
        public Vector2 mousePosition
        {
            get
            {
                MouseState ms = Mouse.GetState();
                return new Vector2(ms.X, ms.Y);
            }
        }

        /// <summary>
        /// Gets the change in mouse position since the last update.
        /// </summary>
        public Vector2 mousePositionChange
        {
            get
            {
                return mousePosition - new Vector2(oldMouseState.X, oldMouseState.Y);
            }
        }

        /// <summary>
        /// Gets the value of the mouse wheel.
        /// </summary>
        public int mouseWheel
        {
            get
            {
                return Mouse.GetState().ScrollWheelValue;
            }
        }

        /// <summary>
        /// Gets the change in value of the mouse wheel.
        /// </summary>
        public int mouseWheelChange
        {
            get
            {
                return mouseWheel - oldMouseState.ScrollWheelValue;
            }
        }

        /// <summary>
        /// Returns whether the given mouse button is newly pressed.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>Newly pressed yes or no.</returns>
        public bool isMouseButtonPressed(EMouseButtons button)
        {
            return getMouseState(button) == EInputState.Pressed;
        }

        /// <summary>
        /// Returns whether the given mouse button is just released.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>Just released yes or no.</returns>
        public bool isMouseButtonReleased(EMouseButtons button)
        {
            return getMouseState(button) == EInputState.Released;
        }

        /// <summary>
        /// Returns whether the given mouse button is still pressed.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>Still pressed yes or no.</returns>
        public bool isMouseButtonActive(EMouseButtons button)
        {
            return getMouseState(button) == EInputState.Active;
        }

        /// <summary>
        /// Returns whether the given mouse button is still released.
        /// </summary>
        /// <param name="button">The mouse button to check.</param>
        /// <returns>Still released yes or no.</returns>
        public bool isMouseButtonInactive(EMouseButtons button)
        {
            return getMouseState(button) == EInputState.Inactive;
        }
        #endregion
    }
}
