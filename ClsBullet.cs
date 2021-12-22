using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace IP3D
{
    class ClsBullet : ClsGameObject {

        //references
        public enum BulletState { fired, stored  }
        public BulletState state;
        public Vector3 lastPosition;
        private ClsTank tank;

        //physics
        private float impulsoInicial;
        private Vector3 velocity;
        private Vector3 direction;
        private Matrix rotation;
        private int massa;

        //Model
        private Model bullet;
        private Matrix[] boneTransforms;

        public ClsBullet(ClsTank tank, Model bullet, Vector3 position, float impulsoInicial) : base(position, Matrix.CreateScale(1.0f), Layer.projectile, 0.5f, "bullet"){
            this.tank = tank;
            this.bullet = bullet;
            this.impulsoInicial = impulsoInicial;
            this.position = position;
            this.massa = 45;
            state = BulletState.stored;
            boneTransforms = new Matrix[bullet.Bones.Count];

        }
        //apenas 1 aceleracao == gravidade
        //forca = velocidade inicial em newton * direction
        public void Update(GameTime gameTime){
            Vector3 gravity = new Vector3(0.0f, -9.8f, 0.0f);
            Vector3 forcatotal = impulsoInicial * direction;
            Vector3 aceleracao = (forcatotal / massa) + gravity;
            velocity = velocity + aceleracao * (float)gameTime.ElapsedGameTime.TotalSeconds;
            lastPosition = position;
            position = position + velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //lidar com o model
            Matrix translacao = Matrix.CreateTranslation(position);
            bullet.Root.Transform = scale * rotation * translacao;
            bullet.CopyAbsoluteBoneTransformsTo(boneTransforms);
        }

        public void Fire(Vector3 position, Matrix rotation, Vector3 direction){
            state = BulletState.fired;
            this.position = position;
            this.direction = direction;
            this.rotation = rotation;
        }


        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            Console.WriteLine("Draw was called");
            foreach (ModelMesh mesh in bullet.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
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
