using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D
{
    public class CameraLivre
    {
        public Matrix view;
        public float yaw, pitch;
        private int screenW, screenH;
        
        public CameraLivre(GraphicsDevice device) 
        {
            screenW = device.Viewport.Width;
            screenH = device.Viewport.Height;
        }

        public Vector3 Update(KeyboardState kb, MouseState ms, Vector3 position, float yaw, float pitch) 
        {
            Mouse.SetPosition(screenW / 2, screenH / 2);
            Vector2 mouseOffSet = ms.Position.ToVector2() - new Vector2(screenW / 2, screenH / 2);
            float radianosPorPixel = MathHelper.ToRadians(0.5f);
            yaw = yaw - mouseOffSet.X * radianosPorPixel;
            pitch = pitch + mouseOffSet.Y * radianosPorPixel;
            if (pitch > MathHelper.ToRadians(85f)) pitch = MathHelper.ToRadians(85f);
            if (pitch < MathHelper.ToRadians(-85f)) pitch = -MathHelper.ToRadians(85f);
            Matrix rotacao;
            rotacao = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0f); //yaw move for sides(Angle), pitch for up and down (angle), camera rotation
            Vector3 direction; 
            direction = Vector3.Transform(-Vector3.UnitZ, rotacao); //  move for up and  down 
            Vector3 right; 
            right = Vector3.Cross(direction, Vector3.UnitY);
            Vector3 up;
            up = Vector3.Cross(right, direction);
            float speed = 0.5f;
            if (kb.IsKeyDown(Keys.J)) position = position - right * speed;
            if (kb.IsKeyDown(Keys.L)) position = position + right * speed;
            if (kb.IsKeyDown(Keys.I)) position = position + direction * speed;
            if (kb.IsKeyDown(Keys.K)) position = position - direction * speed;
            if (kb.IsKeyDown(Keys.NumPad7)) position = position + up * speed;
            if (kb.IsKeyDown(Keys.NumPad1)) position = position - up * speed;
            if (kb.IsKeyDown(Keys.G)) position = new Vector3(64f, 20f, 64f); // reset position 

            Vector3 target = position + direction;
            view = Matrix.CreateLookAt(position, target, up);
            this.yaw = yaw;
            this.pitch = pitch;
            return position;
        }
   }
}
