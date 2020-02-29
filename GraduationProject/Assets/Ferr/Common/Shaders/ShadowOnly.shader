Shader "Ferr/Common/Shadow Only"
 {
     Subshader
     {
         UsePass "VertexLit/SHADOWCOLLECTOR"    
         UsePass "VertexLit/SHADOWCASTER"
     }
 
     Fallback off
 }