#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

uniform float4x4 World;
uniform float4x4 View;
uniform float4x4 Projection;
uniform float3 DiffuseColor;

// Vertice
struct VertexShaderInput
{
	float4 Position : POSITION0;
};

// Vertice Proyectado
struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
};

// Vertex Shader
// Se aplican las transformaciones estándar: Local → Mundo → Vista → Proyeccion
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    // Restablezco el output
	VertexShaderOutput output = (VertexShaderOutput)0;
	
    // Multiplico matrices: Local → Mundo
    float4 worldPosition = mul(input.Position, World);
	
	// Multiplico matrices: Mundo → Vista
    float4 viewPosition = mul(worldPosition, View);	
	
	// Multiplico matrices: Vista → Proyeccion
    output.Position = mul(viewPosition, Projection);

    return output;
}

// Fragment Shader
// Opacidad al 100% y color uniforme
float4 MainPS(VertexShaderOutput input) : COLOR
{
    return float4(DiffuseColor, 1.0);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
