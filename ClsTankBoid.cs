using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D
{
    public class ClsTankBoid {

        public enum tankState { stop, move }
        public tankState actual;
        public Vector3 position;
        public ClsCircleCollider collider;
        public int lifes = 3;
        public Vector3 direcao;
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
        private ClsTank tankP1;
        private Game1 game;
        //structure
        public Model tankModel;
        private Matrix scale;
        private ModelBone turretBone, cannonBone, leftBackWheelBone, rightBackWheelBone, leftFrontWheelBone,
                          rightFrontWheelBone, leftSteerBone, rightSteerBone, hatchBone;
        private Matrix leftSteerTranform, rightSteerTranform, cannonTransform, turretTransform;
        public Matrix[] boneTransforms;
        
        private float rotCanon = 0;
        private float rotTower = 0;
        private float rotTank = 0;
        private float yaw;
        private float speed = 2f;
        private bool automaticBoid;
        private float wheelRotationAngle = 0;
        private float wheelRotationSpeed = 0;

        
        Matrix rotacao = Matrix.Identity;
        

        private List<ClsBullet> bullets = new List<ClsBullet>();
        private GraphicsDevice device;
        private int cannonPower = 100000;
        private float reloadTime = 2.0f;
        private float lastShotTime;
        private bool canFire;
        private Vector3 dirCanhaoMundo;
        private Vector3 posCanhaoLocal;
        private Vector3 posCanhaoMundo;
        private Random random = new Random();

        public ClsTankBoid(GraphicsDevice device, Game1 game1, Model model, ClsTerreno terreno, Vector3 position, Matrix scale, float radius, string name,ClsTank tankP1) { 
            this.scale     = scale;
            this.tankModel = model;
            this.terreno   = terreno;
            this.position  = position;
            this.game      = game1;
            this.device    = device;
            this.tankP1    = tankP1;
            direcao = new Vector3(0.0f, 0.0f, 1.0f);
            actual = tankState.stop;
            collider = new ClsCircleCollider(position, radius);
            ClsCollisionManager.instance.tankboid = this;

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

        public void Update(GameTime gameTime, Keys[] controls)
        {
            Vector3 futurePosition = position;
            KeyboardState state = Keyboard.GetState();
            actual = tankState.stop;
            if (Keyboard.GetState().IsKeyDown(Keys.O)) automaticBoid = true;
            if (Keyboard.GetState().IsKeyDown(Keys.P)) automaticBoid = false;

            if (automaticBoid == true)
            {
                if (Vector3.Distance(tankP1.position, position) > 20)
                {
                    futurePosition = seek(tankP1, gameTime);
                    actual = tankState.move;
                }
                else if (Vector3.Distance(tankP1.position, position) < 19)
                {
                    futurePosition = flee(tankP1, gameTime);
                    actual = tankState.move;
                }
                else actual = tankState.stop;
            }
            
            if (automaticBoid == false)
            {
                rotacao = Matrix.CreateFromYawPitchRoll(rotTank, 0f, 0f);
                direcao = Vector3.Transform(-Vector3.UnitZ, rotacao);
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

                futurePosition = position;
                //forward
                if (state.IsKeyDown(controls[6]))
                {
                    futurePosition += -direcao * 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    wheelRotationSpeed += 10.0f;
                    actual = tankState.move;
                }
                if (wheelRotationSpeed > 10.0f) wheelRotationSpeed = 10.0f;


                //backward
                if (state.IsKeyDown(controls[7]))
                {
                    futurePosition -= -direcao * 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    wheelRotationSpeed -= 10.0f;
                    actual = tankState.move;
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

                //fire

                dirCanhaoMundo = boneTransforms[10].Backward;
                posCanhaoLocal = Vector3.Zero;
                posCanhaoMundo = Vector3.Transform(posCanhaoLocal, boneTransforms[10]);

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
                        if (bullets[i].position.X <= 0
                            || bullets[i].position.Z <= 0
                            || bullets[i].position.X >= terreno.width - 1
                            || bullets[i].position.Z >= terreno.height - 1
                            || terreno.GetHeight(bullets[i].position.X, bullets[i].position.Z) >= bullets[i].position.Y
                            || bullets[i].collided) bullets.RemoveAt(i);

                    }
                }
                if ((float)gameTime.TotalGameTime.TotalSeconds - lastShotTime > reloadTime) canFire = true;

            }
            
            Vector3 normal = terreno.GetNormals(position.X, position.Z);
            normal.Normalize();
            Vector3 right = Vector3.Cross(direcao,normal);
            right.Normalize();
            Vector3 direcaoCorreta = Vector3.Cross(normal, right);
            direcaoCorreta.Normalize();
            
            rotacao.Up = normal;
            rotacao.Forward = direcaoCorreta;
            rotacao.Right = right;

            //collision
            if (!ClsCollisionManager.instance.CheckTankCollision() || ClsCollisionManager.instance.BoidMovingAway(futurePosition))
            {
                if (futurePosition.X < 127 && futurePosition.X > 0 && futurePosition.Z < 127 && futurePosition.Z > 0) position = futurePosition;
            }



            position.Y = terreno.GetHeight(position.X, position.Z);

            Matrix translation = Matrix.CreateTranslation(position);
            collider.center = position;
            tankModel.Root.Transform = scale * rotacao * translation;
            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

        }

        public void Fire()
        {
            ClsBullet newBullet = new ClsBullet(device, posCanhaoMundo, cannonPower);
            bullets.Add(newBullet);
            dirCanhaoMundo.Normalize();
            newBullet.Fire(dirCanhaoMundo);

        }

        public Vector3 seek(ClsTank tankP1,GameTime gameTime)
        {
            Vector3 vInical, a, vNext, pNext;
            vInical = Vector3.Normalize(tankP1.position - position)*speed;
            a = Vector3.Normalize(vInical - direcao )* 500f;        
            vNext = direcao + a * (float)gameTime.ElapsedGameTime.TotalSeconds;
            pNext = position + vNext * (float)gameTime.ElapsedGameTime.TotalSeconds;
            vNext.Normalize();
            direcao = -vNext;
            
            return pNext;
            
        }

        public Vector3 flee(ClsTank tankP1, GameTime gameTime)
        {
            Vector3 vInical, a, vNext, pNext;
            vInical = Vector3.Normalize(tankP1.position - position) * speed;
            a = Vector3.Normalize(vInical - direcao) * 500f;
            vNext = direcao + a * (float)gameTime.ElapsedGameTime.TotalSeconds;
            pNext = position - vNext * (float)gameTime.ElapsedGameTime.TotalSeconds;
            vNext.Normalize();
            direcao = vNext;

            return pNext;

        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            if (lifes > 0)
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
}


