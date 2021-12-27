using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace IP3D
{
    class ClsParticula
    {
        public Vector3 pos, vel;
        float massa;

        public ClsParticula(Vector3 pos, Vector3 vel,float massa) // Posição inicial e velocidade inicial 
        {
            this.pos = pos;
            this.vel = vel;
            this.massa = massa;
        }

        public void Update(GameTime gameTime, List<Vector3>forcas,List<Vector3> aceleracao) 
        {
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
        }
    }
}
