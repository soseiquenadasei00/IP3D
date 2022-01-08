using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IP3D
{
    class SystemParticulaDust
    {

        List<ClsParticulaDust> particulas;
        System.Random r;
        private BasicEffect effect;
        private float particlesPerSec = 50;
        private ClsTank tank;
  
        GraphicsDevice device;
        ClsTerreno terreno;

        // Sistema de particulas, lista de particulas 
        public SystemParticulaDust(GraphicsDevice device, ClsTank tank,ClsTerreno terreno)
        {
            r = new Random(1);
            this.tank = tank;
            this.device = device;
            particulas = new List<ClsParticulaDust>();
            effect = new BasicEffect(device);
            effect.LightingEnabled = false;
            effect.TextureEnabled = false;
            this.terreno = terreno;
           
        }

        public void Update(GameTime gametime)
        {
            if (tank.actual== ClsTank.tankState.move) Generate((int)(Math.Round(particlesPerSec * (float)gametime.ElapsedGameTime.TotalSeconds)));
             killParticula(gametime);
           
        }

        private void Generate(int particlesQnt)
        {
            for (int i = 0; i < particlesQnt; i++)
            {
                particulas.Add(geradorLeft(0.5f,MathHelper.ToRadians(r.Next(360) * 2 * MathHelper.Pi)));
                particulas.Add(geradorRight(0.5f,MathHelper.ToRadians(r.Next(360) * 2 * MathHelper.Pi)));
            }
        }

        public ClsParticulaDust geradorLeft(float raio, float randomAngle)
        {
            Vector3 posRodaLocal = tank.tankModel.Bones["l_back_wheel_geo"].Transform.Translation;
            Vector3 posRodaMundo = Vector3.Transform(posRodaLocal, tank.boneTransforms[0]);
            Vector3 pos;
            Vector3 vel;
            Vector3 offset = new Vector3(0f, 0f, -0.15f);

            pos = offset + posRodaMundo + raio * (float)r.NextDouble() * new Vector3(MathF.Cos(randomAngle), 0, MathF.Sin(randomAngle));
            vel = new Vector3((float)r.NextDouble() * 2 - 1, (float)(10f * r.NextDouble()), (float)r.NextDouble() * 2 - 1);
            float massa = 1;
            ClsParticulaDust novaParticula = new ClsParticulaDust(device,pos, vel, massa);

            return novaParticula;
        }
        public ClsParticulaDust geradorRight(float raio, float randomAngle)
        {
            Vector3 posRodaLocal = tank.tankModel.Bones["r_back_wheel_geo"].Transform.Translation;
            Vector3 posRodaMundo = Vector3.Transform(posRodaLocal, tank.boneTransforms[0]);
            Vector3 pos;
            Vector3 vel;
            Vector3 offset = new Vector3(0f, 0f, -0.15f);

            pos = offset + posRodaMundo + raio * (float)r.NextDouble() * new Vector3(MathF.Cos(randomAngle), 0, MathF.Sin(randomAngle));
            vel = new Vector3((float)r.NextDouble() * 2 - 1, (float)(8f * r.NextDouble()), (float)r.NextDouble() * 2 - 1);
            float massa = 1;
            ClsParticulaDust novaParticula = new ClsParticulaDust(device, pos, vel, massa);

            return novaParticula;
        }

        public void killParticula(GameTime gametime)
        {
            List<Vector3> acc = new List<Vector3>();
            List<Vector3> f = new List<Vector3>();
            acc.Add(new Vector3(0, -9.8f, 0));
            

            for (int i = particulas.Count - 1; i > 0; i--)
            {
                if (particulas[i].timer > 2 || particulas[i].pos.Y < 0)
                    particulas.RemoveAt(i);
                else
                    particulas[i].Update(gametime, f, acc);
            }
        }
        public void Draw(GraphicsDevice device,Matrix view,Matrix projection)
        {
            foreach (ClsParticulaDust particula in particulas)
            {
                particula.draw(device,view,projection);
            }
        }

    }
}
