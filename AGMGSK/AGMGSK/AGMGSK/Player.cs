/*  
    Copyright (C) 2014 G. Michael Barnes
 
    The file Player.cs is part of AGMGSKv5 a port of AGXNASKv4 from
    XNA 4 refresh to MonoGames 3.0.6.

    AGMGSKv5 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if ! __XNA4__  // when __XNA4__ == true build for MonoGames
   using Microsoft.Xna.Framework.Storage; 
#endif
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace AGMGSK {

/// <summary>
/// Represents the user / player interacting with the stage. 
/// The Update(Gametime) handles both user keyboard and gamepad controller input.
/// If there is a gamepad attached the keyboard inputs are not processed.
/// 
/// 1/17/2014 last changed
/// </summary>

public class Player : Agent {
   private GamePadState oldGamePadState;
   private KeyboardState oldKeyboardState;  
   private int rotate;
   private float angle;
   private Matrix initialOrientation;

   public Player(Stage theStage, string label, Vector3 pos, Vector3 orientAxis, 
   float radians, string meshFile)
   : base(theStage, label, pos, orientAxis, radians, meshFile)
      {  // change names for on-screen display of current camera
      first.Name =  "First";
      follow.Name = "Follow";
      above.Name =  "Above";
      IsCollidable = true;  // players test collision with Collidable set.
      stage.Collidable.Add(agentObject);  // player's agentObject can be collided with by others.
      rotate = 0;
      angle = 0.01f;
      initialOrientation = agentObject.Orientation;
      }

   /// <summary>
   /// Handle player input that affects the player.
   /// See Stage.Update(...) for handling user input that affects
   /// how the stage is rendered.
   /// First check if gamepad is connected, if true use gamepad
   /// otherwise assume and use keyboard.
   /// </summary>
   /// <param name="gameTime"></param>
   public override void Update(GameTime gameTime) {
      GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
      if (gamePadState.IsConnected) {
            if (gamePadState.Buttons.X == ButtonState.Pressed) stage.Exit();
            else if (gamePadState.Buttons.A == ButtonState.Pressed &&
               oldGamePadState.Buttons.A != ButtonState.Pressed) stage.nextCamera();
            else if (gamePadState.Buttons.B == ButtonState.Pressed &&
               oldGamePadState.Buttons.B != ButtonState.Pressed) stage.Fog = ! stage.Fog;
            else if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed &&
               oldGamePadState.Buttons.RightShoulder != ButtonState.Pressed) 
               stage.FixedStepRendering = ! stage.FixedStepRendering;
            // allow more than one gamePadState to be pressed
            if (gamePadState.DPad.Up == ButtonState.Pressed) agentObject.Step++;
            if (gamePadState.DPad.Down == ButtonState.Pressed) agentObject.Step--;
            if (gamePadState.DPad.Left == ButtonState.Pressed) rotate++;
            if (gamePadState.DPad.Right == ButtonState.Pressed) rotate--;
            oldGamePadState = gamePadState;
            }
         else { // no gamepad assume use of keyboard
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.R) && !oldKeyboardState.IsKeyDown(Keys.R)) 
               agentObject.Orientation = initialOrientation; 
            // allow more than one keyboardState to be pressed
            if (keyboardState.IsKeyDown(Keys.Up)) agentObject.Step++;
            if (keyboardState.IsKeyDown(Keys.Down)) agentObject.Step--; 
            if (keyboardState.IsKeyDown(Keys.Left)) rotate++;
            if (keyboardState.IsKeyDown(Keys.Right)) rotate--;
            // sk 565 old not used with terrain following -- can be useful in debugging
           // if (keyboardState.IsKeyDown(Keys.PageUp)) vertical++;    
           // if (keyboardState.IsKeyDown(Keys.PageDown)) vertical--;
            oldKeyboardState = keyboardState;    // Update saved state.
            } 
      agentObject.Yaw = rotate * angle;
      base.Update(gameTime);
      rotate = agentObject.Step = 0;
      }
   }
}
