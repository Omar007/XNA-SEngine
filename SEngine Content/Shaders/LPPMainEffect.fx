//-----------------------------------------------------------------------------
// LPPMainEffect.fx
//
// Jorge Adriano Luna 2011
// http://jcoluna.wordpress.com
//
// It uses some code from Nomal Mapping Sample found at
// http://create.msdn.com/en-US/education/catalog/sample/normal_mapping
// and also code from here
// http://aras-p.info/texts/CompactNormalStorage.html
//-----------------------------------------------------------------------------

//as we used a 0.1f scale when rendering to light buffer,
//revert it back here.
const static float LightBufferScaleInv = 10.0f;

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 LightViewProj; //used when rendering to shadow map

float FarClip;
float2 LightBufferPixelSize;

//we should use one of these 4 defines to compute ambient color:
//NO_AMBIENT -- default, you don't need to define it
//AMBIENT_COLOR --we use an external variable to modulate the diffuse color
//AMBIENT_CUBEMAP --we use a cubemap to encode ambient light information
#define AMBIENT_COLOR

#ifndef NO_AMBIENT
float4 AmbientColor;

#ifdef AMBIENT_CUBEMAP
texture AmbientCubeMap;
samplerCUBE ambientCubemapSampler = sampler_state
{
	Texture = <AmbientCubeMap>;
	MinFilter=LINEAR;
	MagFilter=LINEAR;
	MipFilter=LINEAR;
	AddressU = WRAP;
	AddressV = WRAP;
};
#endif
#endif

#ifdef ALPHA_MASKED
float AlphaReference;
#endif

