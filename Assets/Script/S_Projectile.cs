using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public enum DamageType { Physical, Energy, Explosive } // Type of damage dealt by the projectile
public struct DamageInfo
        {
            public int damage;
            public DamageType damageType;
        } 

// This script defines a projectile class that is spawned by turrets it will execute a projectile algorithm to match a shape for the bullet hell
public class S_Projectile : MonoBehaviour
{
    public enum ProjectilePattern
    {
        None, // No sub-projectiles
        Spiral, // Spiral pattern for sub-projectiles
        Circle, // Circular pattern for sub-projectiles
        Perpendicular, // Perpendicular pattern for sub-projectiles
        Cone // Cone pattern for sub-projectiles in V-shape
    }

    [Header("Projectile Settings")]
    [Tooltip("Speed of the projectile, how fast it moves in the game world.")]
    [SerializeField] private float speed = 10f; // Speed of the projectile
    [Tooltip("Rotation speed of the projectile, how fast it rotates around its axis.")]
    [SerializeField] private float rotationSpeed = 0f; // Speed at which the projectile rotates, if applicable
    [SerializeField] private int damage = 10; // Damage dealt by the projectile
    [SerializeField] private float lifetime = 5f; // Lifetime of the projectile before it is destroyed
    
    [Space][SerializeField] private DamageType damageType = DamageType.Physical; // Type of damage dealt by the projectile

