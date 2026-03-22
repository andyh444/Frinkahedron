#version 450

layout(location = 0) in vec4 fsin_position;

void main()
{
	// manually set the frag depth to ensure it's in the same space for the shadow checking in the main pass
	// TODO Investigate why this is necessary and remove if possible
	float depth = fsin_position.z / fsin_position.w;
	depth = depth * 0.5 + 0.5;
	gl_FragDepth = depth;
}