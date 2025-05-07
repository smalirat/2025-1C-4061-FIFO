#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Matrices
uniform float4x4 World;
uniform float4x4 View;
uniform float4x4 Projection;
uniform float3 DiffuseColor;

// Entrada de vertice
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
};

// Salida del vertex shader
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float3 LocalPos : TEXCOORD0;
    float3 WorldNormal : TEXCOORD1;
};

// Vertex Shader
VertexShaderOutput MainVS(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPos = mul(input.Position, World);
    float4 viewPos = mul(worldPos, View);
    output.Position = mul(viewPos, Projection);

    // Pasamos ña posicion local (sin transformarla) para detectar zona del ecuador
    output.LocalPos = input.Position.xyz;

    // Normal transformada a mundo
    output.WorldNormal = normalize(mul(float4(input.Normal, 0.0), World).xyz);

    return output;
}

// Pixel Shader
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float thickness = 0.05;

    // Franja horizontal "ecuador"
    float stripeEquator = abs(input.LocalPos.y) < thickness ? 1.0 : 0.0;

    // Franja vertical "meridiano"
    float stripeMeridian = abs(input.LocalPos.x) < thickness ? 1.0 : 0.0;

    // Color final: blanco en la franja, base en el resto
    float stripe = max(stripeEquator, stripeMeridian);
    float3 finalColor = stripe > 0.0 ? float3(1, 1, 1) : DiffuseColor;

    return float4(finalColor, 1.0);
}

// Tecnica
technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
