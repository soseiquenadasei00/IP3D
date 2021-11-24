using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace IP3D {
    public class ClsTerreno
    {

        //normal stuff
        private BasicEffect effect;

        //vertex
        private VertexBuffer vertexBuffer;
        private VertexPositionNormalTexture[] vertex;

        //Index
        private IndexBuffer indexBuffer;
        private short[] index;
        private int indexCount;

        //heightMap
        public int height;
        public int width;
        private int vertexCount;
        private float vScale;
        public float[,] alturas;

        public ClsTerreno(GraphicsDevice device, Texture2D heightMap, Texture2D texture)
        {
            effect = new BasicEffect(device);
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            Vector3 d0 = new Vector3(1.0f, -1.0f, 1.0f);
            d0.Normalize();
            effect.DirectionalLight0.Direction = d0;
            effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.View = Matrix.CreateLookAt(new Vector3(64.0f, 20.0f, 64.0f), new Vector3(64.0f, 0.0f, 64.0f), Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), aspectRatio, 0.1f, 1000000f);


            effect.LightingEnabled = true;
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = texture;
            effect.World = Matrix.Identity;
            effect.DirectionalLight0.Enabled = true;

            //dimensoes do terreno
            height = heightMap.Height;
            width = heightMap.Width;
            vertexCount = heightMap.Height * heightMap.Width;
            indexCount = (width - 1) * height * 2;


            index = new short[indexCount];
            vertex = new VertexPositionNormalTexture[vertexCount];
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertexCount, BufferUsage.None);

            indexBuffer = new IndexBuffer(device, typeof(short), indexCount, BufferUsage.None);

            CreateGeometry(device, heightMap);
        }
        #region SetNormals
        public void SetTopEdgeNormals()
        {
            Vector3 normal;
            Vector3[] results = new Vector3[4];
            Vector3 soma = Vector3.Zero;
            for (int i = 1; i < width - 1; i++)
            {
                Vector3 position = vertex[i].Position;

                Vector3 w = vertex[i - 1].Position - position;
                Vector3 sw = vertex[i - 1 + width].Position - position;
                Vector3 s = vertex[i + 0 + width].Position - position;
                Vector3 se = vertex[i + 1 + width].Position - position;
                Vector3 e = vertex[i + 1].Position - position;

                results[0] = Vector3.Cross(w, sw);
                results[1] = Vector3.Cross(sw, s);
                results[2] = Vector3.Cross(s, se);
                results[3] = Vector3.Cross(se, e);

                for (int toMedian = 0; toMedian < 4; toMedian++)
                {
                    results[toMedian].Normalize();
                    soma += results[toMedian];
                    soma.Normalize();
                }
                normal = soma / 4;
                normal.Normalize();
                vertex[i].Normal = normal;
            }
        }
        public void SetBottomEdgeNormals()
        {
            Vector3 normal;
            Vector3[] results = new Vector3[4];
            Vector3 soma = Vector3.Zero;
            for (int i = width * (height - 1) + 1; i < (width * height - 1) - 1; i++)
            {
                Vector3 position = vertex[i].Position;

                Vector3 e = vertex[i + 1].Position - position;
                Vector3 ne = vertex[i + 1 - width].Position - position;
                Vector3 n = vertex[i + 0 - width].Position - position;
                Vector3 nw = vertex[i - 1 - width].Position - position;
                Vector3 w = vertex[i - 1].Position - position;

                results[0] = Vector3.Cross(e, ne);
                results[1] = Vector3.Cross(ne, n);
                results[2] = Vector3.Cross(n, nw);
                results[3] = Vector3.Cross(nw, w);

                for (int toMedian = 0; toMedian < 4; toMedian++)
                {
                    results[toMedian].Normalize();
                    soma += results[toMedian];
                    soma.Normalize();
                }
                normal = soma / 4;
                normal.Normalize();
                vertex[i].Normal = normal;
            }
        }

        public void SetLeftEdgeNormals()
        {
            Vector3 normal;
            Vector3[] results = new Vector3[4];
            Vector3 soma = Vector3.Zero;
            for (int j = width; j < width * (height - 2); j = j + width)
            {
                Vector3 position = vertex[j].Position;

                Vector3 s = vertex[j + width].Position - position;
                Vector3 se = vertex[j + 1 + width].Position - position;
                Vector3 e = vertex[j + 1].Position - position;
                Vector3 ne = vertex[j + 1 - width].Position - position;
                Vector3 n = vertex[j - width].Position - position;

                results[0] = Vector3.Cross(s, se);
                results[1] = Vector3.Cross(se, e);
                results[2] = Vector3.Cross(e, ne);
                results[3] = Vector3.Cross(ne, n);

                for (int toMedian = 0; toMedian < 4; toMedian++)
                {
                    results[toMedian].Normalize();
                    soma += results[toMedian];
                    soma.Normalize();
                }
                normal = soma / 4;
                normal.Normalize();
                vertex[j].Normal = normal;
            }
        }
        public void SetRightEdgeNormals()
        {
            Vector3 normal;
            Vector3[] results = new Vector3[4];
            Vector3 soma = Vector3.Zero;
            for (int j = (2 * width) - 1; j < width * (height - 1) - 1; j = j + width)
            {
                Vector3 position = vertex[j].Position;

                Vector3 n = vertex[j - width].Position - position;
                Vector3 nw = vertex[j - 1 - width].Position - position;
                Vector3 w = vertex[j - 1].Position - position;
                Vector3 sw = vertex[j - 1 + width].Position - position;
                Vector3 s = vertex[j + width].Position - position;

                results[0] = Vector3.Cross(n, nw);
                results[1] = Vector3.Cross(nw, w);
                results[2] = Vector3.Cross(w, sw);
                results[3] = Vector3.Cross(sw, s);

                for (int toMedian = 0; toMedian < 4; toMedian++)
                {
                    results[toMedian].Normalize();
                    soma += results[toMedian];
                    soma.Normalize();
                }
                normal = soma / 4;
                normal.Normalize();
                vertex[j].Normal = normal;
            }
        }

        public void SetCornerNormals()
        {
            Vector3 normal;
            Vector3[] results = new Vector3[2];
            Vector3 soma = Vector3.Zero;

            //top left corner
            Vector3 position = vertex[0].Position;

            Vector3 s = vertex[width].Position - position;
            Vector3 se = vertex[width + 1].Position - position;
            Vector3 e = vertex[1].Position - position;

            results[0] = Vector3.Cross(s, se);
            results[1] = Vector3.Cross(se, e);

            results[0].Normalize();
            results[1].Normalize();
            soma += results[0];
            soma.Normalize();
            soma += results[1];
            soma.Normalize();
            normal = soma / 2;
            normal.Normalize();
            vertex[0].Normal = normal;

            soma = Vector3.Zero;
            //top right corner
            position = vertex[width - 1].Position;

            Vector3 w = vertex[(width - 1) - 1].Position - position;
            Vector3 sw = vertex[(2 * width) - 2].Position - position;
            s = vertex[(2 * width) - 1].Position - position;

            results[0] = Vector3.Cross(w, sw);
            results[1] = Vector3.Cross(sw, s);

            results[0].Normalize();
            results[1].Normalize();
            soma += results[0];
            soma.Normalize();
            soma += results[1];
            soma.Normalize();
            normal = soma / 2;
            normal.Normalize();
            vertex[width - 1].Normal = normal;

            soma = Vector3.Zero;
            //bottom left corner
            position = vertex[width * (height - 1)].Position;

            e = vertex[width * (height - 1) + 1].Position - position;
            Vector3 ne = vertex[width * (height - 2) + 1].Position - position;
            Vector3 n = vertex[width * (height - 2) + 0].Position - position;


            results[0] = Vector3.Cross(e, ne);
            results[1] = Vector3.Cross(ne, n);

            results[0].Normalize();
            results[1].Normalize();
            soma += results[0];
            soma.Normalize();
            soma += results[1];
            soma.Normalize();
            normal = soma / 2;
            normal.Normalize();
            vertex[width * (height - 1)].Normal = normal;

            soma = Vector3.Zero;
            //bottom right corner
            position = vertex[(width * height) - 1].Position;

            n = vertex[width * (height - 1) - 1].Position - position;
            Vector3 nw = vertex[width * (height - 1) - 2].Position - position;
            w = vertex[(width * height) - 2].Position - position;

            results[0] = Vector3.Cross(n, nw);
            results[1] = Vector3.Cross(nw, w);

            results[0].Normalize();
            results[1].Normalize();
            soma += results[0];
            soma.Normalize();
            soma += results[1];
            soma.Normalize();
            normal = soma / 2;
            normal.Normalize();
            vertex[(width * height) - 1].Normal = normal;
        }

        public void SetCenterNormals()
        {
            Vector3 normal;
            Vector3[] results = new Vector3[8];
            Vector3 soma = Vector3.Zero;
            for (int i = 1; i < height - 1; i++)
            {
                for (int j = 1; j < width - 1; j++)
                {
                    Vector3 position = vertex[i * width + j].Position;

                    Vector3 n = vertex[(i - 1) * width + j + 0].Position - position;
                    Vector3 nw = vertex[(i - 1) * width + j - 1].Position - position;
                    Vector3 w = vertex[(i + 0) * width + j - 1].Position - position;
                    Vector3 sw = vertex[(i + 1) * width + j - 1].Position - position;
                    Vector3 s = vertex[(i + 1) * width + j + 0].Position - position;
                    Vector3 se = vertex[(i + 1) * width + j + 1].Position - position;
                    Vector3 e = vertex[(i + 0) * width + j + 1].Position - position;
                    Vector3 ne = vertex[(i - 1) * width + j + 1].Position - position;

                    results[0] = Vector3.Cross(n, nw);
                    results[1] = Vector3.Cross(nw, w);
                    results[2] = Vector3.Cross(w, sw);
                    results[3] = Vector3.Cross(sw, s);
                    results[4] = Vector3.Cross(s, se);
                    results[5] = Vector3.Cross(se, e);
                    results[6] = Vector3.Cross(e, ne);
                    results[7] = Vector3.Cross(ne, n);

                    for (int toMedian = 0; toMedian < 8; toMedian++)
                    {
                        results[toMedian].Normalize();
                        soma += results[toMedian];
                        soma.Normalize();
                    }
                    normal = soma / 8;
                    normal.Normalize();
                    vertex[i * width + j].Normal = normal;
                }
            }
        }

        public void SetNormals()
        {
            SetCornerNormals();
            SetTopEdgeNormals();
            SetBottomEdgeNormals();
            SetLeftEdgeNormals();
            SetRightEdgeNormals();
            SetCenterNormals();
        }

        #endregion
        public void CreateGeometry(GraphicsDevice device, Texture2D heightMap)
        {

            Color[] texelColors = new Color[vertexCount];
            heightMap.GetData<Color>(texelColors);
            vScale = 0.04f;
            alturas = new float[width, height];

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    float y = texelColors[width * z + x].R * vScale;
                    alturas[x, z] = y;
                    vertex[width * z + x] = new VertexPositionNormalTexture(new Vector3(x, y, z), Vector3.UnitY, new Vector2(x % 2, z % 2));
                }
            }
            for (int x = 0; x < width - 1; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    index[(height * 2) * x + z * 2 + 0] = (short)(x + 0 + width * z);
                    index[(height * 2) * x + z * 2 + 1] = (short)(x + 1 + width * z);
                }
            }
            SetNormals();
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertex);
            indexBuffer.SetData<short>(index);
        }
        public float GetHeight(float u, float z)
        {
            float yA, yB, yC, yD;
            Vector2[] pontos = new Vector2[4]; // four point, 'cause we used a square
            pontos[0] = new Vector2((int)u, (int)z);
            pontos[1] = new Vector2((int)u + 1, (int)z);
            pontos[2] = new Vector2((int)u, (int)z + 1);
            pontos[3] = new Vector2((int)u + 1, (int)z + 1);


            #region Interpolation in the X axis
            /*
             * Map point = int , We need subtract my cam point - int point ( When close to the point the more relative weight it as)
             * 0.1 > 0.9
             */
            //weight of A
            float wA = Math.Abs(pontos[0].X - u);
            //weight of B
            float wB = Math.Abs(pontos[1].X - u);
            //weight of C
            float wC = Math.Abs(pontos[2].X - u);
            //weight of D
            float wD = Math.Abs(pontos[3].X - u);
            #endregion

            yA = alturas[Math.Abs((int)pontos[0].X), Math.Abs((int)pontos[0].Y)];
            yB = alturas[Math.Abs((int)pontos[1].X), Math.Abs((int)pontos[1].Y)];
            yC = alturas[Math.Abs((int)pontos[2].X), Math.Abs((int)pontos[2].Y)];
            yD = alturas[Math.Abs((int)pontos[3].X), Math.Abs((int)pontos[3].Y)];

            #region Interpolation 
            //interpolation of A to B
            float yAB = (yA * wB) +
                (yB * wA);

            //Interpolation between C and D
            float yCD = (yD * wC) +
                (yC * wD);
            #endregion


            #region Interpolation in the Z axis
            //straight AB
            float wAB = Math.Abs(pontos[0].Y - z);
            //straight CD
            float wCD = Math.Abs(pontos[2].Y - z);
            #endregion

            //Camera Height
            float camSurf = (yAB * wCD) + (yCD * wAB);
            return camSurf;
        }
        public Vector3 GetNormals(float x, float z)
        {
            int xInt = (int)x;
            int zInt = (int)z;

            #region Interpolation in the X axis
            //weight of A
            float wA = x - xInt;
            //weight of B
            float wB = (xInt + 1) - x;

            //interpolation of A to B
            Vector3 yAB = (vertex[xInt + zInt * width].Normal * wB) +
                (vertex[(xInt + 1) + zInt * width].Normal * wA);

            //Interpolation between C and D
            Vector3 yCD = (vertex[xInt + (zInt + 1) * width].Normal * wB) +
                (vertex[(xInt + 1) + (zInt + 1) * width].Normal * wA);
            #endregion

            #region Interpolation in the Z axis
            //weight of AB
            float wAB = z - zInt;
            //weight of CD
            float wCD = zInt + 1 - z;
            #endregion

            /* quick guide for the bi-interpolation

                 wA      yAB   wB
             a------------|---------b
             |            | wAB     | wB 
             |------------x----------
             |            |         |
             |            | wCD     | wD
             c------------|---------d
                 wA       yCD   wB

             */

            //Camera Height
            Vector3 tankPitch = (yAB * wCD) + (yCD * wAB);
            return tankPitch;
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            effect.View = view;
            effect.Projection = projection;
            effect.CurrentTechnique.Passes[0].Apply();
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;

            for (int i = 0; i < width - 1; i++)
            {
                int indexOffset = height * 2;
                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, i * indexOffset, (height - 1) * 2);
            }
        }

    }
}