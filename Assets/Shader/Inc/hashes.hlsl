#ifndef HASHES_INCLUDED
#define HASHES_INCLUDED

uint hash_bit_rotate(uint x, uint k)
{
  return (x << k) | (x >> (32 - k));
}

uint hash(uint kx)
{
  uint a, b, c;
  a = b = c = 0xdeadbeef + (1 << 2) + 13;

  a += kx;

  c ^= b;
  c -= hash_bit_rotate(b, 14);
  a ^= c;
  a -= hash_bit_rotate(c, 11);
  b ^= a;
  b -= hash_bit_rotate(a, 25);
  c ^= b;
  c -= hash_bit_rotate(b, 16);
  a ^= c;
  a -= hash_bit_rotate(c, 4);
  b ^= a;
  b -= hash_bit_rotate(a, 14);
  c ^= b;
  c -= hash_bit_rotate(b, 24);

  return c;
}

uint hash_float(float kx)
{
  return hash(asuint(kx));
}

float uint_to_float_01(uint k)
{
  return float(k) / float(0xFFFFFFFFu);
}

float hash_float_to_float(float k)
{
  return uint_to_float_01(hash_float(k));
}

#endif