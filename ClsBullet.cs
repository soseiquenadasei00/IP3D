using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace IP3D
{
    class ClsBullet : ClsGameObject
    {
        public enum BulletState
        {
            fired, stored
        }
        public BulletState state;
        private ClsTank tank;
        private Model mymeshbullet;
        public Vector3 lastPosition;
        public float velocity = 0.5f;
        public Vector3 direction;
        public Matrix[] boneTransforms;

        public ClsBullet(ClsTank tank, Model bullet, Vector3 position) : base(position, Matrix.CreateScale(1.0f), Layer.projectile, 0.5f, "bullet"){
            this.tank = tank;
            this.mymeshbullet = bullet;
            state = BulletState.stored;
            boneTransforms = new Matrix[mymeshbullet.Bones.Count];
        }

        public void Update(GameTime gameTime) {
            Vector3 gravity = new Vector3(0.0f, -9.8f, 0.0f);

            Vector3 DeltaPosition = position + direction *( (float)gameTime.ElapsedGameTime.TotalSeconds / 5);
            Console.WriteLine(position);
            lastPosition = position;
            position = DeltaPosition;
            Matrix translation = Matrix.CreateTranslation(position);
            mymeshbullet.Root.Transform = mymeshbullet.Root.Transform * translation;
            mymeshbullet.CopyAbsoluteBoneTransformsTo(boneTransforms);
        }

        public void Fire(Vector3 position, Vector3 direction){
            state = BulletState.fired;
            this.position = position;
            this.direction = direction;
        }


        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            Console.WriteLine("Draw was called");
            foreach (ModelMesh mesh in mymeshbullet.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;
                    
                }
                // Draw each mesh of the model
                mesh.Draw();
            }
        }
    }
}
