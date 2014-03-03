/*  
    Copyright (C) 2014 G. Michael Barnes
 
    The file Cloud.cs is part of AGMGSKv5 a port of AGXNASKv4 from
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
/// An example of how to override the MovableModel3D's Update(GameTime) to 
/// animate a model's objects.  The actual update of values is done by calling 
/// each instance object and setting its (Pitch, Yaw, Roll, or Step property. 
/// Then call base.Update(GameTime) method of MovableModel3D to apply transformations.
/// 
/// 1/25/2012  last changed
/// </summary>
public class Cloud : MovableModel3D {
   private Random random;

   // Constructor
   public Cloud(Stage stage, string label, string meshFile)
      : base(stage, label, meshFile)
      {
      random = new Random();     
      } 
  
   public override void Update(GameTime gameTime) {
      foreach (Object3D obj in instance) {
         obj.Yaw = 0.0f;
         if (random.NextDouble() < 0.34) {
            obj.Yaw = 0.01f;
            obj.updateMovableObject();
            }
         }
      base.Update(gameTime);
      }

   }}
