using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace IP3D
{
    public class ClsCameraMira 
    {

        public Matrix view;
        public Vector3 position;

        private float offset = 4;
        private ClsTank playerTank;

        public ClsCameraMira(ClsTank playerTank) 
        {
            this.playerTank = playerTank;
        }

        public Vector3 Update()
        {
            position = playerTank.posCanhaoMundo + playerTank.dirCanhaoMundo * offset;
            Vector3 direction = playerTank.dirCanhaoMundo;
            Vector3 right = Vector3.Cross(playerTank.dirCanhaoMundo, Vector3.UnitY);      
            Vector3 up = Vector3.Cross(right, direction);
            Vector3 target = position + direction;
            view = Matrix.CreateLookAt(position, target, up);
            return position;
        }

    }
}
