using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D
{
    public class ClsTerreno
    {

        //normal stuff
        private BasicEffect effect;
        private Matrix worldMatrix;

        private VertexBuffer vertexBuffer;
        private VertexPositionColorTexture[] vertex;
        private int vertexCount;
        private int height;
        private int width;
        private IndexBuffer indexBuffer;
        private short[] index;

        public ClsTerreno(GraphicsDevice device, Texture2D texture)
        {
            effect = new BasicEffect(device);
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;

            effect.View = Matrix.CreateLookAt(new Vector3(-1f, 3f, 5f), Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), aspectRatio, 0.1f, 10000f);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;

            height = texture.Height;
            width = texture.Width;

            vertexCount = texture.Height * texture.Width;
            index = new short[vertexCount * 2];
            vertex = new VertexPositionColorTexture[vertexCount];
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionColorTexture), vertexCount, BufferUsage.None);
            indexBuffer = new IndexBuffer(device, typeof(VertexPositionColorTexture), vertexCount * 2, BufferUsage.None);



            CreateGeometry();
            worldMatrix = Matrix.Identity;
        }

        public void CreateGeometry()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; i++)
                {
                    vertex[width * i + j] = new VertexPositionColorTexture(new Vector3(j, 0, i), Color.White, new Vector2((j + i) % 2, i % 2));
                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; i++)
                {
                    index[2 * j + 2 * i + 0] = (short)(i * width + j);
                    index[2 * j + 2 * i + 1] = (short)(i * width + j + 1);
                }
            }
        }

        public void Draw(GraphicsDevice device)
        {
            effect.World = worldMatrix;

            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;

            for (int i = 0; i < width; i++)
            {
                //for(int j = 0; j < width; i++){
                //for(int zeroUm = 0; zeroUm <= 1; zeroUm++) {
                device.DrawPrimitives(PrimitiveType.TriangleStrip, i, height * 2);
            }

        }
    }
}