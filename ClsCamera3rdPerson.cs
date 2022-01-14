using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace IP3D
{
    public class ClsCamera3rdPerson
    { 
        public Matrix view;
        private ClsTank playerTank;
        private ClsTerreno terreno;
        private float offsetDist = 6f;
        private Vector3 offSetHeight = new Vector3(0, 4, 0);
        public ClsCamera3rdPerson(ClsTank playerTank, ClsTerreno terreno)
        {
            this.playerTank = playerTank;
            this.terreno = terreno;
        }
        public Vector3 Update()
        {
            Vector3 position = playerTank.position + playerTank.direcaoCorreta * offsetDist + offSetHeight;
            Vector3 direction = -playerTank.direcaoCorreta;
            Vector3 target = position + direction ;
            view = Matrix.CreateLookAt(position, target, Vector3.Up);
            return position;
        }
    }
}
