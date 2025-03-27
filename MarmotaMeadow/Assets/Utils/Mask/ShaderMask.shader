// https://www.youtube.com/watch?v=Blits1yymCw
Shader "Custom/ShaderMask"
{
     SubShader
  {
	 Tags {"Queue" = "Transparent+1"}	 

  Pass
     {
		 Blend Zero One 
     }
  }
}
