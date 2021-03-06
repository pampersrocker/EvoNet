﻿using EvoNet.Configuration;
using EvoNet.Objects;
using EvoNet.Providers;
using EvoNet.Rendering;
using EvoSim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace EvoNet.Input
{
    public class InputManager : UpdateModule
    {

        GameConfig gameConfiguration;
        Camera camera;

        bool rightMouseDown = false;
        bool oldSpaceDown = false;
        bool oldEnterDown = false;
        Vector2 oldMousePosition = Vector2.Zero;
        int scrollWheelValue;

        public IMousePositionProvider PositionProvider;

        SimulationRenderer renderer;

        public bool EnableFastForward { get; private set; }
        public bool Paused { get; private set; }

        public override bool WantsFastForward
        {
            get
            {
                return false;
            }
        }

        public void Initialize(GameConfig config, Simulation sim, SimulationRenderer renderer)
        {
            gameConfiguration = config;
            Initialize(sim);
            camera = renderer.Camera;
            this.renderer = renderer;

            float viewportWidth = renderer.GraphicsDevice.Viewport.Width;
            float tileMapWidth = sim.TileMap.GetWorldWidth();
            float viewportHeight = renderer.GraphicsDevice.Viewport.Height;
            float tileMapHeight = sim.TileMap.GetWorldHeight();
            renderer.Camera.Scale = Mathf.Min(viewportWidth / tileMapWidth, viewportHeight / tileMapHeight);
            renderer.Camera.MinScale = renderer.Camera.Scale;
            renderer.Camera.Translation = new Vector2(tileMapWidth / 2, 0);

        }

        public override void Initialize(Simulation ingame)
        {
            base.Initialize(ingame);
            scrollWheelValue = Mouse.GetState().ScrollWheelValue;
        }

        public void OnMouseClick(Vector2 mousePosition)
        {
            Vector2 mousePos = Vector2.Transform(mousePosition, Matrix.Invert(camera.Matrix));
            float distanceSq = float.MaxValue;
            Creature closestCreature = null;
            foreach (Creature creature in simulation.CreatureManager.Creatures)
            {
                float distance = (creature.Pos.ToXNA() - mousePos).LengthSquared();
                if (distance < distanceSq)
                {
                    distanceSq = distance;
                    closestCreature = creature;
                }
            }
            if (closestCreature != null)
            {
                simulation.CreatureManager.SelectedCreature = closestCreature;
            }
        }

        protected override void Update(float gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = Vector2.Zero;
            if (PositionProvider != null)
            {
                mousePosition = PositionProvider.GetMousePosition();
            }

            DoMovement(gameTime, keyboardState, mouseState);

            bool spaceDown = keyboardState.IsKeyDown(Keys.Space);

            if (!oldSpaceDown && spaceDown)
            {
                EnableFastForward = !EnableFastForward;
            }

            bool enterDown = keyboardState.IsKeyDown(Keys.Enter);

            if (!oldEnterDown && enterDown)
            {
                Paused = !Paused;
            }

            if (keyboardState.IsKeyDown(Keys.R))
            {
                float viewportWidth = renderer.GraphicsDevice.Viewport.Width;
                float tileMapWidth = simulation.TileMap.GetWorldWidth();
                float viewportHeight = renderer.GraphicsDevice.Viewport.Height;
                float tileMapHeight = simulation.TileMap.GetWorldHeight();
                camera.Scale = Mathf.Min(viewportWidth / tileMapWidth, viewportHeight / tileMapHeight);

                camera.Translation = new Vector2(tileMapWidth / 2, 0);
            }

            oldSpaceDown = spaceDown;
            oldEnterDown = enterDown;
            scrollWheelValue = mouseState.ScrollWheelValue;

            oldMousePosition = new Vector2(mouseState.X, mouseState.Y);
        }

        public void Zoom(float zoomFactor, Vector2 mousePos)
        {
            // Track where we are before scale
            Vector2 mousePositionBeforeScale = Vector2.Transform(mousePos, Matrix.Invert(camera.Matrix));

            camera.Scale += (gameConfiguration.ScaleFactor * zoomFactor) / (camera.Scale < 1 ? 1 / camera.Scale : camera.Scale);

            // Track where we are after scale
            Vector2 mousePositionAfterScale = Vector2.Transform(mousePos, Matrix.Invert(camera.Matrix));

            // Adjust screen position with respect to scale to achieve zoom to Mouse cursor functionality
            camera.Move(mousePositionAfterScale - mousePositionBeforeScale);
        }

        private void DoMovement(float gameTime, KeyboardState keyboardState, MouseState mouseState)
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
            movement *= gameTime;

            if (camera != null)
            {
                camera.Move(new Vector2(movement.X, movement.Y));
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {

                if (!rightMouseDown)
                {
                    rightMouseDown = true;
                    oldMousePosition = new Vector2(mouseState.X, mouseState.Y);
                }

                Vector2 delta = new Vector2(mouseState.X, mouseState.Y) - oldMousePosition;
                camera.Move(delta / camera.Scale);
            }
            else
            {
                rightMouseDown = false;
            }

            int deltaScrollWheelValue = mouseState.ScrollWheelValue - scrollWheelValue;

            float zoomFactor = (deltaScrollWheelValue / 120.0f);

            Zoom(zoomFactor, new Vector2(mouseState.X, mouseState.Y));
        }
    }
}
