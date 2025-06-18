using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpaceMovement : MonoBehaviour
{
    private InputAction moveAction;
    private InputAction hitAction;

    [Space][SerializeField] private InputSystem_Actions playerControls;

    [Header("Paramètres de mouvement spatial")]
    [SerializeField] private float thrustForce = 1f; // Force de propulsion
    [SerializeField] private float maxSpeed = 15f; // Vitesse maximale

    private Vector2 currentMoveInput;
    private Vector3 currentVelocity = Vector3.zero; // Vélocité actuelle
    private Rigidbody rb;

    private void Start()
    {
        // Récupération du Rigidbody (optionnel, selon si vous voulez utiliser la physique Unity)
        rb = GetComponent<Rigidbody>();
        // Configuration pour un mouvement spatial réaliste
        rb.useGravity = false; // Pas de gravité dans l'espace
        rb.linearDamping = 0.1f; // Très peu de résistance
        rb.angularDamping = 0.1f;

        // Références aux actions 
        moveAction = InputSystem.actions.FindAction("Move");
        // hitAction = InputSystem.actions.FindAction("Hit"); // do not touch atm its will be used to hit bullet


        // Abonnement aux événements
        moveAction.performed += OnMoveChanged;
        moveAction.canceled += OnMoveChanged;

    }

    private void OnMoveChanged(InputAction.CallbackContext context)
    {
        currentMoveInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
            HandlePhysicsMovement();
    }

    private void HandlePhysicsMovement()
    {
        // Mouvement horizontal
        Vector3 moveDirection = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y);


        // Applique la force relative à la rotation de l'objet
        Vector3 worldMoveDirection = transform.TransformDirection(moveDirection);

        // Applique la force de propulsion
        if (moveDirection.magnitude > 0.1f)
        {
            rb.AddForce(worldMoveDirection.normalized * thrustForce, ForceMode.Acceleration);
        }

        // Limite la vitesse maximale
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }


    // Méthodes utilitaires
    public void AddImpulse(Vector3 impulse)
    {
        rb.AddForce(impulse, ForceMode.Impulse);
    }

    public void StopMovement()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // Pour débugger
    private void OnDrawGizmos()
    {
        // Affiche la direction de la vélocité
        Gizmos.color = Color.red;
        Vector3 velocity =  rb != null ? rb.linearVelocity : currentVelocity;
        Gizmos.DrawRay(transform.position, velocity);
    }

    private void OnDisable()
    {
        // Désabonnement pour éviter les erreurs
        if (moveAction != null)
        {
            moveAction.performed -= OnMoveChanged;
            moveAction.canceled -= OnMoveChanged;
        }

    }
}


 























// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PlayerSpaceMovement : MonoBehaviour
// {
//     private InputAction moveAction;
//     private InputAction hitAction;

//     [Space][SerializeField] private InputSystem_Actions playerControls;

//     [Header("Paramètres de mouvement spatial")]
//     [SerializeField] private float thrustForce = 1f; // Force de propulsion
//     [SerializeField] private float maxSpeed = 15f; // Vitesse maximale
//     [SerializeField] private float drag = 0.98f; // Résistance (0.98 = légère résistance)

//     [Header("Paramètres de rotation")]
//     [SerializeField] private float rotationSpeed = 90f; // Vitesse de rotation
//     [SerializeField] private RotationIntegrationMode rotationMode = RotationIntegrationMode.TankStyle;
//     [SerializeField] private bool smoothRotation = true;
//     [SerializeField] private float rotationSmoothness = 5f;

//     [Header("Contrôles")]
//     [SerializeField] private bool usePhysics = true; // Utiliser la physique ou déplacement direct
//     [SerializeField] private bool allowVerticalMovement = true; // Permettre mouvement vertical

//     public enum RotationIntegrationMode
//     {
//         TankStyle,          // A/D pour tourner, W/S pour avancer/reculer
//         DirectionalMovement, // Le joueur se dirige automatiquement vers la direction du mouvement
//         StrafeWithRotation, // A/D pour tourner, W/S pour avancer, mouvement latéral possible
//         FreeMovement        // Mouvement libre dans toutes les directions + rotation séparée
//     }

