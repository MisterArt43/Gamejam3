using System;
using UnityEngine;

public enum DamageType { Physical, Energy, Explosive }
public struct DamageInfo
{
    public int damage;
    public DamageType damageType;
} 

public class S_Projectile : MonoBehaviour
{
    public enum ProjectilePattern
    {
        None,
        Spiral,
        Circle,
        Perpendicular,
        Cone
    }

    [Header("Projectile Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 0f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f;
    
    [Space][SerializeField] private DamageType damageType = DamageType.Physical;

    [Space]
    [Header("Bullet Hell Patterns")]
    [Range(0, 100)]
    [SerializeField] private int subProjectileCount = 0;
    [SerializeField] private float setSubProjectileInterval = 0f;
    [SerializeField] private ProjectilePattern projectilePattern = ProjectilePattern.None;
    [SerializeField] private GameObject subProjectilePrefab;
    
    private float nextSpawnTime;
    private Transform cachedTransform;

    void Awake()
    {
        cachedTransform = transform;
        nextSpawnTime = Time.time + setSubProjectileInterval;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        
        // Movement and rotation
        cachedTransform.Translate(speed * deltaTime * Vector3.forward, Space.Self);
        
        if (rotationSpeed > 0f)
        {
            cachedTransform.Rotate(Vector3.up, rotationSpeed * deltaTime);
        }

        // Sub-projectile spawning
        if (subProjectileCount > 0 && subProjectilePrefab != null && setSubProjectileInterval > 0f)
        {
            if (Time.time >= nextSpawnTime)
            {
                SpawnSubProjectiles();
                nextSpawnTime = Time.time + setSubProjectileInterval;
            }
        }
    }

    public void Initialize(int damage, float speed, float rotationSpeed, DamageType damageType)
    {
        this.damage = damage;
        this.speed = speed;
        this.rotationSpeed = rotationSpeed;
        this.damageType = damageType;
    }

    public void Initialize(int damage, float speed, float rotationSpeed, DamageType damageType, 
                          int subProjectileCount, float setSubProjectileInterval, 
                          GameObject subProjectilePrefab, ProjectilePattern projectilePattern)
    {
        Initialize(damage, speed, rotationSpeed, damageType);
        this.subProjectileCount = subProjectileCount;
        this.setSubProjectileInterval = setSubProjectileInterval;
        this.subProjectilePrefab = subProjectilePrefab;
        this.projectilePattern = projectilePattern;
        
        nextSpawnTime = Time.time + setSubProjectileInterval;
    }

    void SpawnSubProjectiles()
    {
        if (subProjectilePrefab == null || subProjectileCount <= 0)
            return;

        Vector3 spawnPosition = cachedTransform.position;
        Quaternion baseRotation = cachedTransform.rotation;
        float angleOffset = 360f / subProjectileCount;
        
        for (int i = 0; i < subProjectileCount; i++)
        {
            Vector3 direction = Vector3.forward;
            Vector3 finalPosition = spawnPosition;
            
            switch (projectilePattern)
            {
                case ProjectilePattern.Spiral:
                    float rad = i * angleOffset * Mathf.Deg2Rad;
                    finalPosition += 0.5f * Mathf.Sin(rad) * cachedTransform.right;
                    finalPosition += 0.5f * Mathf.Cos(rad) * cachedTransform.forward;
                    direction = Quaternion.Euler(0, i * angleOffset, 0) * Vector3.forward;
                    break;
                    
                case ProjectilePattern.Circle:
                    float circleRad = i * angleOffset * Mathf.Deg2Rad;
                    finalPosition += cachedTransform.right * Mathf.Cos(circleRad);
                    finalPosition += cachedTransform.forward * Mathf.Sin(circleRad);
                    direction = new Vector3(Mathf.Cos(circleRad), 0, Mathf.Sin(circleRad));
                    break;
                    
                case ProjectilePattern.Perpendicular:
                    finalPosition += cachedTransform.up;
                    direction = Vector3.up;
                    break;
                    
                case ProjectilePattern.Cone:
                    finalPosition += cachedTransform.forward * 0.5f;
                    float totalAngle = 60f;
                    float coneAngle = subProjectileCount > 1 
                        ? -totalAngle / 2 + (i * totalAngle / (subProjectileCount - 1)) 
                        : 0;
                    direction = baseRotation * Quaternion.Euler(0, coneAngle, 0) * Vector3.forward;
                    break;
            }

            Quaternion spawnRotation = Quaternion.LookRotation(direction);
            GameObject subProjectile = Instantiate(subProjectilePrefab, finalPosition, spawnRotation);
            
            if (subProjectile.TryGetComponent(out S_Projectile subProjectileScript))
            {
                subProjectileScript.Initialize(damage, speed, rotationSpeed, damageType);
            }
        }
    }

    public DamageInfo GetDamageInfo()
    {
        return new DamageInfo
        {
            damage = damage,
            damageType = damageType
        };
    }
}
