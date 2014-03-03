/*  
    Copyright (C) 2014 G. Michael Barnes
 
    The file Camera.cs is part of AGMGSKv5 a port of AGXNASKv4 from
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
/// A viewpoint in the stage.  Cameras have a viewMatrix that is its position and orientation 
/// in the stage.  There are four cameras:  TopDownCamera, FirstCamera, FollowCamera, AboveCamera.
/// The stage has one TopDownCamera named "Whole stage camera".  This is the starting camera
/// for SK565.  The TopDownCamera is stationary.  
/// The other three cameras are associated with an agent.  They are the agent's
/// first (person view), follow (person view), and above (look down on agent view).  
/// Player handles user input to call stage.nextCamera() to selects the currentCamera, 
/// update the agent's current avatarCamera, update currentCamera's viewMatrix 
/// with updateViewMatrix().
/// 
/// 1/25/2012 last changed
/// </summary>

public class Camera  {
   public enum CameraEnum { TopDownCamera, FirstCamera, FollowCamera, AboveCamera }
   Object3D agent;
   private int terrainCenter, offset = 300;
   private Matrix viewMatrix;
   private string name;
   private Stage scene;
   CameraEnum cameraCase;

   public Camera(Stage aScene, CameraEnum cameraType) { 
      name = "Whole stage";
      scene = aScene;
      cameraCase = cameraType;
      terrainCenter = scene.TerrainSize / 2;
      updateViewMatrix();
      }

   public Camera(Stage aScene, Object3D anAgentObject, CameraEnum cameraType) {
      scene = aScene;
      agent = anAgentObject;
      cameraCase = cameraType;
      }
      
   // Properties

   public string Name {
      get { return name; }
      set { name = value; }}
         
   public Matrix ViewMatrix {
      get { return viewMatrix;}}

   /// <summary>
   /// When an agent updates its place in the stage it calls its currentCamera's
   /// updateViewMatrix() to place the camera's viewMatrix.
   /// </summary>
   public void updateViewMatrix() { 
      switch (cameraCase) {
         case CameraEnum.TopDownCamera:
         viewMatrix = Matrix.CreateLookAt(
            new Vector3(terrainCenter, scene.FarYon - 50, terrainCenter),
            new Vector3(terrainCenter, 0, terrainCenter), 
            new Vector3(0, 0, -1));
            break;
         case CameraEnum.FirstCamera:
            viewMatrix = Matrix.CreateLookAt(agent.Translation,
               agent.Translation + agent.Forward, agent.Orientation.Up);
            viewMatrix *= Matrix.CreateTranslation(0, -offset, 0);
            break;
         case CameraEnum.FollowCamera:
            viewMatrix = Matrix.CreateLookAt(agent.Translation,
               agent.Translation + agent.Forward, agent.Orientation.Up);
            viewMatrix *= Matrix.CreateTranslation(0, -2 * offset, -8 * offset);
            break;
         case CameraEnum.AboveCamera:
            viewMatrix = Matrix.CreateLookAt(
               new Vector3(agent.Translation.X, agent.Translation.Y + 3 * offset,
               agent.Translation.Z), 
               agent.Translation, new Vector3(0, 0, -1));
            break;
         }
      }           
   }
}
