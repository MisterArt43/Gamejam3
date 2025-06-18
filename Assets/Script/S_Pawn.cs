using UnityEngine;

public class S_Pawn : MonoBehaviour
{
    [SerializeField]
    protected int health = 100; // Health of the pawn
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

    void Fire()
    {

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
