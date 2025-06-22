using UnityEngine;

public class S_Enemy : S_Pawn
{
    [SerializeField] private float rotationSpeed = 5f; // Speed at which the enemy pawn rotates
    [Space][SerializeField] private float detectionRange = 50f; // Range within which the enemy pawn can detect targets
    enum EnemyState { Idle, Moving, Attacking } // Possible states for the enemy pawn
    private GameObject target; // Target for the enemy pawn, can be a player or another pawn
    private S_Turret[] turrets; // Array to hold the enemy's turrets
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.faction = PawnFaction.Enemy; // Set the faction of the enemy pawn
        InitTurrets(); // Initialize turrets for the enemy pawn
        turrets = GetComponentsInChildren<S_Turret>(); // Get all turrets attached to the enemy pawn
        if (turrets.Length == 0)
        {
            Debug.LogWarning("No turrets found for the enemy pawn. Please ensure turrets are assigned.");
        }
    }

    void IdleMove()
    {
        // randomly move the enemy pawn around
        Vector3 randomDirection = Random.insideUnitSphere * 5f; // Random direction within a sphere of radius 5
        randomDirection.y = 0f; // Keep the movement on the XZ plane
        Vector3 targetPosition = transform.position + randomDirection; // Calculate the target position
        // Move the enemy pawn towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        // Rotate the enemy pawn to face the target position
        if (targetPosition != transform.position)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f; // Ignore height for rotation
            Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void CheckForTarget()
    {
        // Check for nearby targets (e.g., player pawns) within a certain range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            S_Pawn pawn = hitCollider.GetComponent<S_Pawn>();
            if (pawn != null && pawn.getFaction() == PawnFaction.Player)
            {
                target = pawn.gameObject; // Set the target to the player pawn
                break; // Exit the loop once a target is found
            }
        }
    }

    void AttackTarget()
    {
        if (target != null)
        {
            // Implement attack logic here, e.g., firing projectiles from turrets
            foreach (S_Turret turret in turrets)
            {
                // Check if the turret is within range and can fire at the target
                if (turret != null && Vector3.Distance(turret.transform.position, target.transform.position) <= turret.GetRange())
                {
                    // Fire the turret at the target
                    turret.Fire();
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a sphere in the editor to visualize the detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            CheckForTarget(); // Check for targets if none is currently set
            IdleMove(); // Move the enemy pawn randomly when not engaged with a target
        }
        else
        {
            // Rotate towards the target
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0f; // Ignore height for rotation
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }

            // Check if the target is within range to attack
            if (Vector3.Distance(transform.position, target.transform.position) <= detectionRange)
            {
                AttackTarget(); // Attack the target if within range
            }
            else
            {
                target = null; // Clear the target if it's out of range
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (turrets.Length > 0)
        {
            foreach (S_Turret turret in turrets)
            {
                if (turret != null && target != null)
                {
                    // Rotate each turret to face the target
                    Vector3 turretDirection = target.transform.position - turret.transform.position;
                    turretDirection.y = 0f; // Ignore height for turret rotation
                    if (turretDirection != Vector3.zero)
                    {
                        Quaternion turretLookRotation = Quaternion.LookRotation(turretDirection.normalized);
                        turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, turretLookRotation, turret.GetRotationSpeed() * Time.deltaTime);
                    }
                }
            }
        }
    }
}
