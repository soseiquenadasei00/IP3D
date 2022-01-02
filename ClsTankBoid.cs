using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D
{
    class ClsTankBoid  : ClsGameObject{
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


        public ClsTankBoid(GraphicsDevice device, Game1 game1, Model model, ClsTerreno terreno, Vector3 position, Matrix scale, float radius, string name) : base(position, scale, Layer.blockable, radius, name){
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

        public void Update(GameTime gameTime)
        {
            Matrix rotacao = Matrix.CreateFromYawPitchRoll(rotTank, 0f, 0f);
            Vector3 direcao = Vector3.Transform(-Vector3.UnitZ, rotacao);

            Vector3 normal = terreno.GetNormals(position.X, position.Z);
            normal.Normalize();
            Vector3 right = Vector3.Cross(direcao, normal);
            right.Normalize();
            Vector3 direcaoCorreta = Vector3.Cross(normal, right);
            direcaoCorreta.Normalize();



            rotacao.Up = normal;
            rotacao.Forward = direcaoCorreta;
            rotacao.Right = right;

            position.Y = terreno.GetHeight(position.X, position.Y);

            Matrix translation = Matrix.CreateTranslation(position);
            tankModel.Root.Transform = scale * rotacao * translation;
            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);
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


