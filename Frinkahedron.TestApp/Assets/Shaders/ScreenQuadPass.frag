#version 450

layout(location = 0) in vec2 fsin_texCoord;

layout(location = 0) out vec4 fsout_colour;

layout(set = 0, binding = 0) uniform texture2D Texture;
layout(set = 0, binding = 1) uniform sampler TextureSampler;

vec3 applyFXAA(vec2 uv)
{
    vec2 texelSize = 1.0 / textureSize(sampler2D(Texture, TextureSampler), 0);

    vec3 rgbNW = texture(sampler2D(Texture, TextureSampler), uv + vec2(-1.0, -1.0) * texelSize).rgb;
    vec3 rgbNE = texture(sampler2D(Texture, TextureSampler), uv + vec2( 1.0, -1.0) * texelSize).rgb;
    vec3 rgbSW = texture(sampler2D(Texture, TextureSampler), uv + vec2(-1.0,  1.0) * texelSize).rgb;
    vec3 rgbSE = texture(sampler2D(Texture, TextureSampler), uv + vec2( 1.0,  1.0) * texelSize).rgb;
    vec3 rgbM  = texture(sampler2D(Texture, TextureSampler), uv).rgb;

    // Luminance
    vec3 lumaWeights = vec3(0.299, 0.587, 0.114);
    float lumaNW = dot(rgbNW, lumaWeights);
    float lumaNE = dot(rgbNE, lumaWeights);
    float lumaSW = dot(rgbSW, lumaWeights);
    float lumaSE = dot(rgbSE, lumaWeights);
    float lumaM  = dot(rgbM,  lumaWeights);

    float lumaMin = min(lumaM, min(min(lumaNW, lumaNE), min(lumaSW, lumaSE)));
    float lumaMax = max(lumaM, max(max(lumaNW, lumaNE), max(lumaSW, lumaSE)));

    // Edge direction
    vec2 dir;
    dir.x = -((lumaNW + lumaNE) - (lumaSW + lumaSE));
    dir.y =  ((lumaNW + lumaSW) - (lumaNE + lumaSE));

    float dirReduce = max((lumaNW + lumaNE + lumaSW + lumaSE) * 0.25 * 0.0312, 0.0078125);
    float rcpDirMin = 1.0 / (min(abs(dir.x), abs(dir.y)) + dirReduce);

    dir = clamp(dir * rcpDirMin, vec2(-8.0), vec2(8.0)) * texelSize;

    // Sample along edge
    vec3 rgbA = 0.5 * (
        texture(sampler2D(Texture, TextureSampler), uv + dir * (1.0/3.0 - 0.5)).rgb +
        texture(sampler2D(Texture, TextureSampler), uv + dir * (2.0/3.0 - 0.5)).rgb
    );

    vec3 rgbB = rgbA * 0.5 + 0.25 * (
        texture(sampler2D(Texture, TextureSampler), uv + dir * -0.5).rgb +
        texture(sampler2D(Texture, TextureSampler), uv + dir * 0.5).rgb
    );

    float lumaB = dot(rgbB, lumaWeights);

    if (lumaB < lumaMin || lumaB > lumaMax)
        return rgbA;

    return rgbB;
}

vec4 applyMeanBlur(vec2 uv, int radius)
{
    vec2 texelSize = 1.0 / textureSize(sampler2D(Texture, TextureSampler), 0);
	vec4 colour = vec4(0);
	int count = 0;
	for (int i = -radius; i <= radius; i++)
	{
		for (int j = -radius; j <= radius; j++)
		{
			colour += texture(sampler2D(Texture, TextureSampler), fsin_texCoord + vec2(i, j) * texelSize);
			count += 1;
		}
	}
	return colour / count;
}

vec4 applyPixellation(vec2 uv, int radius)
{
    vec2 texSize = textureSize(sampler2D(Texture, TextureSampler), 0);
    vec2 texelSize = 1.0 / texSize;
	vec4 colour = vec4(0);
	int count = 0;

    // Snap to block center
    vec2 pixelCoord = uv * texSize;
    vec2 blockCoord = floor(pixelCoord / float(radius)) * float(radius);
    vec2 baseUV = blockCoord / texSize;

	for (int i = -radius; i <= radius; i++)
	{
		for (int j = -radius; j <= radius; j++)
		{
        vec2 offset = vec2(i, j) * texelSize;
			colour += texture(sampler2D(Texture, TextureSampler), baseUV + offset);
			count += 1;
		}
	}
	return colour / count;
}

vec4 applySepia(vec4 color, float strength)
{
    vec4 sep = vec4(
        dot(color.rgb, vec3(0.393, 0.769, 0.189)),
        dot(color.rgb, vec3(0.349, 0.686, 0.168)),
        dot(color.rgb, vec3(0.272, 0.534, 0.131)),
        1
    );
    return mix(color, sep, strength);
}

vec4 applyGreyscale(vec4 color)
{
    float avg = (color.r + color.g + color.b) / 3;
    return vec4(avg, avg, avg, 1);
}

void main()
{
    fsout_colour = vec4(applyFXAA(fsin_texCoord), 1);
    //fsout_colour = applyPixellation(fsin_texCoord, 4);
}