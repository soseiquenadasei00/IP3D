using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace IP3D
{

    //Para a criação desta classe tivemos ajuda no seguinte link http://thver.blogspot.com/2012/07/how-to-create-sphere-programmatically.html
    public class ClsSphere
    {
        private BasicEffect effect;

        //vertex
        private VertexPositionColor[] vertices;
        private VertexBuffer vertexbuffer;

        //index
        private short[] index;
        private IndexBuffer indexBuffer;

        //logic
        private float radius;
        private int depth = 60;
        private int nVert, nIndex;
        private Vector3 position;

        public ClsSphere(GraphicsDevice device, Vector3 position, Color color, float radius)
        {
            this.position = position;
            this.radius = radius;
            effect = new BasicEffect(device);
            nVert = depth * depth;
            nIndex = nVert * 6;
            vertexbuffer = new VertexBuffer(device, typeof(VertexPositionColor), nVert, BufferUsage.None);
            indexBuffer = new IndexBuffer(device, typeof(short), nIndex, BufferUsage.None);
            vertices = new VertexPositionColor[nVert];
            index = new short[nIndex];
            CreateGeometry(color);
            vertexbuffer.SetData<VertexPositionColor>(vertices);
            indexBuffer.SetData<short>(index);
            effect.VertexColorEnabled = true;
        }

        public void Update(Vector3 position)
        {
            this.position = position;
        }
        private void CreateGeometry(Color color)
        {
            #region vertex
            Vector3 rad = new Vector3(radius, 0, 0);
            for (int x = 0; x < depth; x++)
            {
                float difx = 360.0f / depth;
                for (int y = 0; y < depth; y++)
                {
                    float dify = 360.0f / depth;
                    Matrix zrot = Matrix.CreateRotationZ(MathHelper.ToRadians(y * dify)); //make vertices around z 
                    Matrix yrot = Matrix.CreateRotationY(MathHelper.ToRadians(x * difx)); //make vertices around y
                    Vector3 point = Vector3.Transform(Vector3.Transform(rad, zrot), yrot);//transformation

                    vertices[x + y * depth] = new VertexPositionColor(point, color);
                }
            }
            #endregion
            #region index
            int i = 0;
            for (int x = 0; x < depth; x++)
            {
                for (int y = 0; y < depth; y++)
                {
                    int s1, s2;
                    if (x == depth - 1) s1 = 0;
                    else s1 = x + 1;
                    if (y == depth - 1) s2 = 0;
                    else s2 = y + 1;
                    short upperLeft = (short)(x * depth + y);
                    short upperRight = (short)(s1 * depth + y);
                    short lowerLeft = (short)(x * depth + s2);
                    short lowerRight = (short)(s1 * depth + s2);
                    index[i++] = upperLeft;
                    index[i++] = upperRight;
                    index[i++] = lowerLeft;
                    index[i++] = lowerLeft;
                    index[i++] = upperRight;
                    index[i++] = lowerRight;
                }
            }
            #endregion
        }
        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            effect.World = Matrix.CreateTranslation(position);
            effect.View = view;
            effect.Projection = projection;
            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vertexbuffer);
            device.Indices = indexBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, index.Length / 3);
        }
    }
}
