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
        public enum CameraActual{sf, livre, mira, thirdperson}
        public CameraActual actual;
        public Vector3 position;
        public Matrix view;
        public Matrix projection;
        public CameraLivre cameraLivre;
        public ClsCameraSurfaceFollow cameraSurfaceFollow;
        public ClsCameraMira cameraMira;
        public ClsCamera3rdPerson thirdperson;
        public ClsTank playerTank;
        float yaw = 0, pitch = 0;

        public CameraManager(GraphicsDevice device, ClsTerreno terreno, ClsTank playerTank)
        {
            position = new Vector3(64, 10, 64);
            this.playerTank = playerTank;
            cameraLivre = new CameraLivre(device, terreno);
            cameraSurfaceFollow = new ClsCameraSurfaceFollow(device, terreno);
            cameraMira = new ClsCameraMira(device, terreno, playerTank);
            thirdperson = new ClsCamera3rdPerson(device, terreno, playerTank);
            actual = CameraActual.livre;
            view = cameraLivre.view;
            projection = cameraLivre.projection;

        }

        public void Update(Vector3 bulletDir, Vector3 posCanhaoMundo)
        {
            MouseState ms = Mouse.GetState();
            KeyboardState kb = Keyboard.GetState();
            
            
            if (Keyboard.GetState().IsKeyDown(Keys.F1)) actual = CameraActual.livre;
            if (Keyboard.GetState().IsKeyDown(Keys.F2)) actual = CameraActual.sf;
            if (Keyboard.GetState().IsKeyDown(Keys.F3)) actual = CameraActual.mira;
            if (Keyboard.GetState().IsKeyDown(Keys.F4)) actual = CameraActual.thirdperson;
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
                case CameraActual.mira:
                    position = cameraMira.Update(bulletDir, posCanhaoMundo);
                    yaw = cameraMira.yaw;
                    pitch = cameraMira.pitch;
                    projection = cameraMira.projection;
                    view = cameraMira.view;
                    break;
                case CameraActual.thirdperson:
                    position = thirdperson.Update(playerTank.position, playerTank.direcao);
                    yaw = thirdperson.yaw;
                    pitch = thirdperson.pitch;
                    projection = thirdperson.projection;
                    view = thirdperson.view;
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
