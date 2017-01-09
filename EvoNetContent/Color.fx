float4 Color;


struct VertexShaderInput
{
	float4 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Position = input.Position;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return float4(pow(Color.rgb, 1.0f/2.2f), Color.a);
}

technique Technique1
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
