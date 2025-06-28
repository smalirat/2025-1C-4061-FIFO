#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 WorldViewProjection;
float4x4 World;
float4x4 InverseTransposeWorld;

float3 ambientColor;
float3 diffuseColor;
float3 specularColor;
float KAmbient;
float KDiffuse;
float KSpecular;
float shininess;
float3 lightPosition;
float3 eyePosition;
float2 Tiling;

texture baseTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (baseTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
    MIPFILTER = LINEAR;
};

texture NormalTexture;
sampler2D normalSampler = sampler_state
{
    Texture = (NormalTexture);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

float3 getNormalFromMap(float2 textureCoordinates, float3x3 TBN)
{
    float3 tangentNormal = tex2D(normalSampler, textureCoordinates).xyz * 2.0 - 1.0;
    return normalize(mul(tangentNormal, TBN));
}

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
    float4 Normal : TEXCOORD2;
    float3x3 TBN : TEXCOORD3;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = mul(input.Position, WorldViewProjection);
    output.WorldPosition = mul(input.Position, World);
    output.Normal = mul(input.Normal, InverseTransposeWorld);
    output.TextureCoordinates = input.TextureCoordinates * Tiling;

    // Generar TBN
    float3 worldNormal = normalize(output.Normal.xyz);
    float3 tangent = float3(1, 0, 0);
    float3 bitangent = normalize(cross(worldNormal, tangent));
    tangent = normalize(cross(bitangent, worldNormal));
    output.TBN = float3x3(tangent, bitangent, worldNormal);

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 lightDirection = normalize(lightPosition - input.WorldPosition.xyz);
    float3 viewDirection = normalize(eyePosition - input.WorldPosition.xyz);
    float3 halfVector = normalize(lightDirection + viewDirection);
    float3 normal = normalize(input.Normal.xyz);

    float4 texelColor = tex2D(textureSampler, input.TextureCoordinates);

    float NdotL = saturate(dot(normal, lightDirection));
    float3 diffuseLight = KDiffuse * diffuseColor * NdotL;

    float NdotH = dot(normal, halfVector);
    float3 specularLight = sign(NdotL) * KSpecular * specularColor * pow(saturate(NdotH), shininess);

    float4 finalColor = float4(saturate(ambientColor * KAmbient + diffuseLight) * texelColor.rgb + specularLight, texelColor.a);
    return finalColor;
}

float4 NormalMapPS(VertexShaderOutput input) : COLOR
{
    float3 lightDirection = normalize(lightPosition - input.WorldPosition.xyz);
    float3 viewDirection = normalize(eyePosition - input.WorldPosition.xyz);
    float3 halfVector = normalize(lightDirection + viewDirection);

    float3 normal = getNormalFromMap(input.TextureCoordinates, input.TBN);

    float4 texelColor = tex2D(textureSampler, input.TextureCoordinates);

    float NdotL = saturate(dot(normal, lightDirection));
    float3 diffuseLight = KDiffuse * diffuseColor * NdotL;

    float NdotH = dot(normal, halfVector);
    float3 specularLight = KSpecular * specularColor * pow(saturate(NdotH), shininess);

    float4 finalColor = float4(saturate(ambientColor * KAmbient + diffuseLight) * texelColor.rgb + specularLight, texelColor.a);
    return finalColor;
}

struct GouraudVertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
};

struct GouraudVertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureCoordinates : TEXCOORD0;
    float3 Diffuse : TEXCOORD1;
    float3 Specular : TEXCOORD2;
};

GouraudVertexShaderOutput GouraudVS(in GouraudVertexShaderInput input)
{
    GouraudVertexShaderOutput output = (GouraudVertexShaderOutput)0;

    output.Position = mul(input.Position, WorldViewProjection);

    float3 worldPosition = mul(input.Position, World);
    float3 lightDirection = normalize(lightPosition - worldPosition);
    float3 viewDirection = normalize(eyePosition - worldPosition);
    float3 halfVector = normalize(lightDirection + viewDirection);
    float3 normal = normalize(mul(input.Normal, InverseTransposeWorld).xyz);

    float NdotL = saturate(dot(normal, lightDirection));
    float3 diffuseLight = KDiffuse * diffuseColor * NdotL;

    float NdotH = dot(normal, halfVector);
    float3 specularLight = KSpecular * specularColor * pow(saturate(NdotH), shininess);

    output.Diffuse = saturate(diffuseLight + ambientColor * KAmbient);
    output.Specular = specularLight;
    output.TextureCoordinates = input.TextureCoordinates * Tiling;

    return output;
}

float4 GouraudPS(GouraudVertexShaderOutput input) : COLOR
{
    float4 texelColor = tex2D(textureSampler, input.TextureCoordinates);
    float4 finalColor = float4(input.Diffuse * texelColor.rgb + input.Specular, texelColor.a);
    return finalColor;
}

technique Default
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

technique Gouraud
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL GouraudVS();
        PixelShader = compile PS_SHADERMODEL GouraudPS();
    }
};

technique NormalMapping
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL NormalMapPS();
    }
};
