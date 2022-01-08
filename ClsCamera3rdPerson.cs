using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace IP3D
{
    public class ClsCamera3rdPerson
    {

        private ClsTerreno terreno;
        private ClsTank playerTank;
        public Matrix view, projection;
        public float yaw, pitch;
        private int screenW, screenH;
        private float offset = 1f;

        public ClsCamera3rdPerson(GraphicsDevice device, ClsTerreno terreno, ClsTank playerTank)
        {
            this.playerTank = playerTank;
            screenW = device.Viewport.Width;
            screenH = device.Viewport.Height;
            this.terreno = terreno;
            float aspectRatio = (float)device.Viewport.Width /
            device.Viewport.Height;

            projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(60f),
            aspectRatio,
            0.1f,
            100.0f);
        }
        public Vector3 Update(Vector3 tankpos, Vector3 tankdir){
            Vector3 position = tankpos + tankdir * 6 + new Vector3(0, 4, 0);
            Vector3 direction = - tankdir;
            Vector3 right;
            right = Vector3.Cross(tankdir, Vector3.UnitY);
            Vector3 up;
            up = Vector3.Cross(right, direction);

            Vector3 target = position + direction;
            view = Matrix.CreateLookAt(position, target, -up);
            this.yaw = yaw;
            this.pitch = pitch;
            return position;
        }
    }
}
