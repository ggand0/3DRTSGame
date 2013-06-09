float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: ここでエフェクトのパラメーターを追加します。
// test
float3 offset;
float4x4 Transform;
struct VertexPositionColor
{
	float4 Position : POSITION;
	float4 Color : COLOR;
};

VertexPositionColor TestVertexShader(VertexPositionColor input)
{
	// 回転エフェクト用
	/*input.Position.xyz += offset;
	return input;*/

	// 3D化
	// mul ( vector, matrix )を使う：積算をする関数
	// vectorは変換前のベクトル、matrixは変換を表すマトリックス。
	// 戻り値は変換後のベクトル。
	VertexPositionColor output = input;
	output.Position = mul(input.Position, Transform);
	return output;
}
/*float4 TestVertexShader(float4 position:POSITION):POSITION
{
	return position;
}*/

// float4は４つの成分を持つベクトルで、位置や色を表すのに使われる
float4 TestPixelShader(float4 color : COLOR) : COLOR
{
	/*color.rb = color.br;// 色の位置が反転
	return color;//float4(1, 1, 1, 1);*/
	//return 1 - color;// 色反転

	// pink lightっぽいエフェクト。この演算はベクトルの内積や外積とは違い、それぞれの成分同士の積が新たな成分となるような演算
	/*float4 lightColor = float4(1, 0, 0.5f, 1);
	return lightColor * color;*/

	// 縞のようなエフェクト
	/*if (color.r % 0.1f < 0.05f) return float4(1, 1, 1, 1);
	else return color;*/
	
	return color;
}

technique testTechnique
{
	pass testPass
	{
		VertexShader = compile vs_2_0 TestVertexShader();
		PixelShader = compile ps_2_0 TestPixelShader();
	}
}