    //write text in the inspector "this is used for bullet hell patterns"
    [Space]
    [Header("Bullet Hell Patterns")]
    [Tooltip("Amount of sub-projectiles to spawn, used for bullet hell patterns. if set to 0, the subProjectile will not spawn")]
    [Range(0, 100)] // Range for the number of sub-projectiles to spawn
    [SerializeField] private int subProjectileCount = 0; // Amount of sub-projectiles to spawn, used for bullet hell patterns
    [Tooltip("Time interval between projectile spawns, used for bullet hell patterns. if set to 0, the subProjectile will not spawn")]
    [Space][SerializeField] private float setSubProjectileInterval = 0f; // Time interval between projectile spawns, used for bullet hell patterns
    [Tooltip("Prefab of the sub projectile to spawn, used for bullet hell patterns. if set to null, the subProjectile will not spawn")]
    [SerializeField] private ProjectilePattern projectilePattern = ProjectilePattern.None; // Pattern of the projectile, used for bullet hell patterns
    [Tooltip("Prefab of the sub projectile to spawn, used for bullet hell patterns. if set to null, the subProjectile will not spawn")]
    [SerializeField] private GameObject subProjectilePrefab; // Prefab of the sub projectile to spawn
    private float creationTime; // Time when the projectile was created


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        creationTime = Time.time; // Record the time when the projectile is created
        Destroy(gameObject, lifetime); // Destroy the projectile after its lifetime expires
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = transform.forward * speed; // Set the velocity of the projectile
        }
        else
        {
            Debug.LogWarning("Rigidbody component is missing on the projectile. Please add a Rigidbody to the projectile prefab.");
        }
    }

    public void Initialize(int damage, float speed, float rotationSpeed, DamageType damageType)
    {
        this.damage = damage; // Set the damage of the projectile
        this.speed = speed; // Set the speed of the projectile
        this.rotationSpeed = rotationSpeed; // Set the rotation speed of the projectile
        this.damageType = damageType; // Set the type of damage dealt by the projectile
    }
    public void Initialize(int damage, float speed, float rotationSpeed, DamageType damageType, int subProjectileCount, float setSubProjectileInterval, GameObject subProjectilePrefab, ProjectilePattern projectilePattern)
    {
        this.damage = damage; // Set the damage of the projectile
        this.speed = speed; // Set the speed of the projectile
        this.rotationSpeed = rotationSpeed; // Set the rotation speed of the projectile
        this.damageType = damageType; // Set the type of damage dealt by the projectile
        this.subProjectileCount = subProjectileCount; // Set the number of sub-projectiles to spawn
        this.setSubProjectileInterval = setSubProjectileInterval; // Set the time interval between sub-projectile spawns
        this.subProjectilePrefab = subProjectilePrefab; // Set the prefab for sub-projectiles
        this.projectilePattern = projectilePattern; // Set the pattern for sub-projectiles
    }

    void SpawnSubProjectiles()
    {
        if (subProjectilePrefab == null || subProjectileCount <= 0)
        {
            Debug.LogWarning("Sub projectile prefab is not set or sub projectile count is zero. No sub-projectiles will be spawned.");
            return; // Exit if no sub-projectile prefab is set or count is zero
        }

        for (int i = 0; i < subProjectileCount; i++)
        {
            transform.GetPositionAndRotation(out Vector3 spawnPosition, out Quaternion spawnRotation);

            // Calculate the offset based on the projectile pattern and depending the amount of sub-projectiles
            float angleOffset = 360f / subProjectileCount; // Calculate the angle offset for each sub-projectile
            // Adjust the spawn position and rotation based on the projectile pattern
            switch (projectilePattern)
            {
                case ProjectilePattern.Spiral:
                    spawnPosition += 0.5f * Mathf.Sin(i * angleOffset * Mathf.Deg2Rad) * transform.right; // Spiral effect
                    spawnPosition += 0.5f * Mathf.Cos(i * angleOffset * Mathf.Deg2Rad) * transform.forward;
                    break;
                case ProjectilePattern.Circle:
                    spawnPosition += transform.right * Mathf.Cos(i * angleOffset * Mathf.Deg2Rad); // Circular effect
                    spawnPosition += transform.forward * Mathf.Sin(i * angleOffset * Mathf.Deg2Rad);
                    break;
                case ProjectilePattern.Perpendicular:
                    spawnPosition += transform.up; // Perpendicular effect, slightly above the original position
                    break;
                case ProjectilePattern.Cone:
                    spawnPosition += transform.forward * 0.5f; // Cone effect, slightly in front of the original position
                    break;
                default:
                    break;
            }
            switch (projectilePattern)
            {
                case ProjectilePattern.Spiral:
                case ProjectilePattern.Circle:
                    spawnRotation = Quaternion.Euler(0, i * angleOffset, 0) * spawnRotation; // Rotate the sub-projectile based on the angle offset
                    break;
                case ProjectilePattern.Perpendicular:
                    spawnRotation = Quaternion.Euler(90, 0, 0) * spawnRotation; // Rotate the sub-projectile to face upwards
                    break;
                case ProjectilePattern.Cone:
                    spawnRotation = Quaternion.Euler(0, i * angleOffset / 2, 0) * spawnRotation; // Rotate the sub-projectile to create a cone effect
                    break;
                default:
                    break;
            }
            // Instantiate the sub-projectile at the calculated position and rotation
            // Ensure the sub-projectile has a Rigidbody component to apply velocity
            if (subProjectilePrefab.TryGetComponent<Rigidbody>(out var subRb))
            {
                subRb.linearVelocity = transform.forward * speed; // Set the velocity of the sub-projectile
            }
            else
            {
                Debug.LogWarning("Rigidbody component is missing on the sub projectile prefab. Please add a Rigidbody to the sub projectile prefab.");
            }
        }
    }

    void FixedUpdate()
    {
        if (rotationSpeed > 0f)
        {
            // Rotate the projectile around its axis at the specified rotation speed
            transform.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime);
        }

        // Check if the projectile should spawn sub-projectiles
        if (subProjectileCount > 0 && subProjectilePrefab != null && setSubProjectileInterval > 0f)
        {
            float elapsedTime = Time.time - creationTime; // Calculate elapsed time since projectile creation
            if (elapsedTime >= setSubProjectileInterval)
            {
                SpawnSubProjectiles(); // Spawn sub-projectiles based on the defined pattern
                creationTime = Time.time; // Reset the creation time for the next spawn
            }
        }
    }

    public DamageInfo GetDamageInfo()
    {
        return new DamageInfo
        {
            damage = damage,
            damageType = damageType
        }; // Return the damage information of the projectile
    }
    
}
