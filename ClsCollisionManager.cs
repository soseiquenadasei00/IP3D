using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace IP3D {

    public class ClsCollisionManager {
        public static ClsCollisionManager instance;

        public List<ClsGameObject> objects;

        public ClsCollisionManager(){
            instance = this;
            objects = new List<ClsGameObject>();
        }


        public List<ClsGameObject> IsColliding(ClsCircleCollider collider) {
            List<ClsGameObject> collisionList = new List<ClsGameObject>();
            foreach(ClsGameObject go in objects) {
                if (Vector3.Distance(collider.center, go.position) < collider.radius + go.collider.radius) {
                    collisionList.Add(go);
                }
            }
            return collisionList;
        }

        public ClsGameObject ReturnCollidedObject(ClsCircleCollider collider)
        {
            foreach (ClsGameObject go in objects)
            {
                if (Vector3.Distance(collider.center, go.position) < collider.radius + go.collider.radius) return go;                
            }
            return null;
        }

        public bool CheckFutureCollision(ClsCircleCollider collider, Vector3 position){
            foreach(ClsGameObject go in objects) {
                if (Vector3.Distance(position, go.position)  < collider.radius + go.collider.radius 
                    && go.name != collider.gameObject.name  &&
                    go.layer == ClsGameObject.Layer.blockable) return true;
            }
            return false;
        }


    }



}