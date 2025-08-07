using UnityEngine;

[System.Serializable]
public class SphereBody 
{
    public Vector3 position;
    public Vector3 velocity;
    public float radius;

    public float springiness;

    Vector3 smoothVelocity;
    Vector3 refVelocity;

    public float smoothTime = 0.1f;

    public float gravScale = 1f;
    float g;
    bool falling;

    public SphereBody(Vector3 position, Vector3 velocity, float radius)
    {
        this.position = position;
        this.velocity = velocity;
        this.radius = radius;
    }

    public void SetVelocityInChain(SphereBody next)
    {
        if (next != null)
        {
            Vector3 direction = next.position - position;
            float distance = direction.magnitude;
            if (distance < radius)
            {
                velocity = direction.normalized * (distance - radius) * springiness * direction.magnitude;
            }else if (distance > radius)
            {
                velocity = direction.normalized * (distance - radius) * springiness * direction.magnitude * direction.magnitude;

            }
            else
            {
                velocity = Vector3.zero; // No movement if exactly at the radius
            }
            if (distance > 5f)
            {
                position = next.position - direction.normalized * radius; // Adjust position to maintain distance
                velocity = Vector3.zero; // Reset velocity if too far apart
            }
        }
    }

    public void Simulate() 
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != null && collider.gameObject.activeInHierarchy)
            {
                Vector3 closestPoint = collider.ClosestPoint(position);
                Vector3 direction = closestPoint - position;
                float distance = direction.magnitude;
                if (distance < radius)
                {
                    Vector3 normal = direction.normalized;
                    float penetrationDepth = radius - distance;
                    Debug.DrawLine(position, closestPoint, Color.red, .1f);
                    position -= normal * penetrationDepth; // Resolve penetration
                    if (closestPoint.y < position.y)
                    {
                        g = 0; // Reset gravity if collision occurs
                    }
                }
            }
        }
        g -= 9.81f * gravScale * Time.fixedDeltaTime; // Apply gravity
        g *= 0.99f; // Dampen gravity effect
        g = Mathf.Clamp(g, -20f, 20f); // Limit gravity to a maximum value
        if (g < -10f)
        {
            falling = true;
        }
        else
        {
            falling = false;
        }
    }
    public void UpdatePosition()
    {
        Vector3 targetVelocity = velocity + Vector3.up * g;
        smoothVelocity = Vector3.SmoothDamp(smoothVelocity, targetVelocity, ref refVelocity, smoothTime);
        position += smoothVelocity * Time.fixedDeltaTime;
    }
}
