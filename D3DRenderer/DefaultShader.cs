using SlimDX.Direct3D11;
using SlimDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3DRenderer
{
    static class DefaultShader
    {
        public static InputElement[] InputElements = new[] {
            new InputElement("POSITION_IN", 0, Format.R32G32B32_Float, 0),
            new InputElement("TEXCOORD_IN", 0, Format.R32G32_Float, 0),
            new InputElement("COLOR_IN", 0, Format.R32G32B32A32_Float, 0)
        };

        public const string Source = @"Texture2D <float4> Texture : register(t0);
SamplerState TextureSampler : register(s0);

cbuffer MATRIX_BUFFER : register(b0)
{
    matrix WorldToViewport;
}

struct VShader_Out
{
	float4 position : SV_POSITION;
	float2 uv : TEXCOORD;
    float4 color : COLOR0;
};

VShader_Out VShader(float3 position : POSITION_IN, float2 uv : TEXCOORD_IN, float4 color : COLOR_IN)
{
	VShader_Out ret;
	ret.position = mul(WorldToViewport, float4(position.x, position.y, position.z, 1.0));
	ret.uv = uv;
    ret.color = color;
	return ret;
}

float4 PShader(VShader_Out input) : SV_Target
{
    return Texture.Sample(TextureSampler, input.uv) * input.color;
}
";
    }
}
