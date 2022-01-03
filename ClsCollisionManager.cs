using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace IP3D {

    public class ClsCollisionManager{

        public static ClsCollisionManager instance;
        public ClsTank tank;
        public ClsTankBoid tankboid;
        public List<ClsBullet> bullets;

        public ClsCollisionManager()
        {
            instance = this;
            instance.tank = tank;
            instance.tankboid = tankboid;
            bullets = new List<ClsBullet>();
        }

        public bool CheckTankCollision() => 
            Vector3.Distance(tank.position, tankboid.position) <= tank.collider.radius + tankboid.collider.radius;

        //backup method to unstuck tanks
        public bool MovingAway(Vector3 futureposition) =>
            Vector3.Distance(tank.collider.center, tankboid.collider.center) < Vector3.Distance(futureposition, tankboid.collider.center);

        public bool CheckBulletCollision()
        {
            return false;
        }



    }

}