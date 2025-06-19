#ifndef BSPLINERAMP_INCLUDED
#define BSPLINERAMP_INCLUDED

static const int N = 5;
static const float2 points[N] = {
	float2(0.1, 0.0),
	float2(0.3, 0.0),
	float2(0.35, 1.0),
	float2(0.4, 0.0),
	float2(0.6, 0.0)
};

float evaluateSpline(int toggle, float t)
{
    float a[N], b[N], c[N], d[N];

	int i;

    [unroll]
    for (i = 0; i < N; ++i)
        a[i] = points[i][toggle];

    [unroll]
    for (i = 0; i < N - 1; ++i)
        b[i] = lerp(a[i], a[i + 1], t);

    [unroll]
    for (i = 0; i < N - 2; ++i)
        c[i] = lerp(b[i], b[i + 1], t);

    [unroll]
    for (i = 0; i < N - 3; ++i)
        d[i] = lerp(c[i], c[i + 1], t);

    return lerp(d[0], d[1], t);
}

float findTForX(float x_target)
{
    float t_min = 0.0;
    float t_max = 1.0;
    float t_mid;

	int i;
    for (i = 0; i < 32; ++i) {
        t_mid = 0.5 * (t_min + t_max);
        float x_val = evaluateSpline(0, t_mid);
        if (abs(x_val - x_target) < 1e-4)
            break;
        if (x_val < x_target)
            t_min = t_mid;
        else
            t_max = t_mid;
    }

    return t_mid;
}

float getYFromX(float x_input)
{
    float t = findTForX(x_input);

    return evaluateSpline(1, t);
}

void bsplineramp_float(float fac, out float color)
{
	fac = clamp(fac, points[0].x, points[N - 1].x);

	color = getYFromX(fac);
}

#endif //BSPLINERAMP_INCLUDED