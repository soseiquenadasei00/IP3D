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
        public bool BoidMovingAway(Vector3 futureposition) =>
           Vector3.Distance(tank.collider.center, tankboid.collider.center) < Vector3.Distance(futureposition, tank.collider.center);

        public bool CheckBulletCollision()
        {
            foreach (ClsBullet bullet in bullets) {
                Vector3 bulletToTankAnterior = tankboid.position - bullet.lastPosition;
                Vector3 bulletToTankSeguinte = tankboid.position - bullet.position;
                float dotAnterior = Vector3.Dot(bullet.velocity, bulletToTankAnterior);
                float dotSeguinte = Vector3.Dot(bullet.velocity, bulletToTankSeguinte);
                if (dotAnterior > 0 && dotSeguinte < 0)   //houve interseção com a projeção do tanque na linha
                {
                    float lastPosToTank = MathF.Abs(Vector3.Distance(bullet.lastPosition, tankboid.position)); // b
                    float posToTank = MathF.Abs(Vector3.Distance(bullet.position, tankboid.position)); //a 
                    float lastPosToPos = MathF.Abs(Vector3.Distance(bullet.lastPosition, bullet.position)); //c
                    float sp = (posToTank + lastPosToTank + lastPosToPos) / 2;
                    float area = MathF.Sqrt(sp * (sp - posToTank) * (sp - lastPosToTank) * (sp - lastPosToPos));
                    float distance = 2 * area / lastPosToPos;
                    return distance < tankboid.collider.radius;
                }
            }
            return false;
        }
    }
}