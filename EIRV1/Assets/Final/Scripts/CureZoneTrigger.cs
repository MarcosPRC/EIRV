using UnityEngine;

public class CureZoneTrigger : MonoBehaviour
{
    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("¡El jugador ha encontrado la cura!");
            DiseaseManager.Instance.CurePlayer();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawCube(transform.position, GetComponent<BoxCollider>()?.size ?? Vector3.one);
    }
}