//-----------------------------------------
// Textures
//-----------------------------------------
texture DiffuseMap;
sampler diffuseMapSampler = sampler_state
{
	Texture = (DiffuseMap);
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture SpecularMap;
sampler specularMapSampler = sampler_state
{
	Texture = (SpecularMap);
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture NormalMap;
sampler normalMapSampler = sampler_state
{
	Texture = (NormalMap);
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture EmissiveMap;
sampler emissiveMapSampler = sampler_state
{
	Texture = (EmissiveMap);
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};

texture LightBuffer;
sampler2D lightSampler = sampler_state
{
	Texture = <LightBuffer>;
	MipFilter = POINT;
	MagFilter = POINT;
	MinFilter = POINT;
	AddressU = Clamp;
	AddressV = Clamp;
};


texture LightSpecularBuffer;
sampler2D lightSpecularSampler = sampler_state
{
	Texture = <LightSpecularBuffer>;
	MipFilter = NONE;
	MagFilter = POINT;
	MinFilter = POINT;
	AddressU = Clamp;
	AddressV = Clamp;
};



#ifdef DUAL_LAYER
texture SecondDiffuseMap;
sampler secondDiffuseMapSampler = sampler_state
{
	Texture = (SecondDiffuseMap);
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};


texture SecondSpecularMap;
sampler secondSpecularMapSampler = sampler_state
{
	Texture = (SecondSpecularMap);
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};


texture SecondNormalMap;
sampler secondNormalMapSampler = sampler_state
{
	Texture = (SecondNormalMap);
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
	AddressU = Wrap;
	AddressV = Wrap;
};
#endif
//-------------------------------
// Helper functions
//-------------------------------
half2 EncodeNormal (half3 n)
{
	float kScale = 1.7777;
	float2 enc;
	enc = n.xy / (n.z+1);
	enc /= kScale;
	enc = enc*0.5+0.5;
	return enc;
}

float2 PostProjectionSpaceToScreenSpace(float4 pos)
{
	float2 screenPos = pos.xy / pos.w;
	return (0.5f * (float2(screenPos.x, -screenPos.y) + 1));
}

half3 NormalMapToSpaceNormal(half3 normalMap, float3 normal, float3 binormal, float3 tangent)
{
	normalMap = normalMap * 2 - 1;
    normalMap = half3(normal * normalMap.z + normalMap.x * tangent - normalMap.y * binormal);
	return normalMap;
}


//-------------------------------
// Shaders
//-------------------------------

#ifdef SKINNED_MESH

#define MaxBones 82
float4x3 Bones[MaxBones];

float4x4 CalcSkinTransform(float4 BoneIndices, float4 BoneWeights)
{
	//float4x4 skinTransform = 0;
	//skinTransform += Bones[input.BoneIndices.x] * input.BoneWeights.x;
    //skinTransform += Bones[input.BoneIndices.y] * input.BoneWeights.y;
    //skinTransform += Bones[input.BoneIndices.z] * input.BoneWeights.z;
    //skinTransform += Bones[input.BoneIndices.w] * input.BoneWeights.w;
	//return skinTransform;

	// Calculate the final bone transformation matrix
	float4x3 skinTransform = 0;
    skinTransform += Bones[BoneIndices.x] * BoneWeights.x;
    skinTransform += Bones[BoneIndices.y] * BoneWeights.y;
    skinTransform += Bones[BoneIndices.z] * BoneWeights.z;
    skinTransform += Bones[BoneIndices.w] * BoneWeights.w;

    float4x4 finalSkinTransform = 0;
    finalSkinTransform[0] = float4(skinTransform[0], 0);
    finalSkinTransform[1] = float4(skinTransform[1], 0);
    finalSkinTransform[2] = float4(skinTransform[2], 0);
    finalSkinTransform[3] = float4(skinTransform[3], 1);

	return finalSkinTransform;
}

#endif

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal	: NORMAL0;
	float3 Binormal  : BINORMAL0;
	float3 Tangent  : TANGENT0;
#ifdef SKINNED_MESH
    float4 BoneIndices : BLENDINDICES0;
    float4 BoneWeights : BLENDWEIGHT0;
#endif
#ifdef DUAL_LAYER
    float4 Color   : COLOR0;		
#endif
};


struct VertexShaderOutput
{
    float4 Position			: POSITION0;
    float3 TexCoord			: TEXCOORD0;
    float Depth				: TEXCOORD1;
	
    float3 Normal	: TEXCOORD2;
    float3 Tangent	: TEXCOORD3;
    float3 Binormal : TEXCOORD4; 
};

struct PixelShaderInput
{
    float4 Position			: POSITION0;
    float3 TexCoord			: TEXCOORD0;
    float Depth				: TEXCOORD1;
	
    float3 Normal	: TEXCOORD2;
    float3 Tangent	: TEXCOORD3;
    float3 Binormal : TEXCOORD4; 	
	
	//we need this to detect back facing triangles
#ifdef ALPHA_MASKED	
	float Face : VFACE;
#endif
};

VertexShaderOutput RenderToGBufferVertexCommon(VertexShaderInput input, float4x4 instanceTransform) 
{
    VertexShaderOutput output = (VertexShaderOutput)0;

	float4x4 worldView = mul(instanceTransform, View);
	float4x4 worldViewProjection = mul(worldView, Projection);
#ifdef SKINNED_MESH
	// Blend between the weighted bone matrices.
	float4x4 skinTransform = CalcSkinTransform(input.BoneIndices, input.BoneWeights);
	
	float4 skinPos = mul(input.Position, skinTransform);
	float3 skinNormal = mul(input.Normal, skinTransform);
	float3 skinTangent = mul(input.Tangent, skinTransform);
	float3 skinBinormal = mul(input.Binormal, skinTransform);
	
	float3 viewSpacePos = mul(skinPos, worldView);
    output.Position = mul(skinPos, worldViewProjection);
	
    output.TexCoord.xy = input.TexCoord; //pass the texture coordinates further
	
	//we output our normals/tangents/binormals in viewspace
    output.Normal =  mul( skinNormal   , worldView); //worldView
	output.Tangent = mul( skinTangent  , worldView);
	output.Binormal =mul( skinBinormal , worldView);

#else

	float3 viewSpacePos = mul(input.Position, worldView);
    output.Position = mul(input.Position, worldViewProjection);
    output.TexCoord.xy = input.TexCoord; //pass the texture coordinates further

	//we output our normals/tangents/binormals in viewspace
	output.Normal = normalize   (mul( input.Normal  , worldView));
	output.Tangent =  normalize (mul( input.Tangent , worldView));
	output.Binormal =  normalize(mul( input.Binormal, worldView));
#endif

#ifdef DUAL_LAYER
    output.TexCoord.z = input.Color.r;	
#endif
		
	output.Depth = viewSpacePos.z; //pass depth
    return output;
}


//render to our 2 render targets, normal and depth 
struct PixelShaderOutput
{
    float4 Normal : COLOR0;
    float4 Depth : COLOR1;
};

PixelShaderOutput RenderToGBufferPixelShader(PixelShaderInput input)
{
	PixelShaderOutput output = (PixelShaderOutput)1;   

	//if we are using alpha mask, we need to read the diffuse map	
#ifdef ALPHA_MASKED
	half4 diffuseMap = tex2D(diffuseMapSampler, input.TexCoord);
	clip(diffuseMap.a - AlphaReference);	
#elif defined(DUAL_LAYER)
	float transition = tex2D(diffuseMapSampler, input.TexCoord).a;
	transition = (1.3f*input.TexCoord.z-0.15f) - transition;		
	transition = saturate(transition*5);
#endif

	//read from our normal map
	half4 normalMap = tex2D(normalMapSampler, input.TexCoord);
		
#ifdef DUAL_LAYER
	normalMap = normalMap * transition + tex2D(secondNormalMapSampler, input.TexCoord) * (1-transition);
#endif

	half3 normalViewSpace = NormalMapToSpaceNormal(normalMap.xyz, input.Normal, input.Binormal, input.Tangent);
    
	//if we are using alpha mask, we need to invert the normal if its a back face
#ifdef ALPHA_MASKED	
	normalViewSpace = normalViewSpace * sign(input.Face);
#endif

	output.Normal.rg =  EncodeNormal (normalize(normalViewSpace));	//our encoder output in RG channels
	output.Normal.b = normalMap.a;	//our specular power goes into B channel
	//output.Normal.a = 1;					//not used
	output.Depth.x = -input.Depth/ FarClip;		//output Depth in linear space, [0..1]
	
	return output;
}


struct ReconstructVertexShaderInput
{
    float4 Position  : POSITION0;
    float2 TexCoord  : TEXCOORD0;
#ifdef AMBIENT_CUBEMAP
	float3 Normal	 : NORMAL0;
#endif
#ifdef SKINNED_MESH
    float4 BoneIndices : BLENDINDICES0;
    float4 BoneWeights : BLENDWEIGHT0;
#endif
#ifdef DUAL_LAYER
    float4 Color   : COLOR0;		
#endif
};




struct ReconstructVertexShaderOutput
{
    float4 Position			: POSITION0;
    float3 TexCoord			: TEXCOORD0;
	float4 TexCoordScreenSpace : TEXCOORD1;
#ifdef AMBIENT_CUBEMAP
	float3 Normal	 : TEXCOORD2;
#endif
};


ReconstructVertexShaderOutput ReconstructVertexShaderCommon(ReconstructVertexShaderInput input, float4x4 instanceTransform)
{
    ReconstructVertexShaderOutput output=(ReconstructVertexShaderOutput)0;
	
	float4x4 worldView = mul(instanceTransform, View);
	float4x4 worldViewProjection = mul(worldView, Projection);

#ifdef SKINNED_MESH
	// Blend between the weighted bone matrices.
	float4x4 skinTransform = CalcSkinTransform(input.BoneIndices, input.BoneWeights);

	float4 skinPos = mul(input.Position, skinTransform);

    output.Position = mul(skinPos, worldViewProjection);
	#ifdef AMBIENT_CUBEMAP
	float3 skinNormal = mul(input.Normal, skinTransform);
	output.Normal = normalize(mul(skinNormal,instanceTransform)); 
	#endif

#else

    output.Position = mul(input.Position, worldViewProjection);

	#ifdef AMBIENT_CUBEMAP
	output.Normal = normalize(mul(input.Normal,instanceTransform)); 
	#endif
#endif

    output.TexCoord.xy = input.TexCoord; //pass the texture coordinates further
	output.TexCoordScreenSpace = output.Position;
	
#ifdef DUAL_LAYER
    output.TexCoord.z = input.Color.r;	
#endif

    return output;
}


float4 ReconstructPixelShader(ReconstructVertexShaderOutput input):COLOR0
{
	PixelShaderOutput output = (PixelShaderOutput)1;   
	//read from our diffuse, specular and emissive maps
	half4 diffuseMap = tex2D(diffuseMapSampler, input.TexCoord);

	
#ifdef ALPHA_MASKED	
	clip(diffuseMap.a - AlphaReference);
#endif
	
	// Find the screen space texture coordinate and offset it
	float2 screenPos = PostProjectionSpaceToScreenSpace(input.TexCoordScreenSpace) + LightBufferPixelSize;



	half3 emissiveMap = tex2D(emissiveMapSampler, input.TexCoord).rgb;
	half3 specularMap = tex2D(specularMapSampler, input.TexCoord).rgb;
	
#ifdef DUAL_LAYER
	float transition = (1.3f*input.TexCoord.z - 0.15f) - diffuseMap.a;		
	transition = saturate(transition*5);
	diffuseMap.rgb = diffuseMap.rgb*transition + tex2D(secondDiffuseMapSampler, input.TexCoord).rgb*(1-transition);
	specularMap = specularMap.rgb*transition + tex2D(secondSpecularMapSampler, input.TexCoord).rgb*(1-transition);
#endif

	//read our light buffer texture. Remember to multiply by our magic constant explained on the blog
	float4 lightColor =  tex2D(lightSampler, screenPos) * LightBufferScaleInv;

	//our specular intensity is stored in a separate texture
	float4 specular =  tex2D(lightSpecularSampler, screenPos) * LightBufferScaleInv;
	
	float4 finalColor = float4(diffuseMap*lightColor.rgb + specular*specularMap + emissiveMap,1);

#ifdef AMBIENT_COLOR
	//add a small constant to avoid dark areas
	finalColor.rgb+= diffuseMap*AmbientColor.rgb;
#elif defined(AMBIENT_CUBEMAP)
	//fetch ambient cubemap using vertex normal. Mayb you will want to use the per-pixel normal. In this case,
	//you should fetch the normal buffer as we do with the lightBuffer, and recompute the global-space normal
	half3 ambientCubemapColor = texCUBE(ambientCubemapSampler,input.Normal);
	finalColor.rgb += AmbientColor.rgb*ambientCubemapColor.rgb*diffuseMap.rgb;
#endif

	return finalColor;
}


struct ShadowMapVertexShaderInput
{
    float4 Position : POSITION0;	
	//if we have alpha mask, we need to use the tex coord
#ifdef ALPHA_MASKED	
    float2 TexCoord  : TEXCOORD0;
#endif
#ifdef SKINNED_MESH
    float4 BoneIndices : BLENDINDICES0;
    float4 BoneWeights : BLENDWEIGHT0;
#endif

};

struct ShadowMapVertexShaderOutput
{
    float4 Position : POSITION0;
	float2 Depth : TEXCOORD0;
#ifdef ALPHA_MASKED	
    float2 TexCoord  : TEXCOORD1;
#endif
};


ShadowMapVertexShaderOutput OutputShadowVertexShaderCommon(ShadowMapVertexShaderInput input, float4x4 instanceTransform)
{
    ShadowMapVertexShaderOutput output = (ShadowMapVertexShaderOutput)0;
	
#ifdef SKINNED_MESH
	// Blend between the weighted bone matrices.
    float4x4 skinTransform = CalcSkinTransform(input.BoneIndices, input.BoneWeights);

	float4 skinPos = mul(input.Position, skinTransform);
    float4 clipPos = mul(skinPos, mul(instanceTransform, LightViewProj));
#else
    float4 clipPos = mul(input.Position, mul(instanceTransform, LightViewProj));
#endif
	//clamp to the near plane
	clipPos.z = max(clipPos.z,0);
	
	output.Position = clipPos;
	output.Depth = output.Position.zw;
	
#ifdef ALPHA_MASKED	
    output.TexCoord = input.TexCoord; //pass the texture coordinates further	
#endif
    return output;
}

float4 OutputShadowPixelShader(ShadowMapVertexShaderOutput input) : COLOR0
{
#ifdef ALPHA_MASKED	
	//read our diffuse
	half4 diffuseMap = tex2D(diffuseMapSampler, input.TexCoord);
	clip(diffuseMap.a - AlphaReference);
#endif

    float depth = input.Depth.x / input.Depth.y;	
    return float4(depth, 1, 1, 1); 
}



VertexShaderOutput RenderToGBufferVertexShader(VertexShaderInput input)
{
	return RenderToGBufferVertexCommon(input, World);
}

ReconstructVertexShaderOutput ReconstructVertexShader(ReconstructVertexShaderInput input)
{
	return ReconstructVertexShaderCommon(input, World);
}

ShadowMapVertexShaderOutput OutputShadowVertexShader(ShadowMapVertexShaderInput input)
{
	return OutputShadowVertexShaderCommon(input, World);
}



// Hardware instancing reads the per-instance world transform from a secondary vertex stream.
VertexShaderOutput RenderToGBufferInstancedVertexShader(VertexShaderInput input,
                                                  float4x4 instanceTransform : TEXCOORD1)
{
    return RenderToGBufferVertexCommon(input, transpose(instanceTransform));
}


ReconstructVertexShaderOutput ReconstructInstancedVertexShader(ReconstructVertexShaderInput input,
                                                  float4x4 instanceTransform : TEXCOORD1)
{
	return ReconstructVertexShaderCommon(input, transpose(instanceTransform));
}

ShadowMapVertexShaderOutput OutputShadowInstancedVertexShader(ShadowMapVertexShaderInput input,
                                                  float4x4 instanceTransform : TEXCOORD1)
{
	return OutputShadowVertexShaderCommon(input, transpose(instanceTransform));
}





technique RenderToGBuffer
{
    pass RenderToGBufferPass
    {
	#ifdef ALPHA_MASKED	
		CullMode = None;
	#else
		CullMode = CCW;
	#endif

        VertexShader = compile vs_3_0 RenderToGBufferVertexShader();
        PixelShader = compile ps_3_0 RenderToGBufferPixelShader();
    }
}

technique ReconstructShading
{
	pass ReconstructShadingPass
    {
	#ifdef ALPHA_MASKED	
		CullMode = None;
	#else
		CullMode = CCW;
	#endif

        VertexShader = compile vs_3_0 ReconstructVertexShader();
        PixelShader = compile ps_3_0 ReconstructPixelShader();
    }
}

technique OutputShadow
{
	pass OutputShadowPass
	{		
	#ifdef ALPHA_MASKED	
		CullMode = None;
	#else
		CullMode = CCW;
	#endif

        VertexShader = compile vs_3_0 OutputShadowVertexShader();
        PixelShader = compile ps_3_0 OutputShadowPixelShader();
	}
}



technique RenderToGBufferInstanced
{
    pass RenderToGBufferPass
    {
	#ifdef ALPHA_MASKED	
		CullMode = None;
	#else
		CullMode = CCW;
	#endif

        VertexShader = compile vs_3_0 RenderToGBufferInstancedVertexShader();
        PixelShader = compile ps_3_0 RenderToGBufferPixelShader();
    }
}

technique ReconstructShadingInstanced
{
	pass ReconstructShadingPass
    {
	#ifdef ALPHA_MASKED	
		CullMode = None;
	#else
		CullMode = CCW;
	#endif

        VertexShader = compile vs_3_0 ReconstructInstancedVertexShader();
        PixelShader = compile ps_3_0 ReconstructPixelShader();
    }
}

technique OutputShadowInstanced
{
	pass OutputShadowPass
	{		
	#ifdef ALPHA_MASKED	
		CullMode = None;
	#else
		CullMode = CCW;
	#endif

        VertexShader = compile vs_3_0 OutputShadowInstancedVertexShader();
        PixelShader = compile ps_3_0 OutputShadowPixelShader();
	}
}
