#ifndef PERLIN_INCLUDED
#define PERLIN_INCLUDED

#include "hashes.hlsl"

float floor_fraction(float x, out int i)
{
	float x_floor = floor(x);
	i = int(x_floor);
	return x - x_floor;
}

float blerp(float v0, float v1, float v2, float v3, float x, float y)
{
  float x1 = 1.0 - x;
  return (1.0 - y) * (v0 * x1 + v1 * x) + y * (v2 * x1 + v3 * x);
}

float tlerp(float v0,
		float v1,
		float v2,
		float v3,
		float v4,
		float v5,
		float v6,
		float v7,
		float x,
		float y,
		float z)
{
	float x1 = 1.0 - x;
	float y1 = 1.0 - y;
	float z1 = 1.0 - z;
	return z1 * (y1 * (v0 * x1 + v1 * x) + y * (v2 * x1 + v3 * x)) +
			z * (y1 * (v4 * x1 + v5 * x) + y * (v6 * x1 + v7 * x));
}

float qlerp(float v0,
			float v1,
			float v2,
			float v3,
			float v4,
			float v5,
			float v6,
			float v7,
			float v8,
			float v9,
			float v10,
			float v11,
			float v12,
			float v13,
			float v14,
			float v15,
			float x,
			float y,
			float z,
			float w)
{
  return lerp(	tlerp(v0, v1, v2, v3, v4, v5, v6, v7, x, y, z),
				tlerp(v8, v9, v10, v11, v12, v13, v14, v15, x, y, z),
				w);
}

float fade(float t)
{
  return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}

float negate_if(float value, uint condition)
{
  return (condition != 0u) ? -value : value;
}

float noise_grad(uint hashe, float x, float y, float z, float w)
{
	uint h = hashe & 31u;
	float u = h < 24u ? x : y;
	float v = h < 16u ? y : z;
	float s = h < 8u ? z : w;
	return negate_if(u, h & 1u) + negate_if(v, h & 2u) + negate_if(s, h & 4u);
}

float perlin_noise4d(float4 position)
{
	int X, Y, Z, W;

	float fx = floor_fraction(position.x, X);
	float fy = floor_fraction(position.y, Y);
	float fz = floor_fraction(position.z, Z);
	float fw = floor_fraction(position.w, W);

	float u = fade(fx);
	float v = fade(fy);
	float t = fade(fz);
	float s = fade(fw);

	float r = qlerp(
		noise_grad(hash4(X, Y, Z, W), fx, fy, fz, fw),
		noise_grad(hash4(X + 1, Y, Z, W), fx - 1.0, fy, fz, fw),
		noise_grad(hash4(X, Y + 1, Z, W), fx, fy - 1.0, fz, fw),
		noise_grad(hash4(X + 1, Y + 1, Z, W), fx - 1.0, fy - 1.0, fz, fw),
		noise_grad(hash4(X, Y, Z + 1, W), fx, fy, fz - 1.0, fw),
		noise_grad(hash4(X + 1, Y, Z + 1, W), fx - 1.0, fy, fz - 1.0, fw),
		noise_grad(hash4(X, Y + 1, Z + 1, W), fx, fy - 1.0, fz - 1.0, fw),
		noise_grad(hash4(X + 1, Y + 1, Z + 1, W), fx - 1.0, fy - 1.0, fz - 1.0, fw),
		noise_grad(hash4(X, Y, Z, W + 1), fx, fy, fz, fw - 1.0),
		noise_grad(hash4(X + 1, Y, Z, W + 1), fx - 1.0, fy, fz, fw - 1.0),
		noise_grad(hash4(X, Y + 1, Z, W + 1), fx, fy - 1.0, fz, fw - 1.0),
		noise_grad(hash4(X + 1, Y + 1, Z, W + 1), fx - 1.0, fy - 1.0, fz, fw - 1.0),
		noise_grad(hash4(X, Y, Z + 1, W + 1), fx, fy, fz - 1.0, fw - 1.0),
		noise_grad(hash4(X + 1, Y, Z + 1, W + 1), fx - 1.0, fy, fz - 1.0, fw - 1.0),
		noise_grad(hash4(X, Y + 1, Z + 1, W + 1), fx, fy - 1.0, fz - 1.0, fw - 1.0),
		noise_grad(hash4(X + 1, Y + 1, Z + 1, W + 1), fx - 1.0, fy - 1.0, fz - 1.0, fw - 1.0),
		u,
		v,
		t,
		s);

	return r;
}

#endif