using UnityEngine;

public class Test : MonoBehaviour
{
    public SphereBody sphereBody;
    public SphereBody lead;

    private void OnValidate()
    {
        
        sphereBody.SetVelocityInChain(lead);
        sphereBody.Simulate();
        lead.Simulate();
        sphereBody.UpdatePosition();
        lead.UpdatePosition();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if (sphereBody != null)
        {
            Gizmos.DrawWireSphere(sphereBody.position, sphereBody.radius);
            Gizmos.DrawWireSphere(lead.position, lead.radius);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(sphereBody.position, lead.position);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(sphereBody.position, sphereBody.position + sphereBody.velocity);
            Gizmos.DrawLine(lead.position, lead.position + lead.velocity);
        }
        else
        {
            Debug.LogWarning("SphereBody is not assigned.");
        }
    }

    private void FixedUpdate()
    {
        lead.position = transform.position;
        sphereBody.SetVelocityInChain(lead);
        sphereBody.Simulate();
        lead.Simulate();
        sphereBody.UpdatePosition();
        lead.UpdatePosition();
    }
}