//     private Vector2 currentMoveInput;
//     private Vector3 currentVelocity = Vector3.zero;
//     private float currentRotationVelocity = 0f;
//     private Rigidbody rb;

//     private void Start()
//     {
//         rb = GetComponent<Rigidbody>();

//         if (rb != null && usePhysics)
//         {
//             rb.useGravity = false;
//             rb.linearDamping = 0.1f;
//             rb.angularDamping = 0.1f;
//         }

//         moveAction = InputSystem.actions.FindAction("Move");

//         moveAction.performed += OnMoveChanged;
//         moveAction.canceled += OnMoveChanged;
//     }

//     private void OnMoveChanged(InputAction.CallbackContext context)
//     {
//         currentMoveInput = context.ReadValue<Vector2>();
//     }

//     void Update()
//     {
//         if ( rb != null)
//         {
//             HandlePhysicsMovementWithRotation();
//         }
//         else
//         {
//             HandleDirectMovementWithRotation();
//         }
//     }

//     private void HandlePhysicsMovementWithRotation()
//     {
//         switch (rotationMode)
//         {
//             case RotationIntegrationMode.TankStyle:
//                 HandleTankStylePhysics();
//                 break;
//             case RotationIntegrationMode.DirectionalMovement:
//                 HandleDirectionalMovementPhysics();
//                 break;
//             case RotationIntegrationMode.StrafeWithRotation:
//                 HandleStrafeWithRotationPhysics();
//                 break;
//             case RotationIntegrationMode.FreeMovement:
//                 HandleFreeMovementPhysics();
//                 break;
//         }
//     }

//     private void HandleDirectMovementWithRotation()
//     {
//         switch (rotationMode)
//         {
//             case RotationIntegrationMode.TankStyle:
//                 HandleTankStyleDirect();
//                 break;
//             case RotationIntegrationMode.DirectionalMovement:
//                 HandleDirectionalMovementDirect();
//                 break;
//             case RotationIntegrationMode.StrafeWithRotation:
//                 HandleStrafeWithRotationDirect();
//                 break;
//             case RotationIntegrationMode.FreeMovement:
//                 HandleFreeMovementDirect();
//                 break;
//         }
//     }

//     #region Tank Style (comme un char)
//     private void HandleTankStylePhysics()
//     {
//         // A/D pour tourner, W/S pour avancer/reculer
//         float moveForward = currentMoveInput.y; // W/S
//         float turnInput = currentMoveInput.x;   // A/D

//         // Rotation
//         if (Mathf.Abs(turnInput) > 0.1f)
//         {
//             rb.AddTorque(0f, turnInput * rotationSpeed, 0f, ForceMode.Acceleration);
//         }

//         // Mouvement vers l'avant/arrière
//         if (Mathf.Abs(moveForward) > 0.1f)
//         {
//             Vector3 forwardForce = transform.forward * moveForward * thrustForce;
//             rb.AddForce(forwardForce, ForceMode.Acceleration);
//         }

//         // Limite la vitesse
//         if (rb.linearVelocity.magnitude > maxSpeed)
//         {
//             rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
//         }
//     }

//     private void HandleTankStyleDirect()
//     {
//         float moveForward = currentMoveInput.y;
//         float turnInput = currentMoveInput.x;

//         // Rotation
//         if (Mathf.Abs(turnInput) > 0.1f)
//         {
//             currentRotationVelocity += turnInput * rotationSpeed * Time.deltaTime;
//             currentRotationVelocity *= drag; // Applique la résistance à la rotation
//             transform.Rotate(0f, currentRotationVelocity * Time.deltaTime, 0f);
//         }
//         else
//         {
//             currentRotationVelocity *= drag;
//         }

//         // Mouvement
//         if (Mathf.Abs(moveForward) > 0.1f)
//         {
//             Vector3 forwardDirection = transform.forward * moveForward;
//             currentVelocity += forwardDirection * thrustForce * Time.deltaTime;
//         }

//         currentVelocity *= drag;
//         if (currentVelocity.magnitude > maxSpeed)
//         {
//             currentVelocity = currentVelocity.normalized * maxSpeed;
//         }

