using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace IP3D
{
    public class ClsSkyBox
    {

        public VertexPositionNormalTexture[] verticesbase;
        public VertexPositionNormalTexture[] verticestop;
        public VertexPositionNormalTexture[] verticeslado;
        public BasicEffect effect;
        public Matrix worldMatrix;
        float baseRadius = 1f;
        int nlados = 4;
        float altura = 10;
        int nVert;
        Texture2D[] textures;

        public ClsSkyBox(Texture2D[] textures, GraphicsDevice device)
        {
            this.textures = textures;
            effect = new BasicEffect(device);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = true;
            
            //uma vez que faremos 4 arrays para desenhar o solido
            //o numero de triangulos em cada array vai ser igual o numero de lados do solido
            //assim: o numero de vertices é nlados vs 3
            nVert = 3 * nlados;
            verticesbase      = new VertexPositionNormalTexture[nVert];
            verticestop       = new VertexPositionNormalTexture[nVert];
            verticeslado      = new VertexPositionNormalTexture[10];  //2 * nlados + 2

            CreateGeometry();
            worldMatrix = Matrix.Identity;

        }

        public void Update(GameTime gametime)
        {
            
        }


        public void CreateGeometry()
        {
            //for indexing purposes
            for (int i = 0; i <= nlados; i++)
            {
                //math
                float angulo = MathHelper.ToRadians(i * (360.0f / nlados));
                //we need to get the angle of the next vertice to close the triangle
                float angulo2 = MathHelper.ToRadians((i + 1) * (360.0f / nlados));
                float x = baseRadius * System.MathF.Cos(angulo);
                float z = -baseRadius * System.MathF.Sin(angulo);
                float x2 = baseRadius * System.MathF.Cos(angulo2);
                float z2 = -baseRadius * System.MathF.Sin(angulo2);

                Vector3 vertex1 = new Vector3(x, 0, z);
                Vector3 v1Normal = -vertex1;
                v1Normal.Normalize();
                Vector3 vertex2 = new Vector3(x, altura, z);
                Vector3 v2Normal = -vertex2;
                v2Normal.Normalize();
                //base
                verticesbase[3 * i + 0] = new VertexPositionNormalTexture(new Vector3(x, 0, z), );
                verticesbase[3 * i + 1] = new VertexPositionNormalTexture(new Vector3(0, 0, 0), );
                verticesbase[3 * i + 2] = new VertexPositionNormalTexture(new Vector3(x2, 0, z2), );
                //top
                verticestop[3 * i + 0] = new VertexPositionNormalTexture(new Vector3(x, altura, z), );
                verticestop[3 * i + 1] = new VertexPositionNormalTexture(new Vector3(0, altura, 0), );
                verticestop[3 * i + 2] = new VertexPositionNormalTexture(new Vector3(x2, altura, z2), );
                //lado1
                verticeslado[2 * i + 0] = new VertexPositionNormalTexture(new Vector3(x, 0, z), v1Normal,  );
                verticeslado[2 * i + 1] = new VertexPositionNormalTexture(new Vector3(x, altura, z), v2Normal, );

            }
        }

        public void Draw(GraphicsDevice device)
        {
            effect.World = worldMatrix;

            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, verticesbase, 0, nlados);
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, verticestop, 0, nlados);
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, verticeslado, 0, nlados);
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, verticesoutrolado, 0, nlados);
        }

    }