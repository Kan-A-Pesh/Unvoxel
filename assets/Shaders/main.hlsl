uniform vec2 screenRes;

out vec4 FragColor;

#include "screenPos.hlsl"

void main()
{	
    vec2 screenPos = getScreenPos();
	FragColor = vec4(screenPos, 1.0, 1.0);
}