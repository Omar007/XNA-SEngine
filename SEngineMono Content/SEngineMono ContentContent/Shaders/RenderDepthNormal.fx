float4x4 World;
float4x4 View;
float4x4 Projection;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Normal : NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 Depth : TEXCOORD0;
	float4 Normal : TEXCOORD1;
};

struct PixelShaderOutput
{
	//Write to render targets with index 0 and 1.
	float4 Depth : COLOR0;
	float4 Normal : COLOR1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float4x4 wvp = mul(World, mul(View, Projection));

    output.Position = mul(input.Position, wvp);
	output.Depth = 1.0f;
	output.Depth.xy = output.Position.zw;
	output.Normal = mul(World, input.Normal);

    return output;
}

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
	PixelShaderOutput output;

	float d = input.Depth.x / input.Depth.y;
	float n = 0.5f * (input.Normal + 1.0f);

	output.Depth = float4(d, d, d, 1.0f);
	output.Normal = float4(n, n, n, 1.0f);

	return output;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
