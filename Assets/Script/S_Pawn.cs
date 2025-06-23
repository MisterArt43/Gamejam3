using UnityEngine;

public class S_Pawn : MonoBehaviour
{
    [SerializeField]
    protected float health = 100; // Health of the pawn
    [SerializeField]
    protected float speed = 5f; // Speed of the pawn
    public enum PawnFaction { Player, Neutral, Enemy }
    protected PawnFaction faction = PawnFaction.Neutral; // Faction of the pawn
    [SerializeField]
    protected int damage = 10; // Damage dealt by the pawn
    [SerializeField]
    protected float shield = 5; // Shield of the pawn



    [Space]
    [SerializeField]
    protected GameObject[] turretPrefabs; // Array of turret prefabs that can be placed on the pawn
    [SerializeField]
    protected Transform[] turretLocations; // Array of locations where turrets can be placed

    public PawnFaction getFaction()
    {
        return faction; // Returns the faction of the pawn
    }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        InitTurrets();
    }

    protected void InitTurrets()
    {
        // load the turret prefabs and locations
        if (turretPrefabs == null || turretLocations == null || turretPrefabs.Length != turretLocations.Length)
        {
            Debug.LogError("Turret prefabs and locations must be set and of the same length.");
            return;
        }
        for (int i = 0; i < turretPrefabs.Length; i++)
        {
            if (turretPrefabs[i] == null || turretLocations[i] == null)
            {
                Debug.LogError("Turret prefab or location is not set at index " + i);
                continue;
            }
            // Instantiate the turret prefab at the specified location
            GameObject turret = Instantiate(turretPrefabs[i], turretLocations[i].position, Quaternion.identity);
            turret.transform.SetParent(transform); // Set the pawn as the parent of the turret
        }
    }

    public void TakeDamage(int amount, DamageType damageType = DamageType.Physical)
    {
        // Handle damage taken by the pawn
        if (shield > 0)
        {
            // If the pawn has a shield, reduce the shield first
            shield -= amount;
            if (shield < 0)
            {
                health += (int)shield; // If shield goes below zero, reduce health accordingly
                shield = 0; // Reset shield to zero
            }
        }
        else
        {
            health -= amount; // Reduce health directly if no shield is present
        }

        if (health <= 0)
        {
            Die(); // Call the Die method if health is zero or below
        }
    }

    void Die()
    {
        // Handle the pawn's death
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject); // Destroy the pawn game object
    }
    

    void OnTriggerEnter(Collider other)
    {
        // determine if the other collider is a projectile or another pawn
        if (other == null)
        {
            Debug.LogWarning("OnTriggerEnter called with null collider.");
            return;
        }
        if (other.gameObject == gameObject)
        {
            Debug.LogWarning("OnTriggerEnter called with self collider.");
            return;
        }
        // Check if the other collider is a projectile
        if (other.TryGetComponent<S_Projectile>(out S_Projectile projectile))
        {
            projectile.GetDamageInfo(out int damage, out DamageType damageType);
            // Handle collision with a projectile
            if (damageType == DamageType.Physical)
            {
                TakeDamage(damage); // Deal damage to the pawn
            }
            else if (damageType == DamageType.Energy)
            {
                // Handle energy damage differently, e.g., reduce shield instead of health
                shield -= damage;
                if (shield < 0)
                {
                    health += shield; // If shield goes below zero, reduce health accordingly
                    shield = 0; // Reset shield to zero
                }
            }
            Destroy(projectile.gameObject); // Destroy the projectile after collision
            return;
        }
        // Handle collision with other pawns or projectiles
            S_Pawn otherPawn = other.GetComponent<S_Pawn>();
        if (otherPawn != null && otherPawn.getFaction() != faction)
        {
            // Example: Deal damage to the other pawn
            otherPawn.TakeDamage(damage);
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
