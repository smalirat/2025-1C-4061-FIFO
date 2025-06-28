#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Matrices
float4x4 WorldViewProjection;
float4x4 World;
float4x4 InverseTransposeWorld;

// Parametros de iluminacion
float3 ambientColor;
float3 diffuseColor;
float3 specularColor;

// Coeficientes de intensidad
float KAmbient;
float KDiffuse;
float KSpecular;
float shininess;

// Posiciones
float3 lightPosition;
float3 eyePosition; // Camera position

// Texturas
texture baseTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (baseTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};


// Glow
bool useGlow;

// Emissive "energía"
float3 emissiveColor = float3(0, 0, 0);
float emissiveStrength = 0;

// Estructuras
struct VertexShaderInput
{
	float4 Position : POSITION0;
    float4 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    //float4 Normal : TEXCOORD2;
    float2 TextureCoordinates : TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
    float4 Normal : TEXCOORD2;
};

// ===== VERTEX SHADER =====
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    // Restablezco el output
	VertexShaderOutput output = (VertexShaderOutput)0;

    // Propagamos la posicion del vertice al espacio de mundo
    output.WorldPosition = mul(input.Position, World);

    // Propagamos el resultado de multiplicar las matrices: Local -> (Mundo -> Vista -> Proyeccion)
    output.Position = mul(input.Position, WorldViewProjection);

    // Propagamos la normal a espacio de mundo
    output.Normal = mul(input.Normal, InverseTransposeWorld);

    // Propagamos la coordenadas de textura
    output.TextureCoordinates = input.TextureCoordinates;

	return output;
}

// ===== PIXEL SHADER =====
float4 MainPS(VertexShaderOutput input) : COLOR
{
    // direccion hacia la luz
    float3 lightDirection = normalize(lightPosition - input.WorldPosition.xyz);
    // direccion hacia la camara
    float3 viewDirection = normalize(eyePosition - input.WorldPosition.xyz);

    float3 halfVector = normalize(lightDirection + viewDirection);

	// Color base de la Textura
    float4 texelColor = tex2D(textureSampler, input.TextureCoordinates);

	// Calculo de la luz difusa
    float NdotL = saturate(dot(input.Normal.xyz, lightDirection));
    float3 diffuseLight = KDiffuse * diffuseColor * NdotL;

	// Calculo de la luz especular
    float NdotH = dot(input.Normal.xyz, halfVector);
    float3 specularLight = sign(NdotL) * KSpecular * specularColor * pow(saturate(NdotH), shininess);

    // Calculo final Sin brillo extra
    float3 baseLighting = saturate(ambientColor * KAmbient + diffuseLight);
    float4 finalColor;
    // finalColor = float4(baseLighting * texelColor.rgb + specularLight, texelColor.a);    
    float3 finalRGB = baseLighting * texelColor.rgb + specularLight;

    // Si usa Glow 
    if(useGlow) {
        // Agregado de brillo
        //--float edgeFactor = 1.0 - saturate(dot(normalize(input.Normal.xyz), viewDirection)); // Calcula cuánto se aleja la normal del vértice respecto a la vista (más cerca de 1 => más borde)
        //--float glowStrength = pow(edgeFactor, 2.0); // Exageramos esa diferencia para que el borde se vea más brillante
        //float3 glowColor = float3(1.0, 0.5, 0.5); // naranja fuerte
        //float glowIntensity = 0; 

        //finalRGB += glowColor * glowIntensity; //  * glowStrength 
        //finalRGB = float3(10.0, 0.0, 0.0);// para test

        finalRGB += emissiveColor * emissiveStrength;
        finalRGB = saturate(finalRGB);
    }

    return float4(finalRGB, texelColor.a);
}

technique BasicColorDrawing
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};