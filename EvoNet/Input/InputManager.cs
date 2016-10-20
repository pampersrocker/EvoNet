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
  public class InputManager
  {

    GameConfig gameConfiguration;
    Camera camera;

    bool rightMouseDown = false;
    Vector2 oldMousePosition = Vector2.Zero;
    int scrollWheelValue;

    public void Initialize(GameConfig configuration, Camera inCamera)
    {
      gameConfiguration = configuration;
      camera = inCamera;
      scrollWheelValue = Mouse.GetState().ScrollWheelValue;
    }

    public void Update(GameTime gameTime)
    {
      KeyboardState keyboardState = Keyboard.GetState();
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

      MouseState mouseState = Mouse.GetState();

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

      camera.Scale += (gameConfiguration.ScaleFactor * (deltaScrollWheelValue / 120.0f)) / (camera.Scale < 1 ? 1 / camera.Scale : camera.Scale);

      scrollWheelValue = mouseState.ScrollWheelValue;

      oldMousePosition = mouseState.Position.ToVector2();
    }
  }
}
