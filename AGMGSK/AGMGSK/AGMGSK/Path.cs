/*  
    Copyright (C) 2014 G. Michael Barnes
 
    The file Path.cs is part of AGMGSKv5 a port of AGXNASKv4 from
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
using System.IO;
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
/// Path represents a collection of NavNodes that a movable Object3D can traverse.
/// Paths have a PathType of
/// <list type="number"> SINGLE, traverse list once, set done to true </list>
/// <list type="number"> REVERSE, loop by reversing path after each traversal </list>
/// <list type=="number"> LOOP, loop by starting at paths first node again </list>
/// 
/// 2/14/2012 last changed
/// </summary>
public class Path : DrawableGameComponent {
   public enum PathType  {SINGLE, REVERSE, LOOP};
   private List<NavNode> node;
   private int nextNode;
   private PathType pathType;
   private bool done;
   private Stage stage;

   /// <summary>
   /// Create a path
   /// </summary>
   /// <param name="theStage"> "world's stage" </param>
   /// <param name="apath"> collection of nodes in path</param>
   /// <param name="aPathType"> SINGLE, REVERSE, or LOOP path traversal</param>
   public Path(Stage theStage, List<NavNode> aPath, PathType aPathType) : base(theStage) {
      node = aPath;
      nextNode = 0;
      pathType = aPathType;
      stage = theStage;
      done = false;
      }

   /// <summary>
   /// Create a path from XZ nodes defined in a pathFile.
   /// The file must be accessible from the executable environment.
   /// </summary>
   /// <param name="theStage"> "world's stage" </param>
   /// <param name="aPathType"> SINGLE, REVERSE, or LOOP path traversal</param>
   /// <param name="pathFile"> text file, each line a node of X Z values, separated by a single space </x></param>
   public Path(Stage theStage, PathType aPathType, string pathFile)  : base(theStage) {
      node = new List<NavNode>();
      stage = theStage;
      nextNode = 0;
      pathType = aPathType;
      done = false;
      // read file
      using (StreamReader fileIn = File.OpenText(pathFile)) {
         int x, z;
         string line;
         string[] tokens;
         line = fileIn.ReadLine();
         do {
            tokens = line.Split(new char[] {});  // use default separators
            x = Int32.Parse(tokens[0]);  
            z = Int32.Parse(tokens[1]);
            node.Add(new NavNode(new Vector3(x, 0, z), NavNode.NavNodeEnum.WAYPOINT));  
            line = fileIn.ReadLine();
            } while (line != null);
         }
      }

   // Properties

   public int Count { get { return node.Count; }}   

   public bool Done { get { return done; }}

   /// <summary>
   /// Gets the next node in the path using path's PathType
   /// </summary>
   public NavNode NextNode {
      get {
         NavNode n = null;
         if (node.Count > 0 && node.Count - 1 > nextNode) { // take next step on path
            n = node[nextNode];
            nextNode++; }
         // at end of current path, decide what to do:  stop, reverse path, loop?
         else if (node.Count-1 == nextNode &&  pathType == PathType.SINGLE) {
               n = node[nextNode];
               done = true; }  
            else if (node.Count -1 == nextNode && pathType == PathType.REVERSE) {
               node.Reverse();
               nextNode = 0;  // set to next node
               n = node[nextNode];
               nextNode++; }
            else if (node.Count - 1 == nextNode && pathType == PathType.LOOP) {
               n = node[nextNode]; 
               nextNode = 0; }    
         return n; }
      }   


   // Methods

   public override void Draw(GameTime gameTime) {
      Matrix[] modelTransforms = new Matrix[stage.WayPoint3D.Bones.Count];
      foreach(NavNode navNode in node) {
         // draw the Path markers
            foreach (ModelMesh mesh in stage.WayPoint3D.Meshes) {
               stage.WayPoint3D.CopyAbsoluteBoneTransformsTo(modelTransforms);
               foreach (BasicEffect effect in mesh.Effects) {
                  effect.EnableDefaultLighting();
                  if (stage.Fog) {
                     effect.FogColor = Color.CornflowerBlue.ToVector3();
                     effect.FogStart = stage.FogStart;
                     effect.FogEnd = stage.FogEnd;
                     effect.FogEnabled = true;
                  }
                  else effect.FogEnabled = false;
                  effect.DirectionalLight0.DiffuseColor = navNode.NodeColor;
                  effect.AmbientLightColor = navNode.NodeColor;
                  effect.DirectionalLight0.Direction = stage.LightDirection;
                  effect.DirectionalLight0.Enabled = true;
                  effect.View = stage.View;
                  effect.Projection = stage.Projection;
                  effect.World = Matrix.CreateTranslation(navNode.Translation) * modelTransforms[mesh.ParentBone.Index];
               }
               stage.setBlendingState(true);
               mesh.Draw();
               stage.setBlendingState(false);
            }
         }
      }
      
   }
}
