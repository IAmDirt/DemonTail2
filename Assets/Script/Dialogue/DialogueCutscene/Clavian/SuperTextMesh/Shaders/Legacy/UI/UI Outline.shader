//Copyright (c) 2016-2018 Kai Clavier [kaiclavier.com] Do Not Distribute
Shader "Super Text Mesh/Legacy/UI/Outline" { 
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,1)) = 0.05
	}
	SubShader {
    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane"}
        Lighting Off Cull Off ZWrite Off Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
//cardinals
		CGPROGRAM
        static float ANGLE = 0;
        #include "../STMOutline.cginc"
        #pragma surface surf Lambert alpha vertex:vert
        ENDCG

        CGPROGRAM
        static float ANGLE = 90;
        #include "../STMOutline.cginc"
        #pragma surface surf Lambert alpha vertex:vert
        ENDCG

        CGPROGRAM
        static float ANGLE = 180;
        #include "../STMOutline.cginc"
        #pragma surface surf Lambert alpha vertex:vert
        ENDCG

        CGPROGRAM
        static float ANGLE = 270;
        #include "../STMOutline.cginc"
        #pragma surface surf Lambert alpha vertex:vert
        ENDCG
    //intercardinals
        CGPROGRAM
        static float ANGLE = 45;
        #include "../STMOutline.cginc"
        #pragma surface surf Lambert alpha vertex:vert
        ENDCG

        CGPROGRAM
        static float ANGLE = 135;
        #include "../STMOutline.cginc"
        #pragma surface surf Lambert alpha vertex:vert
        ENDCG

        CGPROGRAM
        static float ANGLE = 225;
        #include "../STMOutline.cginc"
        #pragma surface surf Lambert alpha vertex:vert
        ENDCG

        CGPROGRAM
        static float ANGLE = 315;
        #include "../STMOutline.cginc"
        #pragma surface surf Lambert alpha vertex:vert
        ENDCG
//normal text
        CGPROGRAM
        #include "../STM.cginc"
        #pragma surface surf Lambert alpha
        ENDCG
	}
}