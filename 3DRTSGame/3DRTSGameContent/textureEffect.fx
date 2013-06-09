float4x4 View;
float4x4 Projection;

// TODO: ここでエフェクトのパラメーターを追加します。
texture testTexture;
sampler testSampler = sampler_state {
	Texture = <testTexture>;
};
struct VertexPositionTexture
{
	float4 Position : POSITION;
	float4 TextureCoordinate : TEXCOORD;
};

VertexPositionTexture VertexShaderFunction(VertexPositionTexture input)
{
	VertexPositionTexture output;
	output.Position = mul(input.Position, mul(View, Projection));
	output.TextureCoordinate = input.TextureCoordinate;

	return output;
}

float4 PixelShaderFunction(float2 textureCoordinate : TEXCOORD) : COLOR
{
    // TODO: ここでピクセル シェーダー コードを追加します。
	return tex2D(testSampler, textureCoordinate);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: ここでレンダーステートを設定します。

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
