using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D
{
    class ClsTank {
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
        public Vector3 position;

        private ClsTerreno terreno;

        //structure
        private Model tankModel;
        private ModelBone turretBone, cannonBone, leftBackWheelBone, rightBackWheelBone, leftFrontWheelBone,
                          rightFrontWheelBone, leftSteerBone, rightSteerBone, hatchBone;
        private Matrix leftSteerTranform, rightSteerTranform, cannonTransform, turretTransform;
        private Matrix[] boneTransforms;
        private Matrix Scale;
        private List<ModelBone> wheelNames;

        private float rotTower = 0;
        private float rotCanon = 0;
        private float rotTank = 0;

        


        public ClsTank(GraphicsDevice device, Model model, ClsTerreno terreno, Vector3 position) {
            Scale = Matrix.CreateScale(0.01f);
            tankModel     = model;
            this.terreno  = terreno;
            this.position = position;


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
            wheelNames = new List<ModelBone>();
            wheelNames.Add(rightBackWheelBone);
            wheelNames.Add(leftBackWheelBone); 
            wheelNames.Add(rightFrontWheelBone);
            wheelNames.Add(leftFrontWheelBone);
        }

        float wheelRotationAngle = 0;
        float wheelRotationSpeed = 0;

        public void Update(GameTime gameTime, Keys[] controls)
        {

            KeyboardState state = Keyboard.GetState();

            Matrix rotacao = Matrix.CreateFromYawPitchRoll(rotTank, 0f, 0f);
            Vector3 direcao = Vector3.Transform(-Vector3.UnitZ, rotacao);

            Vector3 normal = terreno.GetNormals(position.X, position.Z);
            normal.Normalize();
            Vector3 right = Vector3.Cross(direcao, normal);
            right.Normalize();
            Vector3 direcaoCorreta = Vector3.Cross(normal, right);
            direcaoCorreta.Normalize();

            if (state.IsKeyDown(controls[0])) rotTower -= 45 * (float)gameTime.ElapsedGameTime.TotalSeconds; //tower left
            if (state.IsKeyDown(controls[1])) rotTower += 45 * (float)gameTime.ElapsedGameTime.TotalSeconds; //tower right

            if (state.IsKeyDown(controls[2])) rotCanon -= 45 * (float)gameTime.ElapsedGameTime.TotalSeconds;//cannon up
            if (state.IsKeyDown(controls[3])) rotCanon += 45 * (float)gameTime.ElapsedGameTime.TotalSeconds;//cannon down
            if (rotCanon > 25) rotCanon = 25;
            if (rotCanon < -40) rotCanon = -40;


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


            //forward
            if (state.IsKeyDown(controls[6]))
            {
                position += -direcao * 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                wheelRotationSpeed += 10.0f;
            }
            if (wheelRotationSpeed > 10.0f) wheelRotationSpeed = 10.0f;


            //backward
            if (state.IsKeyDown(controls[7]))
            {
                position -= -direcao * 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                wheelRotationSpeed -= 10.0f;
            }
            if (wheelRotationSpeed < -10.0f) wheelRotationSpeed = -10.0f;
            if (!(state.IsKeyDown(controls[7]) || state.IsKeyDown(controls[6]))) wheelRotationSpeed = 0.0f;



            Matrix rotationCanon = Matrix.CreateRotationX(MathHelper.ToRadians(rotCanon)) * cannonTransform;
            Matrix rotationTower = Matrix.CreateRotationY(MathHelper.ToRadians(rotTower)) * turretTransform;
            Matrix wheelSpeed = Matrix.CreateRotationX(MathHelper.ToRadians(wheelRotationSpeed));
            Matrix wheelRotation = Matrix.CreateRotationY(wheelRotationAngle);
            Console.WriteLine(wheelRotationSpeed) ;
            turretBone.Transform = rotationTower;
            cannonBone.Transform = rotationCanon;
            leftSteerBone.Transform = wheelRotation  * leftSteerTranform;
            rightSteerBone.Transform = wheelRotation  * rightSteerTranform;

            rightBackWheelBone.Transform = wheelSpeed * rightBackWheelBone.Transform;
            rightFrontWheelBone.Transform = wheelSpeed * rightFrontWheelBone.Transform;
            leftBackWheelBone.Transform = wheelSpeed * leftBackWheelBone.Transform;
            leftFrontWheelBone.Transform = wheelSpeed * leftFrontWheelBone.Transform;

            rotacao.Up = normal;
            rotacao.Forward = direcaoCorreta;
            rotacao.Right = right;

            position.Y = terreno.GetHeight(position.X, position.Z);
            Matrix translation = Matrix.CreateTranslation(position);
            tankModel.Root.Transform = Scale * rotacao * translation;
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
        }
    }
}


