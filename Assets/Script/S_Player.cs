using UnityEngine;
using UnityEngine.InputSystem;

public class S_Player : S_Pawn
{
    private S_Turret[] turrets; // Array to hold the player's turrets
    private InputAction fireAction; // Action for firing with the turrets
    void Start()
    {
        InitTurrets(); // Initialize turrets for the player pawn
        this.faction = PawnFaction.Player; // Set the faction of the player pawn

        turrets = GetComponentsInChildren<S_Turret>(); // Get all turrets attached to the player pawn

        fireAction = InputSystem.actions.FindAction("Attack"); // Find the fire action in the input system

        fireAction.performed += ctx => FireTurrets(); // Subscribe to the fire action

    }

    void FireTurrets()
    {
        foreach (S_Turret turret in turrets)
        {
            if (turret != null)
            {
                turret.Fire(); // Call the Fire method on each turret
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTurretsRotation(); // Update the rotation of the turrets to face the mouse position
    }

    void UpdateTurretsRotation()
    {
        foreach (S_Turret turret in turrets)
        {
            if (turret != null)
            {
                Vector3 mousePos = Mouse.current.position.ReadValue();
                float distanceToTurret = Vector3.Distance(Camera.main.transform.position, turret.transform.position);
                mousePos.z = distanceToTurret;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                Vector3 direction = worldPos - turret.transform.position;

                direction.y = 0f; // On ignore la hauteur pour rester sur le plan XZ

                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);

                    turret.transform.rotation = Quaternion.Slerp(
                        turret.transform.rotation,
                        lookRotation,
                        turret.GetRotationSpeed() * Time.deltaTime
                    );
                }
            }
        }
    }
}
