using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D {

    public  class ClsGameObject {

        public enum Layer {blockable, projectile}
        public string name;
        public Layer layer;
        public Vector3 position;
        protected Matrix scale;
        public ClsCircleCollider collider;

        public ClsGameObject(Vector3 position, Matrix scale, Layer collisionLayer, float radius, string name) {
            this.position = position;
            this.scale = scale;
            this.name = name;
            collider = new ClsCircleCollider(this, position,radius);
        }

        public virtual void Update(){
            

        }


        
        //protected abstract float GetRadius();
    }
}