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

        private ClsTank playerTank;

        public ClsCameraMira(ClsTank playerTank) 
        {
            this.playerTank = playerTank;
        }

        public Vector3 Update()
        {
            position = playerTank.posCanhaoMundo + playerTank.dirCanhaoMundo;
            Vector3 direction = playerTank.dirCanhaoMundo;
            Vector3 target = position + direction;
            view = Matrix.CreateLookAt(position, target, playerTank.normal);
            return position;
        }

    }
}
