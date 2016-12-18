#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_2_0
#define PS_SHADERMODEL ps_2_0

Texture2D SandTexture;
Texture2D GrassTexture;
Texture2D BlendMap;

sampler2D SandTexturesSampler
{
    Texture = <SandTexture>;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler2D GrassTextureSampler
{
    Texture = <GrassTexture>;
    AddressU = Wrap;
    AddressV = Wrap;
};
sampler2D BlendMapSampler
{
    Texture = <BlendMap>;
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
    float4 SandColor = tex2D(SandTexturesSampler,input.TextureCoordinates);
    float4 GrassColor = tex2D(GrassTextureSampler, input.TextureCoordinates);
    float4 BlendColor = tex2D(BlendMapSampler, input.TextureCoordinates);
    return input.Color.a > BlendColor.r ? SandColor : GrassColor;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
