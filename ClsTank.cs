using System;
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

        private BasicEffect effect;
        private ClsTerreno terreno;

        //structure
        private Model tankModel;
        private ModelBone turretBone, cannonBone, leftBackWheelBone, rightBackWheelBone, leftFrontWheelBone,
                          rightFrontWheelBone, leftSteerBone, rightSteerBone, hatchBone;
        private Matrix leftSteerTranform, rightSteerTranform, cannonTransform, turretTransform;
        private Matrix[] boneTransforms;
        private Matrix Scale;
        
        public ClsTank(GraphicsDevice device, Model model,ClsTerreno terreno, Vector3 position)
        {
            effect = new BasicEffect(device);
            Scale = Matrix.CreateScale(0.01f);
            tankModel = model;
            this.terreno = terreno;
            leftBackWheelBone = tankModel.Bones["l_back_wheel_geo"];
            rightBackWheelBone = tankModel.Bones["r_back_wheel_geo"];

            leftFrontWheelBone= tankModel.Bones["l_front_wheel_geo"];
            rightFrontWheelBone= tankModel.Bones["r_front_wheel_geo"];

            leftSteerBone = tankModel.Bones["l_steer_geo"];
            rightSteerBone = tankModel.Bones["r_steer_geo"];

            turretBone = tankModel.Bones["turret_geo"];
            cannonBone = tankModel.Bones["canon_geo"];
            hatchBone = tankModel.Bones["hatch_geo"];

            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            leftSteerTranform = leftSteerBone.Transform;
            rightSteerTranform = rightSteerBone.Transform;

            boneTransforms = new Matrix[tankModel.Bones.Count];
            this.position = position;
           
        }
        float rotTower = 0;
        float rotCanon = 0;
        float rotTank = 0;
        
        Vector3 direction = new Vector3();
        public void Update(GameTime gameTime, Keys[] controls)
        {
            
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(controls[0]))  //tower left
            {
                rotTower -= 45 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (state.IsKeyDown(controls[1])) //tower right
            {
                rotTower += 45 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (state.IsKeyDown(controls[2]))  //cannon up
            {
                rotCanon -= 45 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (state.IsKeyDown(controls[3])) //cannon down
            {
                rotCanon += 45 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (state.IsKeyDown(controls[4])) //tank left
            {
                rotTank += 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (state.IsKeyDown(controls[5]))  //tank right
            {
                rotTank -= 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            

            Matrix rotationCanon = Matrix.CreateRotationX(MathHelper.ToRadians(rotCanon)) * cannonTransform;
            Matrix rotationTower = Matrix.CreateRotationY(MathHelper.ToRadians(rotTower)) * turretTransform;

            Matrix rotationSteer = Matrix.CreateRotationY(MathHelper.ToRadians(rotTank));

            turretBone.Transform = rotationTower;
            cannonBone.Transform = rotationCanon;
            leftSteerBone.Transform = rotationSteer * leftSteerTranform;
            rightSteerBone.Transform = rotationSteer * rightSteerTranform;
            
            

           
            Vector3 baset = Vector3.UnitZ;
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

            if (state.IsKeyDown(controls[6]))  //forward
            {
                position += -direcao * 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (state.IsKeyDown(controls[7]))  //backward
            {
                position -= -direcao * 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

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


