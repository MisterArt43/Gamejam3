using UnityEngine;

public class S_Turret : MonoBehaviour
{
    [SerializeField] protected int damage = 10; // Damage dealt by the turret
    protected enum DamageType { Physical, Energy, Explosive }
    [SerializeField] protected DamageType damageType = DamageType.Physical; // Type of damage dealt by the turret
    [Space][SerializeField] protected float range = 10f; // Range of the turret
    [Space][SerializeField] protected float fireRate = 1f; // Fire rate of the turret
    [Space][SerializeField] protected float rotationSpeed = 5f; // Speed at which the turret rotates to face targets
    [Space][SerializeField] protected GameObject projectilePrefab; // Prefab of the projectile fired by the turret
    [SerializeField] protected Transform firePoint; // Point from which the turret fires projectiles
    protected float nextFireTime = 0f; // Time when the turret can fire again

    public float GetRotationSpeed()
    {
        return rotationSpeed;
    }

    public float GetRange()
    {
        return range;
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate; // Calculate the next time the turret can fire
            if (projectilePrefab != null && firePoint != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
                // S_Projectile projectileScript = projectile.GetComponent<S_Projectile>();
                // notabene if i want to set the projectile's properties, i can do it here
            }
            else
            {
                Debug.LogWarning("Projectile prefab or fire point is not set for turret: " + gameObject.name);
            }
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
