#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Matrices y parámetros
uniform float4x4 World;
uniform float4x4 View;
uniform float4x4 Projection;
uniform float2 Tiling;

// Textura
texture ModelTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (ModelTexture);
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput BaseTilingVS(in VertexShaderInput input)
{
    // Restablezco el output
    VertexShaderOutput output = (VertexShaderOutput) 0;
	
    // Multiplico matrices: Local → Mundo
    float4 worldPosition = mul(input.Position, World);
	
	// Multiplico matrices: Mundo → Vista
    float4 viewPosition = mul(worldPosition, View);
    
    // Multiplico matrices: Vista → Proyeccion
    output.Position = mul(viewPosition, Projection);

    // Propagate scaled Texture Coordinates
    output.TextureCoordinate = input.TextureCoordinate * Tiling;

    return output;
}

float4 BaseTilingPS(VertexShaderOutput input) : COLOR
{
    // Sample the texture using our scaled Texture Coordinates
    return tex2D(textureSampler, input.TextureCoordinate);
}

technique BaseTiling
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL BaseTilingVS();
        PixelShader = compile PS_SHADERMODEL BaseTilingPS();
    }
};

