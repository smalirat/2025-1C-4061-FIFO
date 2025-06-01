#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

texture ModelTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (ModelTexture);
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
    output.Position = input.Position;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    // Calcular la distancia desde el centro de la textura
    float2 center = float2(0.5, 0.5);
    float2 pos = input.TextureCoordinates - center;
    float dist = length(pos);

    // Si estamos fuera del círculo, hacer transparente
    if (dist > 0.5)
        return float4(0, 0, 0, 0);

    // Si estamos dentro, mostrar la textura
    float4 color = tex2D(textureSampler, input.TextureCoordinates);
    return color;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}