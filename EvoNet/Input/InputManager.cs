using EvoNet.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoNet.Input
{



    public class InputManager : UpdateModule
    {

        GameConfig gameConfiguration;
        Camera camera;

        bool rightMouseDown = false;
        bool oldSpaceDown = false;
        Vector2 oldMousePosition = Vector2.Zero;
        int scrollWheelValue;

        public bool EnableFastForward { get; private set; }

        public override bool WantsFastForward
        {
            get
            {
                return false;
            }
        }

        public override void Initialize(EvoGame ingame)
        {
            base.Initialize(ingame);
            game = ingame;
            gameConfiguration = ingame.gameConfiguration;
            camera = Camera.instanceGameWorld;
            scrollWheelValue = Mouse.GetState().ScrollWheelValue;
        }

        protected override void Update(GameTime gameTime)
        {

            if (!game.IsActive)
            {
                scrollWheelValue = Mouse.GetState().ScrollWheelValue;
                return;
            }
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();



            DoMovement(gameTime, keyboardState, mouseState);

            bool spaceDown = keyboardState.IsKeyDown(Keys.Space);

            if (!oldSpaceDown && spaceDown)
            {
                EnableFastForward = !EnableFastForward;
            }

            oldSpaceDown = spaceDown;
            scrollWheelValue = mouseState.ScrollWheelValue;

            oldMousePosition = mouseState.Position.ToVector2();
        }

        private void DoMovement(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
        {
            Vector2 movement = Vector2.Zero;
            bool moveUp = false;
            bool moveDown = false;
            bool moveLeft = false;
            bool moveRight = false;
            foreach (Keys upKey in gameConfiguration.MoveUpKeys)
            {
                if (keyboardState.IsKeyDown(upKey))
                {
                    moveUp = true;
                    break;
                }
            }
            foreach (Keys key in gameConfiguration.MoveLeftKeys)
            {
                if (keyboardState.IsKeyDown(key))
                {
                    moveLeft = true;
                    break;
                }
            }
            foreach (Keys key in gameConfiguration.MoveRightKeys)
            {
                if (keyboardState.IsKeyDown(key))
                {
                    moveRight = true;
                    break;
                }
            }
            foreach (Keys key in gameConfiguration.MoveDownKeys)
            {
                if (keyboardState.IsKeyDown(key))
                {
                    moveDown = true;
                    break;
                }
            }
            if (moveUp)
            {
                movement += Vector2.UnitY * gameConfiguration.MovementSensitivity;
            }

            if (moveDown)
            {
                movement -= Vector2.UnitY * gameConfiguration.MovementSensitivity;
            }
            if (moveLeft)
            {
                movement += Vector2.UnitX * gameConfiguration.MovementSensitivity;
            }
            if (moveRight)
            {
                movement -= Vector2.UnitX * gameConfiguration.MovementSensitivity;
            }
            movement *= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (camera != null)
            {
                camera.Move(movement);
            }


            if (mouseState.RightButton == ButtonState.Pressed)
            {

                if (!rightMouseDown)
                {
                    rightMouseDown = true;
                    oldMousePosition = mouseState.Position.ToVector2();
                }

                Vector2 delta = mouseState.Position.ToVector2() - oldMousePosition;
                camera.Move(delta);
            }
            else
            {
                rightMouseDown = false;
            }

            int deltaScrollWheelValue = mouseState.ScrollWheelValue - scrollWheelValue;

            // Track where we are before scale
            Vector2 mousePositionBeforeScale = Vector2.Transform(mouseState.Position.ToVector2(), Matrix.Invert(camera.Matrix));

            camera.Scale += (gameConfiguration.ScaleFactor * (deltaScrollWheelValue / 120.0f)) / (camera.Scale < 1 ? 1 / camera.Scale : camera.Scale);

            // Track where we are after scale
            Vector2 mousePositionAfterScale = Vector2.Transform(mouseState.Position.ToVector2(), Matrix.Invert(camera.Matrix));

            // Adjust screen position with respect to scale to achieve zoom to Mouse cursor functionality
            camera.Move((mousePositionAfterScale - mousePositionBeforeScale) * camera.Scale);
        }
    }
}
