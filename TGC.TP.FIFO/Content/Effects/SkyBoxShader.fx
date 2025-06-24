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

// Textura
texture ModelTexture;
samplerCUBE textureSampler = sampler_state
{
    Texture = (ModelTexture);
    Magfilter = Linear;
    Minfilter = Linear;
    Mipfilter = Linear;
    AddressU = Mirror;
    AddressV = Mirror;
};

// Entrada al vertex shader
struct VertexShaderInput
{
    float4 Position : POSITION0;
};

// Salida del vertex shader
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 TextureCoordinate : TEXCOORD0;
};

// Vertex Shader
// Se aplican las transformaciones estándar: Local → Mundo → Vista → Proyeccion
VertexShaderOutput MainVS(VertexShaderInput input)
{
    // Restablezco el output
    VertexShaderOutput output = (VertexShaderOutput)0;
	
    // Multiplico matrices: Local → Mundo
    float4 worldPosition = mul(input.Position, World);
	
	// Multiplico matrices: Mundo → Vista
    float4 viewPosition = mul(worldPosition, View);
	
	// Multiplico matrices: Vista → Proyeccion
    output.Position = mul(viewPosition, Projection);
    
    // Propagamos las coordenadas de textura
    output.TextureCoordinate = worldPosition.xyz;

    return output;
}

// Fragment Shader
float4 MainPS(VertexShaderOutput input) : COLOR0
{
    return float4(texCUBE(textureSampler, normalize(input.TextureCoordinate)).rgb, 1);
}

technique BasicTextureDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