//         transform.position += currentVelocity * Time.deltaTime;
//     }
//     #endregion

//     #region Directional Movement (rotation automatique vers la direction)
//     private void HandleDirectionalMovementPhysics()
//     {
//         Vector3 moveDirection = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y);

//         if (moveDirection.magnitude > 0.1f)
//         {
//             // Calcul de la direction cible
//             Vector3 targetDirection = moveDirection.normalized;

//             // Rotation fluide vers la direction cible
//             if (smoothRotation)
//             {
//                 Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
//                 transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
//                     rotationSmoothness * Time.deltaTime);
//             }
//             else
//             {
//                 transform.rotation = Quaternion.LookRotation(targetDirection);
//             }

//             // Applique la force vers l'avant (le joueur va toujours vers l'avant une fois orienté)
//             rb.AddForce(transform.forward * thrustForce, ForceMode.Acceleration);
//         }

//         if (rb.linearVelocity.magnitude > maxSpeed)
//         {
//             rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
//         }
//     }

//     private void HandleDirectionalMovementDirect()
//     {
//         Vector3 inputDirection = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y);

//         if (inputDirection.magnitude > 0.1f)
//         {
//             // Rotation vers la direction du mouvement
//             if (smoothRotation)
//             {
//                 Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
//                 transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
//                     rotationSmoothness * Time.deltaTime);
//             }
//             else
//             {
//                 transform.rotation = Quaternion.LookRotation(inputDirection);
//             }

//             // Mouvement vers l'avant
//             currentVelocity += transform.forward * thrustForce * Time.deltaTime;
//         }

//         currentVelocity *= drag;
//         if (currentVelocity.magnitude > maxSpeed)
//         {
//             currentVelocity = currentVelocity.normalized * maxSpeed;
//         }

//         transform.position += currentVelocity * Time.deltaTime;
//     }
//     #endregion

//     #region Strafe with Rotation (mouvement latéral + rotation avec A/D)
//     private void HandleStrafeWithRotationPhysics()
//     {
//         float moveForward = currentMoveInput.y;
//         float moveStrafe = currentMoveInput.x;

//         // Si on maintient Shift (ou autre touche), A/D font tourner
//         bool rotationMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

//         if (rotationMode)
//         {
//             // Mode rotation : A/D pour tourner
//             if (Mathf.Abs(moveStrafe) > 0.1f)
//             {
//                 rb.AddTorque(0f, moveStrafe * rotationSpeed, 0f, ForceMode.Acceleration);
//             }

//             // W/S pour avancer/reculer
//             if (Mathf.Abs(moveForward) > 0.1f)
//             {
//                 Vector3 forwardForce = transform.forward * moveForward * thrustForce;
//                 rb.AddForce(forwardForce, ForceMode.Acceleration);
//             }
//         }
//         else
//         {
//             // Mode mouvement normal : mouvement dans toutes les directions
//             Vector3 moveDirection = new Vector3(moveStrafe, 0f, moveForward);
//             Vector3 worldMoveDirection = transform.TransformDirection(moveDirection);

//             if (moveDirection.magnitude > 0.1f)
//             {
//                 rb.AddForce(worldMoveDirection.normalized * thrustForce, ForceMode.Acceleration);
//             }
//         }

//         if (rb.linearVelocity.magnitude > maxSpeed)
//         {
//             rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
//         }
//     }

//     private void HandleStrafeWithRotationDirect()
//     {
//         float moveForward = currentMoveInput.y;
//         float moveStrafe = currentMoveInput.x;

//         bool rotationMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

//         if (rotationMode)
//         {
//             // Rotation avec A/D
//             if (Mathf.Abs(moveStrafe) > 0.1f)
//             {
//                 transform.Rotate(0f, moveStrafe * rotationSpeed * Time.deltaTime, 0f);
//             }

