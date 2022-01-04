using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace IP3D
{
    public class CameraManager
    {
        public enum CameraActual{sf, livre, mira}
        public CameraActual actual;
        public Vector3 position;
        public Matrix view;
        public Matrix projection;
        public CameraLivre cameraLivre;
        public ClsCameraSurfaceFollow cameraSurfaceFollow;
        float yaw = 0, pitch = 0;

        public CameraManager(GraphicsDevice device, ClsTerreno terreno)
        {
            position = new Vector3(64, 10, 64);
            cameraLivre = new CameraLivre(device, terreno);
            cameraSurfaceFollow = new ClsCameraSurfaceFollow(device, terreno);
            actual = CameraActual.livre;
            view = cameraLivre.view;
            projection = cameraLivre.projection;

        }

        public void Update()
        {
            MouseState ms = Mouse.GetState();
            KeyboardState kb = Keyboard.GetState();
            
            
            if (Keyboard.GetState().IsKeyDown(Keys.F1)) actual = CameraActual.livre;
            if (Keyboard.GetState().IsKeyDown(Keys.F2)) actual = CameraActual.sf;
            switch (actual)
            {
                case CameraActual.livre:
                    position = cameraLivre.Update(kb, ms, position, yaw, pitch);
                    yaw = cameraLivre.yaw;
                    pitch = cameraLivre.pitch;
                    projection = cameraLivre.projection;
                    view = cameraLivre.view;
                    break;
                case CameraActual.sf:
                    position = cameraSurfaceFollow.Update(kb, ms, position, yaw, pitch);
                    yaw = cameraSurfaceFollow.yaw;
                    pitch = cameraSurfaceFollow.pitch;
                    projection = cameraSurfaceFollow.projection;
                    view = cameraSurfaceFollow.view;
                    break;
                default:
                    position = cameraLivre.Update(kb, ms, position, yaw, pitch);
                    yaw = cameraLivre.yaw;
                    pitch = cameraLivre.pitch;
                    projection = cameraLivre.projection;
                    view = cameraLivre.view;
                    break;
            }
            
        }
    }
}
