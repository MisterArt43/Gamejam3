using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpaceMovement : MonoBehaviour
{


    //-----Mouvement----------
    private InputAction moveAction;

    [Header("Paramètres de mouvement spatial")]
    [SerializeField] private float thrustForce = 10f; // Force de propulsion
    [SerializeField] private float maxSpeed = 100f; // Vitesse maximale

    private float thrustForceSprint; // Force de propulsion sprint

    private InputAction sprintAction;
    
    private Vector2 currentMoveInput;
    public float rotationSpeed = 250f;

    //-----Mouvement----------

    // private InputAction hitAction;
    private Rigidbody rb;



    private void Start()
    {
        thrustForceSprint = thrustForce * 10f;
        // Récupération du Rigidbody (optionnel, selon si vous voulez utiliser la physique Unity)
        rb = GetComponent<Rigidbody>();

        // Configuration pour un mouvement spatial réaliste
        rb.useGravity = false; // Pas de gravité dans l'espace
        rb.linearDamping = 0.1f; // Très peu de résistance
        rb.angularDamping = 0.1f;

        // Références aux actions 
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint"); 
        // hitAction = InputSystem.actions.FindAction("Hit"); // do not touch atm its will be used to hit bullet




        // Abonnement aux événements
        moveAction.performed += OnMoveChanged;
        moveAction.canceled += OnMoveChanged;



    }

    private void OnMoveChanged(InputAction.CallbackContext context)
    {
        currentMoveInput = context.ReadValue<Vector2>();

    }

    void FixedUpdate()
    {
        HandlePhysicsMovement();

        // Utilise seulement l'axe horizontal pour la rotation (A = -1, D = 1)
        float horizontal = currentMoveInput.x;
        transform.Rotate(Vector3.up, horizontal * rotationSpeed * Time.deltaTime);
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
            // Sprint : thurstForce * 10f;
            if (sprintAction.IsPressed())
            {
                Debug.Log("sprint pressed");
                rb.AddForce(worldMoveDirection.normalized * thrustForceSprint, ForceMode.Acceleration);
            }
            else
            {
                rb.AddForce(worldMoveDirection.normalized * thrustForce, ForceMode.Acceleration);
            }
        }

        // Limite la vitesse maximale
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

}
