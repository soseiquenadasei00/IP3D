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
        private SystemParticula systemparticula;
        private ClsTerreno terreno;
        private CameraLivre camera;


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
        private Keys[] control2 = { Keys.U,     Keys.O,      Keys.Y,    Keys.H,      Keys.J,    Keys.L,     Keys.I,       Keys.K,        Keys.RightControl };
        private ClsTank tank1;
        private ClsTank tank2;
        private ClsSkybox skybox;
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

            terreno = new ClsTerreno(_graphics.GraphicsDevice, Content.Load<Texture2D>("lh3d1"), Content.Load<Texture2D>("grass"));
            camera = new CameraLivre(_graphics.GraphicsDevice, terreno);
            tank1 = new ClsTank(this, Content.Load<Model>(@"tank\tank"), terreno, new Vector3(42, 0, 42), Matrix.CreateScale(0.008f), 2.68f, "tank1");
            //tank2 = new ClsTank(this, Content.Load<Model>(@"tank\tank"), terreno, new Vector3(69, 0, 69), Matrix.CreateScale(0.008f), 2.68f, "tank2");
            systemparticula = new SystemParticula(_graphics.GraphicsDevice,terreno);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            systemparticula.Update(gameTime);
            camera.Update();
            tank1.Update(gameTime, control1);
           // tank2.Update(gameTime, control2);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            terreno.Draw(_graphics.GraphicsDevice,camera.view,camera.projection);
            tank1.Draw(_graphics.GraphicsDevice, camera.view, camera.projection);
            systemparticula.Draw(_graphics.GraphicsDevice, camera.projection, camera.view);
            //tank2.Draw(_graphics.GraphicsDevice, camera.view, camera.projection);
            skybox.Draw(camera.view, camera.projection);

            base.Draw(gameTime);
        }
    }
}
