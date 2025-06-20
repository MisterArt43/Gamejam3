#ifndef FBM4D_INCLUDED
#define FBM4D_INCLUDED

#include "simplex.hlsl"
#include "perlin.hlsl"
#include "hashes.hlsl"

float signed_noise_float(float4 position)
{
	float4 precision_correction = 0.5f * float4(float(abs(position.x) >= 1000000.0f),
												float(abs(position.y) >= 1000000.0f),
												float(abs(position.z) >= 1000000.0f),
												float(abs(position.w) >= 1000000.0f));
	/* Repeat Perlin noise texture every 100000.0f on each axis to prevent floating point
	* representation issues. This causes discontinuities every 100000.0f, however at such scales
	* this usually shouldn't be noticeable. */
	position = fmod(position, 100000.0f) + precision_correction;

	//return SimplexNoise4D(position) * 0.8344f;
	return perlin_noise4d(position) * 0.8344f;
}

float fbm(float4 p, float detail, float roughness, float lacunarity)
{
	float fscale = 1.0f;
	float amp = 1.0f;
	float maxamp = 0.0f;
	float sum = 0.0f;

	for (int i = 0; i <= int(detail); i++)
	{
		float t = signed_noise_float(fscale * p);
		sum += t * amp;
		maxamp += amp;
		amp *= roughness;
		fscale *= lacunarity;
	}

	// Here to implement, maybe: partial detail

	return 0.5f * sum / maxamp + 0.5f;
}

float random_float_offset_float(float seed)
{
  return 100.0f + hash_float_to_float(seed) * 100.0f;
}

float perlin_distortion_float(float4 position, float strength)
{
  return signed_noise_float(position + random_float_offset_float(0.0)) * strength;
}

float fractal_distorded(float4 pos, float detail, float roughness, float lacunarity, float distortion)
{
	pos += perlin_distortion_float(pos, distortion);
	return fbm(pos, detail, roughness, lacunarity);
}

float3 fractal_distorded3(float4 pos, float detail, float roughness, float lacunarity, float distortion)
{
	pos += perlin_distortion_float(pos, distortion);
	return float3(
		fbm(pos, detail, roughness, lacunarity),
		fbm(pos + random_float_offset_float(1.0f), detail, roughness, lacunarity),
		fbm(pos + random_float_offset_float(2.0f), detail, roughness, lacunarity)
	);
}

void fbm4d_float(float3 xyz, float w, float scale, float detail, float roughness, float lacunarity, float distortion, out float fac, out float3 color)
{
	float4 input = float4(xyz, w) * scale;

	fac = fractal_distorded(input,
			clamp(detail, 0.0f, 15.0f),
			max(roughness, 0.0f),
			lacunarity,
			distortion);

	color = fractal_distorded3(input,
		clamp(detail, 0.0f, 15.0f),
		max(roughness, 0.0f),
		lacunarity,
		distortion);
}

#endif