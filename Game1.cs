using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace IP3D
{
    
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private ClsCollisionManager collisionManager;

        private SpriteBatch _spriteBatch;
        private ClsSystemParticula systemparticula;
        private ClsTerreno terreno;
        private CameraManager cameraManager;
        private SystemParticulaDust dust;
        private SystemParticulaDustBoid dustP2;
        private Texture2D sight;


        /* CONTROL ARRAY PASSED AS PARAMETER TO TANK UPDATE METHOD:
        0 = TOWER LEFT
        1 = TOWER RIGHT
        2 = CANNON UP
        3 = CANNON DOWN
        4 = TANK LEFT
        5 = TANK RIGHT
        6 = TANK FORWARD
        7 = TANK BACKWARD
        */                        /*TOWER LEFT, TOWER RIGHT, CANNON UP, CANNON DOWN, TANK LEFT, TANK RIGHT, TANK FORWARD, TANK BACKWARD, TANK FIRE*/
        private Keys[] control1 = { Keys.Left,  Keys.Right,  Keys.Up,   Keys.Down,   Keys.A,    Keys.D,     Keys.W,       Keys.S,        Keys.Space };
        private Keys[] control2 = { Keys.G,     Keys.U,      Keys.Y,    Keys.H,      Keys.J,    Keys.L,     Keys.I,       Keys.K,        Keys.RightControl };
        private ClsTank tank1;
        private ClsTankBoid tankboid;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            collisionManager = new ClsCollisionManager();
            sight = Content.Load<Texture2D>("aim");
            terreno = new ClsTerreno(_graphics.GraphicsDevice, Content.Load<Texture2D>("lh3d1"), Content.Load<Texture2D>("grass"));

            
       
            tank1 = new ClsTank(_graphics.GraphicsDevice, this, Content.Load<Model>(@"tank\tank"), terreno, 
                new Vector3(42, 0, 42), Matrix.CreateScale(0.008f), 2.68f, "tank1");

            tankboid = new ClsTankBoid(_graphics.GraphicsDevice, this, Content.Load<Model>(@"tank2\tank"), terreno, 
                new Vector3(29, 0, 39), Matrix.CreateScale(0.008f), 2.68f, "tankboid",tank1);

            systemparticula = new ClsSystemParticula(_graphics.GraphicsDevice,terreno);
            dust = new SystemParticulaDust(_graphics.GraphicsDevice, tank1, terreno);
            dustP2 = new SystemParticulaDustBoid(_graphics.GraphicsDevice, tankboid, terreno);
            cameraManager = new CameraManager(_graphics.GraphicsDevice, terreno, tank1);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit(); 
            tank1.Update(gameTime, control1);
            cameraManager.Update();
            if (cameraManager.actual == CameraManager.CameraActual.mira) tank1.isOnCameraAim = true;
            else tank1.isOnCameraAim = false;
            systemparticula.Update(gameTime);
            dust.Update(gameTime);
            tankboid.Update(gameTime,control2);
            tank1.Update(gameTime, control1);
            dust.Update(gameTime);
            dustP2.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            tank1.Draw(_graphics.GraphicsDevice, cameraManager.view, cameraManager.projection);
            terreno.Draw(_graphics.GraphicsDevice, cameraManager.view, cameraManager.projection);
            systemparticula.Draw(_graphics.GraphicsDevice, cameraManager.projection, cameraManager.view);
            tankboid.Draw(_graphics.GraphicsDevice, cameraManager.view, cameraManager.projection);
            dust.Draw(_graphics.GraphicsDevice, cameraManager.view, cameraManager.projection);
            dustP2.Draw(_graphics.GraphicsDevice, cameraManager.view, cameraManager.projection);


            base.Draw(gameTime);
        }
    }
}
