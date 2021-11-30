using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D
{
    class ClsBullet
    {
        ClsTank tank;
        public Model mymeshbullet;
        public Vector3 position;

        public ClsBullet(ClsTank tank, Model bullet, Vector3 position)
        {
            this.tank = tank;
            this.mymeshbullet = bullet;
            this.position = position;
        }

        public void Update(GameTime gameTime)
        {
            position = getVelocityandPosition(gameTime);
        }


        public Vector3 getVelocityandPosition(GameTime gameTime)
        {
            //Vector3 posBalaMundo = tank.dirBala;
            Vector3 velocity = Vector3.Zero;
            Vector3 gravity = -Vector3.UnitY;
            Vector3 v0 = Vector3.Zero;
            Vector3 DeltaPosition = Vector3.Zero;
            Vector3 dirBala = tank.boneTransforms[10].Backward; // Direção do canhão
            dirBala.Normalize();
            Vector3 posicaoBullet = Vector3.Zero;
            velocity = v0 + gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity *= dirBala;
            v0 = velocity;

            //posicaoBullet = posBalaMundo + velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //posBalaMundo = posicaoBullet;
            return posicaoBullet;
        }


        public void Draw(GraphicsDevice device, Matrix view, Matrix projection,Vector3 pos)
        {
            
            foreach (ModelMesh mesh in mymeshbullet.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World =  Matrix.CreateTranslation(pos);
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                }
                // Draw each mesh of the model
                mesh.Draw();
            }
        }
    }
}
