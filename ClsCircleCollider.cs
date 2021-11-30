using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D {
    public class ClsCircleCollider{
        public float radius;
        public Vector3 center;
        public ClsGameObject gameObject;

        public ClsCircleCollider(ClsGameObject gameObject, Vector3 center, float radius) {
            this.radius = radius;
            this.center = center;
            this.gameObject = gameObject;
            ClsCollisionManager.instance.objects.Add(this.gameObject);
        }


    }
}