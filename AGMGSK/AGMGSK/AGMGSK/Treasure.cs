using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;

namespace AGMGSK {
    public class Treasure:Model3D {
        public List<Object3D> taggedTreasures;
        private float radians = .01f;
        public Treasure(Stage stage, String label, String meshFile):base(stage, label, meshFile) {
            taggedTreasures = new List<Object3D>();
            this.isCollidable = true;
            int spacing = stage.Terrain.Spacing;
            Terrain terrain = stage.Terrain;
            addObject(new Vector3(256*spacing, terrain.surfaceHeight(256,256)+200,256*spacing), Vector3.Up, 0,new Vector3(150,150,150));
            addObject(new Vector3(75 * spacing, terrain.surfaceHeight(75,75) + 200, 75 * spacing), Vector3.Up, 0, new Vector3(150, 150, 150));
            addObject(new Vector3(500 * spacing, terrain.surfaceHeight(500,500) + 200, 500 * spacing), Vector3.Up, 0, new Vector3(150, 150, 150));
            addObject(new Vector3(56 * spacing, terrain.surfaceHeight(56, 467) + 200, 467 * spacing), Vector3.Up, 0, new Vector3(150, 150, 150));
        }

        public override void Update(GameTime gameTime) {
            foreach (Object3D obj in instance) {
                obj.rotateObject(0, radians, 0);
            }
            
            base.Update(gameTime);
        }


        public void Tag(Object3D t){
            taggedTreasures.Add(t);
        }

        public override void Draw(GameTime gameTime) {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            foreach (Object3D obj3d in instance) {
                if (taggedTreasures.Contains(obj3d)) continue;
                foreach (ModelMesh mesh in model.Meshes) {
                    model.CopyAbsoluteBoneTransformsTo(modelTransforms);
                    foreach (BasicEffect effect in mesh.Effects) {
                        effect.EnableDefaultLighting();
                        if (stage.Fog) {
                            effect.FogColor = Color.CornflowerBlue.ToVector3();
                            effect.FogStart = stage.FogStart;
                            effect.FogEnd = stage.FogEnd;
                            effect.FogEnabled = true;
                        }
                        else effect.FogEnabled = false;
                        effect.DirectionalLight0.DiffuseColor = stage.DiffuseLight;
                        effect.AmbientLightColor = stage.AmbientLight;
                        effect.DirectionalLight0.Direction = stage.LightDirection;
                        effect.DirectionalLight0.Enabled = true;
                        effect.View = stage.View;
                        effect.Projection = stage.Projection;
                        effect.World = modelTransforms[mesh.ParentBone.Index] * obj3d.Orientation;
                    }
                    mesh.Draw();
                }
                // draw the bounding sphere with blending ?
                if (stage.DrawBoundingSpheres && IsCollidable) {
                    foreach (ModelMesh mesh in stage.BoundingSphere3D.Meshes) {
                        model.CopyAbsoluteBoneTransformsTo(modelTransforms);
                        foreach (BasicEffect effect in mesh.Effects) {
                            effect.EnableDefaultLighting();
                            if (stage.Fog) {
                                effect.FogColor = Color.CornflowerBlue.ToVector3();
                                effect.FogStart = 50;
                                effect.FogEnd = 500;
                                effect.FogEnabled = true;
                            }
                            else effect.FogEnabled = false;
                            effect.DirectionalLight0.DiffuseColor = stage.DiffuseLight;
                            effect.AmbientLightColor = stage.AmbientLight;
                            effect.DirectionalLight0.Direction = stage.LightDirection;
                            effect.DirectionalLight0.Enabled = true;
                            effect.View = stage.View;
                            effect.Projection = stage.Projection;
                            effect.World = obj3d.ObjectBoundingSphereWorld * modelTransforms[mesh.ParentBone.Index];
                        }
                        stage.setBlendingState(true);
                        mesh.Draw();
                        stage.setBlendingState(false);
                    }
                }
            }
        }
      


    
    }
}
