using UnityEngine;

public class ChainBody : MonoBehaviour
{
    public SphereBody[] chain; // Array of SphereBody objects
    public SphereBody control; // Control SphereBody for binding
    public SphereBody lead; // Lead SphereBody for tethering
    public int controlIndex = 0; // Index of the control SphereBody in the chain


    int chainLength = 0; // Length of the chain

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(lead.position, lead.radius);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(lead.position, lead.position + lead.velocity);
        Gizmos.color = Color.blue;
        for (int i = 0; i < chain.Length; i++)
        {
            if (chain[i] != null)
            {
                Gizmos.DrawWireSphere(chain[i].position, chain[i].radius);
                if (i > 0)
                {
                    Gizmos.DrawLine(chain[i - 1].position, chain[i].position);
                }
            }
        }
        Gizmos.color = Color.yellow;
        if (control != null)
        {
            Gizmos.DrawWireSphere(control.position, control.radius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(control.position, control.innerRadius);
        }
        else
        {
            Debug.LogWarning("Control SphereBody is not assigned.");
        }
    }

    private void OnValidate()
    {
        float lastRad = lead.radius;
        float lastPos = 0f;
        Debug.Log(chain.Length + " chain bodies in the chain.");

        if (chainLength != chain.Length)
        {
            for (int i = 0; i < chain.Length; i++)
            {
                if (chain[i] == null)
                {
                    Debug.LogWarning($"Chain body at index {i} is null.");
                }
                else
                {
                    chain[i].Initialize(transform.position + new Vector3(lastPos + lastRad, 0, 0), .8f, 10, 5, 1);
                    lastRad = chain[i].radius;
                    lastPos += lastRad; // Update the last position for the next SphereBody

                }
            }
            chainLength = chain.Length;
            control.position = chain[controlIndex].position; // Set control position to the first SphereBody in the chain
        }
    }

    private void Start()
    {
        chain[0].next = lead; // Set the first SphereBody's next to lead
        for (int i = 1; i < chain.Length; i++)
        {
            if (chain[i] != null)
            {
                chain[i].next = chain[i - 1]; // Set each SphereBody's next to the previous one
            }
        }
        control.bind = chain[controlIndex]; // Bind control to the specified SphereBody in the chain
    }


    public void FixedUpdate()
    {
        if (chain.Length == 0)
        {
            Debug.LogWarning("No SphereBody objects in the chain to simulate.");
            return;
        }
        SimulateChain();
    }


    public void SimulateChain()
    {
        control.UpdatePosition();
        lead.position = transform.position;
        control.Simulate();
        lead.Simulate();
        lead.UpdatePosition();
        for (int i = 0; i < chain.Length; i++)
        {
            if (chain[i] != null)
            {
                if (i == 0)
                {
                    chain[i].TetherVelocity(lead); // Bind the first SphereBody to the lead
                }
                else
                {
                    chain[i].TetherVelocity(chain[i - 1]); // Bind each SphereBody to the previous one
                }
                chain[i].Simulate();
                chain[i].UpdatePosition();
            }
            else
            {
                Debug.LogWarning($"Chain body at index {i} is null.");
            }
        }
    }
}
