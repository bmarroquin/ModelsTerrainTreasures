using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AGMGSK {
    class Treasure:Model3D {

        private bool tagged = false;
        private float radians = .01f;
        public Treasure(Stage stage, String label, String meshFile):base(stage, label, meshFile) {

            this.isCollidable = true;
            int spacing = stage.Terrain.Spacing;
            Terrain terrain = stage.Terrain;
            addObject(new Vector3(256*spacing, terrain.surfaceHeight(256,256)+200,256*spacing), Vector3.Up, 0,new Vector3(150,150,150));
        }

        public override void Update(GameTime gameTime) {
            foreach (Object3D obj in instance) {
                obj.rotateObject(0, radians, 0);
            }
            
            base.Update(gameTime);
        }


        public void Tag(){
            tagged = true;
        }
    
    
    }
}
