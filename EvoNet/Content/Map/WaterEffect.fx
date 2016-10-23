#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D Water1;
Texture2D Water2;
float Time;

sampler2D Water1Sampler = sampler_state
{
	Texture = <Water1>;
    AddressU = Wrap;
    AddressV = Wrap;
};

sampler2D Water2Sampler = sampler_state
{
    Texture = <Water2>;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 Water1Color = tex2D(Water1Sampler, input.TextureCoordinates + float2(Time,Time));
    float4 Water2Color = tex2D(Water2Sampler, input.TextureCoordinates - float2(Time/1.3f, Time/2.6f));
	return Water1Color * Water2Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
