﻿using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Devcade {
  public static class Input {
    /// <summary>
    /// Enum of button names available on the cabinet. Maps directly to the 
    /// equivalent in the Buttons enum. Allows you to use existing controller 
    /// logic and essentially just rename the buttons but you must explicitly 
    /// cast it to a Button each time.
    /// </summary>
    public enum ArcadeButtons {
      A1=Buttons.X,
      A2=Buttons.Y,
      A3=Buttons.RightShoulder,
      A4=Buttons.LeftShoulder,
      B1=Buttons.A,
      B2=Buttons.B,
      B3=Buttons.RightTrigger,
      B4=Buttons.LeftTrigger,
      Menu=Buttons.Start,
      StickDown=Buttons.LeftThumbstickDown,
      StickUp=Buttons.LeftThumbstickUp,
      StickLeft=Buttons.LeftThumbstickLeft,
      StickRight=Buttons.LeftThumbstickRight
    }

    #region States
    public static GamePadState p1State { get; private set; }
    public static GamePadState p1LastState { get; private set; }

    public static GamePadState p2State { get; private set; }
    public static GamePadState p2LastState { get; private set; }
    #endregion
    
    private static bool externalUpdate;
    private static bool internalUpdate;
    
    private class UpdateManagerException : ApplicationException {
      public UpdateManagerException(string message) : base(message) { }
    }

    /// <summary>
    /// Checks if a button is currently pressed. 
    /// </summary>
    /// <param name="playerNum">The player whose controls should be checked.</param>
    /// <param name="button">The button to check.</param>
    /// <returns>True when button is pressed, false otherwise.</returns>
    public static bool GetButton(int playerNum, ArcadeButtons button) {
      switch (playerNum) {
        case 1 when p1State.IsButtonDown((Buttons)button):
        case 2 when p2State.IsButtonDown((Buttons)button):
          return true;
        default:
          return false;
      }
    }

    /// <summary>
    /// Checks if a button was pressed last frame. 
    /// </summary>
    /// <param name="playerNum">The player whose controls should be checked.</param>
    /// <param name="button">The button to check.</param>
    /// <returns>True if the button was pressed last frame, false otherwise.</returns>
    private static bool GetLastButton(int playerNum, ArcadeButtons button) {
      switch (playerNum) {
        case 1 when p1LastState.IsButtonDown((Buttons)button):
        case 2 when p2LastState.IsButtonDown((Buttons)button):
          return true;
        default:
          return false;
      }
    }

    /// <summary>
    /// Checks if a button was pressed down this frame.
    /// </summary>
    /// <param name="playerNum">The player whose controls should be checked.</param>
    /// <param name="button">The button to check.</param>
    /// <returns>True if the button transitioned from up to down in the current frame, false otherwise.</returns>
    public static bool GetButtonDown(int playerNum, ArcadeButtons button) {
      return (GetButton(playerNum, button) && !GetLastButton(playerNum, button));
    }

    /// <summary>
    /// Checks if a button was released this frame.
    /// </summary>
    /// <param name="playerNum">The player whose controls should be checked.</param>
    /// <param name="button">The button to check.</param>
    /// <returns>True if the button transitioned from down to up in the current frame, false otherwise.</returns>
    public static bool GetButtonUp(int playerNum, ArcadeButtons button) {
      return (!GetButton(playerNum, button) && GetLastButton(playerNum, button));
    }

    /// <summary>
    /// Checks if a button is being held down.
    /// </summary>
    /// <param name="playerNum">The player whose controls should be checked.</param>
    /// <param name="button">The button to check.</param>
    /// <returns>True if the button was down last frame and is still down, false otherwise.</returns>
    public static bool GetButtonHeld(int playerNum, ArcadeButtons button) {
      return (GetButton(playerNum, button) && GetLastButton(playerNum, button));
    }

    /// <summary>
    /// Gets the stick position of a player.
    /// </summary>
    /// <param name="playerNum">The player whose controls should be checked.</param>
    /// <returns>A Vector2 representing the stick direction.</returns>
    public static Vector2 GetStick(int playerNum) {
      return playerNum switch {
        1 => p1State.ThumbSticks.Left,
        2 => p2State.ThumbSticks.Left,
        _ => Vector2.Zero
      };
    }

    /// <summary>
    /// Setup initial input states.
    /// </summary>
    public static void Initialize() {
      p1State = GamePad.GetState(0);
      p2State = GamePad.GetState(1);
      p1LastState = GamePad.GetState(0);
      p2LastState = GamePad.GetState(1);
    }
    
    internal static void UpdateInternal() {
      internalUpdate = true;
      if (externalUpdate) {
        // This exception does not necessarily need to be thrown, it could just ignore the calls, but
        // calling both Update() methods will probably lead to behavior that is not intended.
        throw new UpdateManagerException("Cannot use Input.Update() and InputManager.Update() in the same project");
      }
      p1LastState = p1State;
      p2LastState = p2State;
      p1State = GamePad.GetState(0);
      p2State = GamePad.GetState(1);
    }

    /// <summary>
    /// Updates input states.
    /// </summary>
    public static void Update() {
      externalUpdate = true;
      if (internalUpdate) {
        throw new UpdateManagerException("Cannot use Input.Update() and InputManager.Update() in the same project");
      }
      p1LastState = p1State;
      p2LastState = p2State;
      p1State = GamePad.GetState(0);
      p2State = GamePad.GetState(1);
    }
    
    public static (GamePadState player1, GamePadState player2) GetStates() {
      return (player1: p1State, player2: p2State);
    }
  }
}
