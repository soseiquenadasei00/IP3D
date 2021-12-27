using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IP3D
{
    class SystemParticula
    {
        
        List<ClsParticula>particulas;
        System.Random r;
  
        private BasicEffect effect;
        private VertexPositionColor[] vertex = new VertexPositionColor[30000];
        private float particlesPerSec = 15000;
        private ClsTerreno terreno;

        // Sistema de particulas, lista de particulas 
        public SystemParticula(GraphicsDevice device, ClsTerreno terreno)
        {
            r = new System.Random(1);
            this.terreno = terreno;
            particulas = new List<ClsParticula>();
            effect = new BasicEffect(device);
            effect.LightingEnabled = false;
            effect.TextureEnabled = false;
        }

        public void Update(GameTime gametime)
        {
            Generate((int)(Math.Round(particlesPerSec * (float)gametime.ElapsedGameTime.TotalSeconds)));
            killParticula(gametime);
            for (int i = 0; i < particulas.Count; i++)
            {
                float desvio = (float)r.NextDouble()*0.5f;
                Vector3 vel = particulas[i].vel;
                vel.Normalize();
                vel *= desvio;
                vertex[2 * i + 0] = new VertexPositionColor(particulas[i].pos, Color.Blue);
                vertex[2 * i + 1] = new VertexPositionColor(particulas[i].pos -vel, Color.Blue);
            }
        }

        private void Generate(int particlesQnt)
        {
            for (int i = 0; i < particlesQnt; i++)
            {
                particulas.Add(gerador(new Vector3(terreno.width/2, 50f, terreno.height / 2),
                                            (terreno.width*terreno.height)/2,
                                            MathHelper.ToRadians(r.Next(360) * 2 * MathHelper.Pi)));
            }
        }
       
        public ClsParticula gerador(Vector3 centro, float raio, float randomAngle)
        {
            Vector3 pos;
            Vector3 vel;

            pos = centro + raio * (float)r.NextDouble() * new Vector3(MathF.Cos(randomAngle), 0, MathF.Sin(randomAngle));
            vel = new Vector3((float)r.NextDouble()*2-1,(float)(-10f - 50f *r.NextDouble()),(float)r.NextDouble()*2-1);
            float massa = 1;
            ClsParticula novaParticula = new ClsParticula(pos, vel, massa);

            return novaParticula;
        }

        public void killParticula(GameTime gametime) 
        {
            List<Vector3> acc = new List<Vector3>();
            List<Vector3> f = new List<Vector3>();
            acc.Add(new Vector3(0, -9.8f, 0));

            for (int i = particulas.Count-1; i > 0; i--)
            {
                if (particulas[i].pos.Y < 0 || particulas[i].pos.X < 0 || particulas[i].pos.X > terreno.width || particulas[i].pos.Z < 0 || particulas[i].pos.Z > terreno.height)
                    particulas.RemoveAt(i);
                else
                    particulas[i].Update(gametime, f, acc);
            }
        }
        public void Draw(GraphicsDevice device, Matrix projection, Matrix view)
        {
            effect.Projection = projection;
            effect.View = view;
            effect.World = Matrix.Identity;
            effect.CurrentTechnique.Passes[0].Apply();
            effect.VertexColorEnabled = true;
            device.DrawUserPrimitives(PrimitiveType.LineList, vertex, 0, particulas.Count / 2);
        }

    }
}
