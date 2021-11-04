using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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
    private float[] _heightMap;



    public ClsTerreno(GraphicsDevice device, Texture2D heightMap, Texture2D texture)
    {
        effect = new BasicEffect(device);
        float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;

        effect.View = Matrix.CreateLookAt(new Vector3(64.0f, 20.0f, 64.0f), new Vector3(64.0f, 0.0f, 0.0f), Vector3.Up);
        effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), aspectRatio, 0.1f, 10000f);
        effect.LightingEnabled = false;
        effect.VertexColorEnabled = false;
        effect.TextureEnabled = true;
        effect.Texture = texture;
        effect.World = Matrix.Identity;

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

    public void CreateGeometry(GraphicsDevice device, Texture2D heightMap)
    {

        Color[] texelColors = new Color[vertexCount];
        heightMap.GetData<Color>(texelColors);
        vScale = 0.04f;

        //altura do vertice = texels[number].R *vScale
       
        alturas = new float[width, height];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float y = texelColors[width * z + x].R * vScale;  //gotta make sure this index is right
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
        vertexBuffer.SetData<VertexPositionNormalTexture>(vertex);
        indexBuffer.SetData<short>(index);
    }
    public float GetHeight(float u, float z)
    {
        float yA, yB, yC, yD;
        Vector2[] pontos = new Vector2[4]; // four point, 'cause we used a square
        pontos[0] = new Vector2((int)u, (int)z);
        pontos[1] = new Vector2((int)u+1, (int)z);
        pontos[2] = new Vector2((int)u, (int)z+1);
        pontos[3] = new Vector2((int)u+1, (int)z+1);


        #region Interpolation in the X axis
        //weight of A
        float wA = Math.Abs(pontos[0].X - u);
        //weight of B
        float wB = Math.Abs(pontos[1].X - u);
        //weight of C
        float wC = Math.Abs(pontos[2].X - u);
        //weight of D
        float wD = Math.Abs(pontos[3].X - u);
        #endregion
        
        #region Interpolation in the Z axis
        //straight AB
        float wAB = Math.Abs(pontos[0].Y - z);
        //straight CD
        float wCD = Math.Abs(pontos[2].Y - z);
        #endregion


        yA = alturas[Math.Abs((int)pontos[0].X), Math.Abs((int)pontos[0].Y)];
        yB = alturas[Math.Abs((int)pontos[1].X), Math.Abs((int)pontos[1].Y)];
        yC = alturas[Math.Abs((int)pontos[2].X), Math.Abs((int)pontos[2].Y)];
        yD = alturas[Math.Abs((int)pontos[3].X), Math.Abs((int)pontos[3].Y)];


        /* quick guide for the bi-interpolation
         * Map point = int , We need subtract my cam point - int point ( When close to the point the more relative weight it as)
         * 0.1 > 0.9
         */
        #region Interpolation 
        //interpolation of A to B
        float yAB = (yA* wB) +
            (yB * wA);

        //Interpolation between C and D
        float yCD = (yD * wC) +
            (yC * wD);
        #endregion

        //Camera Height
        float camSurf = (yAB * wCD) + (yCD * wAB);
        return camSurf;
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
            device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0,i* indexOffset, (height * 2) - 2);
        }
    }

}