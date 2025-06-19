using UnityEngine;

public class S_CameraFollow : MonoBehaviour
{
    public Transform target;       // Le joueur
    public Vector3 offset = new Vector3(0, 30, -10); // Position verticale

    void LateUpdate()
    {
        if (target == null) return;

        // Position fixe au-dessus du joueur
        transform.position = target.position + offset;

        // Vue strictement vers le bas (optionnel si ta caméra est déjà orientée à 90° en X)
        transform.rotation = Quaternion.Euler(70f, 0f, 0f);
    }
}
