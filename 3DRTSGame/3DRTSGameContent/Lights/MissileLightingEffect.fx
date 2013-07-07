// Textureが無いモデル用のProject...V4エフェクトの改変
float4x4 World;
float4x4 View;
float4x4 Projection;

texture2D LightTexture;
sampler2D lightSampler = sampler_state
{
	texture = <LightTexture>;
	minfilter = point;
	magfilter = point;
	mipfilter = point;
};
float3 AmbientColor = float3(0.15, 0.15, 0.15);
float3 DiffuseColor;
#include "..//PPShared.vsi"

// shadow関係
bool DoShadowMapping = false;
float4x4 ShadowView;
float4x4 ShadowProjection;
texture2D ShadowMap;
sampler2D shadowSampler = sampler_state {
	texture = <ShadowMap>;
	minfilter = point;
	magfilter = point;
	mipfilter = point;
};

// perform the depth comparison
float3 ShadowLightPosition;
float ShadowFarPlane;
float ShadowMult = 0.3f;
float ShadowBias =  0.001f;

// water関係
float4 ClipPlane;
bool ClipPlaneEnabled = true;

// Rim lighting
bool DoRimLighting = false;
float4 RimColor;
float RimIntensity = 0.5f;         // Intensity of the rim light
//float3 CenterToCamera;
float3 CameraPosition;
float Shinniness = 0.5f;


struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL0;
};
struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 PositionCopy : TEXCOORD1;
	float4 ShadowScreenPosition : TEXCOORD2;

	float3 WorldPosition : TEXCOORD3;
	float3 Normal : TEXCOORD4;
};
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
	// water用
	float4 worldPosition = mul(input.Position, World);
	output.WorldPosition = worldPosition;

	float4x4 worldViewProjection = mul(World, mul(View, Projection));
	output.Position = mul(input.Position, worldViewProjection);
	output.PositionCopy = output.Position;

	output.ShadowScreenPosition = mul(mul(input.Position, World),
		mul(ShadowView, ShadowProjection));
	output.Normal = mul(input.Normal, World);

	return output;
}

float2 sampleShadowMap(float2 UV)
{
	if (UV.x < 0 || UV.x > 1 || UV.y < 0 || UV.y > 1) {
		return float2(1, 1);
	}

	float2 debug = tex2D(shadowSampler, UV).rg;
	return debug;
	//return tex2D(shadowSampler, UV).rg;// returns read and green value
}
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	if (ClipPlaneEnabled) {
	 // >指定された値が 0 より小さい場合に、現在のピクセルを破棄します
		clip(dot(float4(input.WorldPosition, 1), ClipPlane));
	}

	// Extract lighting value from light map
	float2 texCoord = postProjToScreen(input.PositionCopy) + halfPixel();
	float3 light = tex2D(lightSampler, texCoord);
	light += AmbientColor;

	float shadow = 1;
	/*if (DoShadowMapping) {
		float2 shadowTexCoord = postProjToScreen(input.ShadowScreenPosition)
			+ halfPixel();
		float ShadowBias = 0.001f;//0.001f
		float realDepth = input.ShadowScreenPosition.z / ShadowFarPlane - ShadowBias;

		if (realDepth < 1) {// realDepth > momentsなら描画？
			// Variance shadow mapping code below from the variance shadow
			// mapping demo code @ http://www.punkuser.net/vsm/

			// Sample from depth texture
			float2 moments = sampleShadowMap(shadowTexCoord);

			// check if we're in shadow
			// momentsは元々depthを出力させているのでどちらの要素も同じ
			float lit_factor = (realDepth <= moments.x);// ピクセルが影かどうか：つまり0 or 1

			// Variance shadow mapping
			float E_x2 = moments.y;									// = E(x^2)
			float Ex_2 = moments.x * moments.x;						// (E(x))^2

			// 分散 = s^2 = E(x^2) - (E(x))^2 = M_2 - (M_1)^2 (pdfより)
			//float variance = min(max(E_x2 - Ex_2, 0.0) + 1.0f / 10000.0f, 1.0);
			float variance = min(max(E_x2 - Ex_2, 0.0) + 1.0f / 10000.0f, 1.0);
			float m_d = (moments.x - realDepth);					// =t-μ
			float p = variance / (variance + m_d * m_d);			// pdfの(5)式 p=1になってしまう←floatをfloat2にいれていたせい

			shadow = clamp(max(lit_factor, p), ShadowMult, 1.0f);
		}
	}*/


	// Rim lighting
	float4 rim;
	if (DoRimLighting) {
		float3 normal = normalize(input.Normal);
		float3 viewDir = normalize(CameraPosition - input.WorldPosition);
		float crossValue = dot(viewDir, normal);
		rim = pow(1 - crossValue, 1.5f) * RimColor * RimIntensity;
	}


	float4 finalColor = float4(DiffuseColor * light * shadow + rim, 1);
	return finalColor;
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