#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Obligatorio para que funcione con SpriteBatch
float4x4 MatrixTransform;

// SpriteBatch pasa automáticamente la textura con este nombre
texture Texture;

sampler2D textureSampler = sampler_state
{
    Texture = <Texture>;
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;
    // TRANSFORMACIÓN OBLIGATORIA PARA SPRITEBATCH
    output.Position = mul(input.Position, MatrixTransform);
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 center = float2(0.5, 0.5); // centro de la textura
    float2 offset = input.TextureCoordinates - center;
    float dist = length(offset);

    // Cortar fuera del círculo
    if (dist > 0.5)
        return float4(0, 0, 0, 0); // Transparente

    // Mostrar la textura dentro del círculo
    return tex2D(textureSampler, input.TextureCoordinates);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}