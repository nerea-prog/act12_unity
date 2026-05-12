using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z); // Mueve la cámara a la posición del objetivo jugador manteniendo la misma posición en el eje Z para no cambiar la distancia de la cámara al jugador
    }
}
