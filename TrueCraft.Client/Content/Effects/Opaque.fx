
#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 world;
float4x4 view;
float4x4 projection;
float4x4 worldInvT;

float4 transform(float4 position) {
	float4 result = position;
	result = mul(result, world);
	result = mul(result, view);
	result = mul(result, projection);
	return result;
}

sampler2D texture0;

float4 getMaterialColor(float4 color, float2 texCoord0) {
	float4 result = float4(color.rgb * color.a, 1.0);
	result *= tex2D(texture0, texCoord0);
	return saturate(result);
}

float3 ambient;
float ambientIntensity;

float4 getAmbient() {
	float4 result = float4(ambient.rgb * ambientIntensity, 1.0);
	return saturate(result);
}

float3 diffuse;
float diffuseIntensity;
float3 diffuseDirection;

float4 getDiffuse(float4 normal) {
	float4 result = mul(normalize(normal), transpose(world));
	float intensity = dot(normalize(normal), diffuseDirection);
	return saturate(float4(diffuse.rgb * diffuseIntensity * intensity, 1.0));
}

struct vertexInput {
	float4 position : SV_POSITION;
	float4 normal : NORMAL0;
	float4 color : COLOR0;
	float2 texCoord0 : TEXCOORD0;
};

struct pixelInput {
	float4 position : SV_POSITION;
	float4 color : COLOR0;
	float2 texCoord0 : TEXCOORD0;
	float4 diffuseColor : TEXCOORD1;
};

pixelInput vertexMain(vertexInput input) {
	pixelInput output = (pixelInput)0;

	output.position = transform(input.position);
	output.diffuseColor = getDiffuse(input.normal);
	output.color = input.color;
	output.texCoord0 = input.texCoord0;

	return output;
}

float4 pixelMain(pixelInput input) : COLOR0 {
	float4 output = float4(1.0, 1.0, 1.0, 1.0);

	output *= getMaterialColor(input.color, input.texCoord0);
	output *= (getAmbient() + input.diffuseColor);
	
	return saturate(output);
}

technique Opaque
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL vertexMain();
		PixelShader = compile PS_SHADERMODEL pixelMain();
	}
};