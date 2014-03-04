/*  
    Copyright (C) 2014 G. Michael Barnes
 
    The file NPAgent.cs is part of AGMGSKv5 a port of AGXNASKv4 from
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
    /// A non-playing character that moves.  Override the inherited Update(GameTime)
    /// to implement a movement (strategy?) algorithm.
    /// Distribution NPAgent moves along an "exploration" path that is created by
    /// method makePath().  The exploration path is traversed in a reverse path loop.
    /// Paths can also be specified in text files of Vector3 values, see alternate
    /// Path class constructor.
    /// 
    /// 2/2/2014 last changed
    /// </summary>

    public class NPAgent : Agent {
        private NavNode nextGoal;
        private Path path;
        private int snapDistance = 20;
        private int turnCount = 0;
        private bool interrupt;
        NavNode currentGoal;
        Object3D targetTreasure = null;
        /// <summary>
        /// Create a NPC. 
        /// AGXNASK distribution has npAgent move following a Path.
        /// </summary>
        /// <param name="theStage"> the world</param>
        /// <param name="label"> name of </param>
        /// <param name="pos"> initial position </param>
        /// <param name="orientAxis"> initial rotation axis</param>
        /// <param name="radians"> initial rotation</param>
        /// <param name="meshFile"> Direct X *.x Model in Contents directory </param>

        public NPAgent(Stage theStage, string label, Vector3 pos, Vector3 orientAxis,
           float radians, string meshFile)
            : base(theStage, label, pos, orientAxis, radians, meshFile) {  // change names for on-screen display of current camera
            first.Name = "npFirst";
            follow.Name = "npFollow";
            above.Name = "npAbove";
            // path is built to work on specific terrain
            interrupt = false;
            path = new Path(stage, makePath(), Path.PathType.REVERSE); // continuous search path
            stage.Components.Add(path);
            nextGoal = path.NextNode;  // get first path goal
            agentObject.turnToFace(nextGoal.Translation);  // orient towards the first path goal
        }

        /// <summary>
        /// Procedurally make a path for NPAgent to traverse
        /// </summary>
        /// <returns></returns>
        private List<NavNode> makePath() {
            List<NavNode> aPath = new List<NavNode>();
            int spacing = stage.Spacing;
            // make a simple path, show how to set the type of the NavNode outside of construction.
            NavNode n;
            // path section near Player for testing 
            n = new NavNode(new Vector3(505 * spacing, stage.Terrain.surfaceHeight(505, 505), 505 * spacing));
            n.Navigatable = NavNode.NavNodeEnum.PATH;
            aPath.Add(n);
            n = new NavNode(new Vector3(500 * spacing, stage.Terrain.surfaceHeight(500, 500), 500 * spacing));
            n.Navigatable = NavNode.NavNodeEnum.VERTEX;
            aPath.Add(n);
            aPath.Add(new NavNode(new Vector3(495 * spacing, stage.Terrain.surfaceHeight(495, 495), 495 * spacing),
                     NavNode.NavNodeEnum.A_STAR));
            aPath.Add(new NavNode(new Vector3(495 * spacing, stage.Terrain.surfaceHeight(495, 505), 505 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));
            aPath.Add(new NavNode(new Vector3(383 * spacing, stage.Terrain.surfaceHeight(383, 500), 500 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));
            // go by wall   
            aPath.Add(new NavNode(new Vector3(445 * spacing, stage.Terrain.surfaceHeight(445, 438), 438 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));
            aPath.Add(new NavNode(new Vector3(500 * spacing, stage.Terrain.surfaceHeight(500, 383), 383 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));
            aPath.Add(new NavNode(new Vector3(500 * spacing, stage.Terrain.surfaceHeight(500, 100), 100 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));
            aPath.Add(new NavNode(new Vector3(105 * spacing, stage.Terrain.surfaceHeight(105, 105), 105 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));
            aPath.Add(new NavNode(new Vector3(105 * spacing, stage.Terrain.surfaceHeight(105, 495), 495 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));
            // turning circle 
            aPath.Add(new NavNode(new Vector3(80 * spacing, stage.Terrain.surfaceHeight(80, 505), 505 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));
            aPath.Add(new NavNode(new Vector3(60 * spacing, stage.Terrain.surfaceHeight(60, 490), 490 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));
            aPath.Add(new NavNode(new Vector3(105 * spacing, stage.Terrain.surfaceHeight(105, 495), 495 * spacing),
                     NavNode.NavNodeEnum.WAYPOINT));
            return (aPath);
        }

        public void FindNearest(Treasure treasure) {
            if (interrupt) return;
            Object3D min = null;
            foreach (Object3D t in treasure.Instance) {
                if (treasure.taggedTreasures.Contains(t)) {
                    continue;
                }
                if (min == null) {
                    min = t;
                }
                else {
                    float testDistance = Vector3.Distance(agentObject.Translation, t.Translation);
                    float minDistance = Vector3.Distance(agentObject.Translation, min.Translation);
                    if (testDistance < minDistance) {
                        min = t;
                    }
                }
            }

            if (min == null) {
                return;
            }
            Interrupt(min.Translation);
            targetTreasure = min;
        }


        public void Interrupt(Vector3 pos){
            
                currentGoal = nextGoal;
                nextGoal = new NavNode(pos, NavNode.NavNodeEnum.WAYPOINT);
                agentObject.turnToFace(nextGoal.Translation);
                interrupt = true;
            
        }


        /// <summary>
        /// Simple path following.  If within "snap distance" of a the nextGoal (a NavNode) 
        /// move to the NavNode, get a new nextGoal, turnToFace() that goal.  Otherwise 
        /// continue making steps towards the nextGoal.
        /// </summary>
        public override void Update(GameTime gameTime) {
            stage.setInfo(15,
               string.Format("npAvatar:  Location ({0:f0},{1:f0},{2:f0})  Looking at ({3:f2},{4:f2},{5:f2})",
                  agentObject.Translation.X, agentObject.Translation.Y, agentObject.Translation.Z,
                  agentObject.Forward.X, agentObject.Forward.Y, agentObject.Forward.Z));
            stage.setInfo(16,
               string.Format("nextGoal:  ({0:f0},{1:f0},{2:f0})", nextGoal.Translation.X, nextGoal.Translation.Y, nextGoal.Translation.Z));
            // See if at or close to nextGoal, distance measured in the flat XZ plane
            float distance = Vector3.Distance(
               new Vector3(nextGoal.Translation.X, 0, nextGoal.Translation.Z),
               new Vector3(agentObject.Translation.X, 0, agentObject.Translation.Z));
            if (distance <= snapDistance) {
                stage.setInfo(17, string.Format("distance to goal = {0,5:f2}", distance));
                // snap to nextGoal and orient toward the new nextGoal 
                if (interrupt) {
                    interrupt = false;
                    nextGoal = currentGoal;
                    agentObject.turnToFace(nextGoal.Translation);
                    stage.treasure.Tag(targetTreasure);
                    targetTreasure = null;
                }
                else {
                    nextGoal = path.NextNode;
                    agentObject.turnToFace(nextGoal.Translation);
                    if (path.Done)
                        stage.setInfo(18, "path traversal is done");
                    else {
                        turnCount++;
                        stage.setInfo(18, string.Format("turnToFace count = {0}", turnCount));
                    }
                }
            }
            base.Update(gameTime);  // Agent's Update();
        }

        public override String ToString() {
            
            return "X: " + agentObject.Translation.X + " Y: " + agentObject.Translation.Y + " Z: " + agentObject.Translation.Z;
        }
    }
}
