Shader "Super Text Mesh/UI/Outline" {
	Properties 
	{
		_MainTex ("Font Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}

        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,1)) = 0.05

		[Toggle(SDF_MODE)] _SDFMode ("Toggle SDF Mode", Float) = 0
		[ShowIf(SDF_MODE)] _SDFCutoff ("SDF Cutoff", Range(0,1)) = 0.5
		[ShowIf(SDF_MODE)] _Blend ("Blend Width", Range(0.0001,1)) = 0.05
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_ShadowCutoff ("Shadow Cutoff", Range(0,1)) = 0.5
		_Cutoff ("Cutoff", Range(0,1)) = 0.0001 //text cutoff
		[Enum(UnityEngine.Rendering.CullMode)] _CullMode ("Cull Mode", Float) = 0
		[Enum(Outside,0,Inside,1)] _MaskMode ("Mask Mode", Float) = 0
	}
	SubShader {
		Tags { 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"STMUberShader"="Yes"
		}
		LOD 100

		Lighting Off
		Cull [_CullMode]
		//special UI zTest Mode
		ZTest [unity_GUIZTestMode]
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Stencil
		{  
			Ref [_MaskMode]  //Customize this value  
			Comp Equal //Customize the compare function  
			Pass Keep
		} 
		Pass 
		{
			CGPROGRAM
			static float ANGLE = 0;
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "../STMoutline.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature SDF_MODE
			#pragma shader_feature PIXELSNAP_ON
			ENDCG
		}
		Pass 
		{
			CGPROGRAM
			static float ANGLE = 90;
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "../STMoutline.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature SDF_MODE
			#pragma shader_feature PIXELSNAP_ON
			ENDCG
		}
		Pass 
		{
			CGPROGRAM
			static float ANGLE = 180;
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "../STMoutline.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature SDF_MODE
			#pragma shader_feature PIXELSNAP_ON
			ENDCG
		}
		Pass 
		{
			CGPROGRAM
			static float ANGLE = 270;
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "../STMoutline.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature SDF_MODE
			#pragma shader_feature PIXELSNAP_ON
			ENDCG
		}
		Pass 
		{
			CGPROGRAM
			static float ANGLE = 45;
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "../STMoutline.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature SDF_MODE
			#pragma shader_feature PIXELSNAP_ON
			ENDCG
		}
		Pass 
		{
			CGPROGRAM
			static float ANGLE = 135;
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "../STMoutline.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature SDF_MODE
			#pragma shader_feature PIXELSNAP_ON
			ENDCG
		}
		Pass 
		{
			CGPROGRAM
			static float ANGLE = 225;
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "../STMoutline.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature SDF_MODE
			#pragma shader_feature PIXELSNAP_ON
			ENDCG
		}
		Pass 
		{
			CGPROGRAM
			static float ANGLE = 315;
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "../STMoutline.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature SDF_MODE
			#pragma shader_feature PIXELSNAP_ON
			ENDCG
		}
		Pass 
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "../STM.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature SDF_MODE
			#pragma shader_feature PIXELSNAP_ON
			ENDCG
		}
	}
	FallBack "GUI/Text Shader"
}
