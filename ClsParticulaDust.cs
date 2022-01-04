using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace IP3D
{
    class ClsParticulaDust
    {
        public Vector3 pos, vel;
        float massa;
        ClsSphere sphere;
        public float timer = 0;
       
        public ClsParticulaDust(GraphicsDevice device,Vector3 pos, Vector3 vel,float massa) // Posição inicial e velocidade inicial 
        {
            this.pos = pos;
            this.vel = vel;
            this.massa = massa;
            this.sphere = new ClsSphere(device, pos, Color.ForestGreen, 0.02f);
        }

        public void Update(GameTime gameTime, List<Vector3>forcas,List<Vector3> aceleracao) 

        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Calcular varias forcas e varias acelerações
            Vector3 forcatotal = Vector3.Zero;
            foreach(Vector3 forca in forcas)
            {
                forcatotal += forca;
            }
            Vector3 a = Vector3.Zero;
            a = forcatotal / massa;
            foreach (Vector3 acelerar in aceleracao)
            {
                a += acelerar;
            }
            vel = vel + a * (float)gameTime.ElapsedGameTime.TotalSeconds;
            pos = pos + vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
            sphere.Update(pos);
        }

        public void draw(GraphicsDevice device, Matrix view, Matrix projection) 
        {
            sphere.Draw(device, view, projection);
        }
    }
}
