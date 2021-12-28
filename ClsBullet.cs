using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace IP3D
{
    class ClsBullet : ClsGameObject {

        //references
        public enum BulletState { fired, stored, travelling }
        public BulletState state;
        public Vector3 lastPosition;
        private ClsTank tank;

        //physics
        private float impulsoInicial;
        private Vector3 forcaInicial;
        Vector3 forcatotal = Vector3.Zero;
        private Vector3 velocity;
        private Vector3 direction;
        private int massa = 45;
        private ClsSphere sphere;

        public ClsBullet(GraphicsDevice device, ClsTank tank, Vector3 position, float impulsoInicial) : base(position, Matrix.CreateScale(1.0f), Layer.projectile, 0.5f, "bullet"){
            this.tank = tank;
            this.impulsoInicial = impulsoInicial;
            this.position = position;
            state = BulletState.stored;
            sphere = new ClsSphere(device, position, Color.Black, 0.5f);
        }
        //apenas 1 aceleracao == gravidade` 
        //forca = velocidade inicial em newton * direction
        public void Update(GameTime gameTime){
            Vector3 gravity = new Vector3(0.0f, -9.8f, 0.0f);
            Vector3 forcatotal = direction;
            if (state == BulletState.fired)
            {
                forcatotal += forcaInicial;
                state = BulletState.travelling;
            }

            Vector3 aceleracao = (forcatotal / massa) + gravity;
            velocity += aceleracao * (float)gameTime.ElapsedGameTime.TotalSeconds;
            lastPosition = position;
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            sphere.Update(position);
        }

        public void Fire(Vector3 direction){
            state = BulletState.fired;
            this.direction = direction;
            direction.Normalize();
            this.forcaInicial = impulsoInicial * direction;
        }


        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            Console.WriteLine(position);
            sphere.Draw(device, view, projection);
        }
    }
}