//             // Mouvement avec W/S
//             if (Mathf.Abs(moveForward) > 0.1f)
//             {
//                 currentVelocity += transform.forward * moveForward * thrustForce * Time.deltaTime;
//             }
//         }
//         else
//         {
//             // Mouvement libre
//             Vector3 inputDirection = new Vector3(moveStrafe, 0f, moveForward);
//             if (inputDirection.magnitude > 0.1f)
//             {
//                 Vector3 worldDirection = transform.TransformDirection(inputDirection.normalized);
//                 currentVelocity += worldDirection * thrustForce * Time.deltaTime;
//             }
//         }

//         currentVelocity *= drag;
//         if (currentVelocity.magnitude > maxSpeed)
//         {
//             currentVelocity = currentVelocity.normalized * maxSpeed;
//         }

//         transform.position += currentVelocity * Time.deltaTime;
//     }
//     #endregion

//     #region Free Movement (mouvement libre + rotation séparée avec Q/E)
//     private void HandleFreeMovementPhysics()
//     {
//         // Mouvement libre dans toutes les directions
//         Vector3 moveDirection = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y);
//         Vector3 worldMoveDirection = transform.TransformDirection(moveDirection);

//         if (moveDirection.magnitude > 0.1f)
//         {
//             rb.AddForce(worldMoveDirection.normalized * thrustForce, ForceMode.Acceleration);
//         }

//         // Rotation séparée avec Q/E
//         float rotationInput = 0f;
//         if (Input.GetKey(KeyCode.Q))
//             rotationInput = -1f;
//         else if (Input.GetKey(KeyCode.E))
//             rotationInput = 1f;

//         if (Mathf.Abs(rotationInput) > 0.1f)
//         {
//             rb.AddTorque(0f, rotationInput * rotationSpeed, 0f, ForceMode.Acceleration);
//         }

//         if (rb.linearVelocity.magnitude > maxSpeed)
//         {
//             rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
//         }
//     }

//     private void HandleFreeMovementDirect()
//     {
//         // Mouvement libre
//         Vector3 inputDirection = new Vector3(currentMoveInput.x, 0f, currentMoveInput.y);

//         if (inputDirection.magnitude > 0.1f)
//         {
//             Vector3 worldDirection = transform.TransformDirection(inputDirection.normalized);
//             currentVelocity += worldDirection * thrustForce * Time.deltaTime;
//         }

//         // Rotation avec Q/E
//         float rotationInput = 0f;
//         if (Input.GetKey(KeyCode.Q))
//             rotationInput = -1f;
//         else if (Input.GetKey(KeyCode.E))
//             rotationInput = 1f;

//         if (Mathf.Abs(rotationInput) > 0.1f)
//         {
//             transform.Rotate(0f, rotationInput * rotationSpeed * Time.deltaTime, 0f);
//         }

//         currentVelocity *= drag;
//         if (currentVelocity.magnitude > maxSpeed)
//         {
//             currentVelocity = currentVelocity.normalized * maxSpeed;
//         }

//         transform.position += currentVelocity * Time.deltaTime;
//     }
//     #endregion

//     // Méthodes utilitaires existantes
//     public void AddImpulse(Vector3 impulse)
//     {
//         if ( rb != null)
//         {
//             rb.AddForce(impulse, ForceMode.Impulse);
//         }
//         else
//         {
//             currentVelocity += impulse;
//         }
//     }

//     public void StopMovement()
//     {
//         if ( rb != null)
//         {
//             rb.linearVelocity = Vector3.zero;
//             rb.angularVelocity = Vector3.zero;
//         }
//         else
//         {
//             currentVelocity = Vector3.zero;
//             currentRotationVelocity = 0f;
//         }
//     }

//     private void OnDrawGizmos()
//     {
//         Gizmos.color = Color.red;
//         Vector3 velocity =  rb != null ? rb.linearVelocity : currentVelocity;
//         Gizmos.DrawRay(transform.position, velocity);

//         // Affiche la direction avant
//         Gizmos.color = Color.blue;
//         Gizmos.DrawRay(transform.position, transform.forward * 2f);
//     }

//     private void OnDisable()
//     {
//         if (moveAction != null)
//         {
//             moveAction.performed -= OnMoveChanged;
//             moveAction.canceled -= OnMoveChanged;
//         }
//     }
// }