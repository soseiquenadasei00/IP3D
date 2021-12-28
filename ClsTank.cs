using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D
{
    class ClsTank  : ClsGameObject{
        /* CONTROL ARRAY PASSED AS PARAMETER TO UPDATE METHOD:
        0 = TOWER LEFT
        1 = TOWER RIGHT
        2 = CANNON UP
        3 = CANNON DOWN
        4 = TANK LEFT
        5 = TANK RIGHT
        6 = TANK FORWARD
        7 = TANK BACKWARD
        */
        private ClsTerreno terreno;
        private Game1 game;
        //structure
        private Model tankModel;
        private ModelBone turretBone, cannonBone, leftBackWheelBone, rightBackWheelBone, leftFrontWheelBone,
                          rightFrontWheelBone, leftSteerBone, rightSteerBone, hatchBone;
        private Matrix leftSteerTranform, rightSteerTranform, cannonTransform, turretTransform;
        public Matrix[] boneTransforms;
        private float rotTower = 0;
        private float rotCanon = 0;
        private float rotTank = 0;

        private float wheelRotationAngle = 0;
        private float wheelRotationSpeed = 0;

        private List<ClsBullet> bullets = new List<ClsBullet>();
        private int cannonPower = 100000;
        private GraphicsDevice device;
        private float reloadTime = 2.0f;
        private float lastShotTime;
        private bool canFire;


        public ClsTank(GraphicsDevice device, Game1 game1, Model model, ClsTerreno terreno, Vector3 position, Matrix scale, float radius, string name) : base(position, scale, Layer.blockable, radius, name){
            this.scale     = scale;
            this.tankModel = model;
            this.terreno   = terreno;
            this.position  = position;
            this.name      = name;
            this.game      = game1;
            this.device    = device;
            leftBackWheelBone   = tankModel.Bones["l_back_wheel_geo"];
            rightBackWheelBone  = tankModel.Bones["r_back_wheel_geo"];

            leftFrontWheelBone  = tankModel.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = tankModel.Bones["r_front_wheel_geo"];

            leftSteerBone  = tankModel.Bones["l_steer_geo"];
            rightSteerBone = tankModel.Bones["r_steer_geo"];

            turretBone = tankModel.Bones["turret_geo"];
            cannonBone = tankModel.Bones["canon_geo"];
            hatchBone  = tankModel.Bones["hatch_geo"];

            turretTransform    = turretBone.Transform;
            cannonTransform    = cannonBone.Transform;
            leftSteerTranform  = leftSteerBone.Transform;
            rightSteerTranform = rightSteerBone.Transform;

            boneTransforms = new Matrix[tankModel.Bones.Count];
        }



        public void Update(GameTime gameTime, Keys[] controls){

            KeyboardState state = Keyboard.GetState();

            Matrix rotacao = Matrix.CreateFromYawPitchRoll(rotTank, 0f, 0f);
            Vector3 direcao = Vector3.Transform(-Vector3.UnitZ, rotacao);

            Vector3 normal = terreno.GetNormals(position.X, position.Z);
            normal.Normalize();
            Vector3 right = Vector3.Cross(direcao, normal);
            right.Normalize();
            Vector3 direcaoCorreta = Vector3.Cross(normal, right);
            direcaoCorreta.Normalize();

            #region Cannon/TowerControl

            if (state.IsKeyDown(controls[0])) rotTower += 45 * (float)gameTime.ElapsedGameTime.TotalSeconds; //tower left
            if (state.IsKeyDown(controls[1])) rotTower -= 45 * (float)gameTime.ElapsedGameTime.TotalSeconds; //tower right

            if (state.IsKeyDown(controls[2])) rotCanon -= 45 * (float)gameTime.ElapsedGameTime.TotalSeconds;//cannon up
            if (state.IsKeyDown(controls[3])) rotCanon += 45 * (float)gameTime.ElapsedGameTime.TotalSeconds;//cannon down
            if (rotCanon > 25) rotCanon = 25;
            if (rotCanon < -40) rotCanon = -40;


            Matrix rotationCanon = Matrix.CreateRotationX(MathHelper.ToRadians(rotCanon)) * cannonTransform;
            Matrix rotationTower = Matrix.CreateRotationY(MathHelper.ToRadians(rotTower)) * turretTransform;
            turretBone.Transform = rotationTower;
            cannonBone.Transform = rotationCanon;
            #endregion  

            #region TankControl
            //tank left
            if (state.IsKeyDown(controls[4]))
            {
                rotTank += 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                wheelRotationAngle += 0.45f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (wheelRotationAngle > 0) wheelRotationAngle -= 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (wheelRotationAngle > 0.45f) wheelRotationAngle = 0.45f;

            //tank right
            if (state.IsKeyDown(controls[5]))
            {
                rotTank -= 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                wheelRotationAngle -= 0.45f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (wheelRotationAngle < 0) wheelRotationAngle += 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (wheelRotationAngle < -0.45f) wheelRotationAngle = -0.45f;

            Vector3 futurePosition = position;
            //forward
            if (state.IsKeyDown(controls[6]))
            {
                futurePosition += -direcao * 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                wheelRotationSpeed += 10.0f;
            }
            if (wheelRotationSpeed > 10.0f) wheelRotationSpeed = 10.0f;


            //backward
            if (state.IsKeyDown(controls[7]))
            {
                futurePosition -= -direcao * 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                wheelRotationSpeed -= 10.0f;
            }
            if (wheelRotationSpeed < -10.0f) wheelRotationSpeed = -10.0f;
            if (!(state.IsKeyDown(controls[7]) || state.IsKeyDown(controls[6]))) wheelRotationSpeed = 0.0f;

            #endregion

            #region wheelAnim 
            Matrix wheelSpeed = Matrix.CreateRotationX(MathHelper.ToRadians(wheelRotationSpeed));
            Matrix wheelRotation = Matrix.CreateRotationY(wheelRotationAngle);

            //turn
            leftSteerBone.Transform = wheelRotation * leftSteerTranform;
            rightSteerBone.Transform = wheelRotation * rightSteerTranform;

            //rotate
            rightBackWheelBone.Transform = wheelSpeed * rightBackWheelBone.Transform;
            rightFrontWheelBone.Transform = wheelSpeed * rightFrontWheelBone.Transform;
            leftBackWheelBone.Transform = wheelSpeed * leftBackWheelBone.Transform;
            leftFrontWheelBone.Transform = wheelSpeed * leftFrontWheelBone.Transform;

            #endregion


            rotacao.Up = normal;
            rotacao.Forward = direcaoCorreta;
            rotacao.Right = right;

            //collision
            if (!ClsCollisionManager.instance.CheckFutureCollision(this.collider, futurePosition))
            {
                position = futurePosition;
            }
            position.Y = terreno.GetHeight(position.X, position.Z);

            //fire
            if (canFire && state.IsKeyDown(controls[8]))
            {
                Fire();
                canFire = false;
                lastShotTime = (float)gameTime.TotalGameTime.TotalSeconds;
            }
            for (int i = 0; i < bullets.Count; i++)
            {
                if (bullets[i].state != ClsBullet.BulletState.stored)
                {
                    bullets[i].Update(gameTime);
                }
                if (bullets[i].state == ClsBullet.BulletState.travelling)
                {
                    if (   bullets[i].position.X <= 0 
                        || bullets[i].position.Z <= 0
                        || bullets[i].position.X >= terreno.width
                        || bullets[i].position.Z >= terreno.height
                        || terreno.GetHeight(bullets[i].position.X,bullets[i].position.Z) >= bullets[i].position.Y) bullets.RemoveAt(i);
                    
                }
            }
            if ((float)gameTime.TotalGameTime.TotalSeconds - lastShotTime > reloadTime) canFire = true;

            Matrix translation = Matrix.CreateTranslation(position);
            tankModel.Root.Transform = scale * rotacao * translation;
            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);
        }

        public void Fire()
        {
            Vector3 cannonPosition = Vector3.Transform(cannonTransform.Translation, tankModel.Root.Transform);
            ClsBullet newBullet = new ClsBullet(device, this, cannonPosition, cannonPower);
            bullets.Add(newBullet);

            Matrix bulletRot = Matrix.CreateFromYawPitchRoll(rotTower, -rotCanon, 0.0f);
            Vector3 direcao = Vector3.Transform(-Vector3.UnitZ, bulletRot);
            Vector3 normal = terreno.GetNormals(position.X, position.Z);
            normal.Normalize();
            Vector3 right = Vector3.Cross(direcao, normal);
            right.Normalize();
            Vector3 direcaoCorreta = Vector3.Cross(normal, right);
            direcaoCorreta.Normalize();
            newBullet.Fire(direcaoCorreta);
           
        }
        
        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in tankModel.Meshes)
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


            foreach (ClsBullet bullet in bullets)
            {
                if (bullet.state != ClsBullet.BulletState.stored) bullet.Draw(device, view, projection);
            }
        }
    }
}


