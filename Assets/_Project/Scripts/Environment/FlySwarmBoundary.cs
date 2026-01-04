using UnityEngine;

namespace Flat.Environment
{
    [RequireComponent(typeof(ParticleSystem))]
    public class FlySwarmBoundary : MonoBehaviour
    {
        private ParticleSystem ps;
        private ParticleSystem.Particle[] particles;

        [Header("Confinement Bounds (Local Space)")]
        [SerializeField] private Vector3 boundsSize = new Vector3(2f, 2f, 2f);
        [SerializeField] private Vector3 boundsCenter = Vector3.zero;

        [Header("Boundary Behavior")]
        [SerializeField] private float boundaryThickness = 0.3f;
        [SerializeField] private float repulsionForce = 5f;

        private void Start()
        {
            ps = GetComponent<ParticleSystem>();
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
        }

        private void LateUpdate()
        {
            int count = ps.GetParticles(particles);

            Vector3 halfSize = boundsSize * 0.5f;
            Vector3 minBounds = boundsCenter - halfSize;
            Vector3 maxBounds = boundsCenter + halfSize;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = particles[i].position;
                Vector3 vel = particles[i].velocity;
                Vector3 repulsion = Vector3.zero;

                // Apply repulsion force on X axis when near boundaries
                if (pos.x < minBounds.x + boundaryThickness)
                {
                    float penetration = (minBounds.x + boundaryThickness - pos.x) / boundaryThickness;
                    repulsion.x = repulsionForce * penetration;
                }
                else if (pos.x > maxBounds.x - boundaryThickness)
                {
                    float penetration = (pos.x - (maxBounds.x - boundaryThickness)) / boundaryThickness;
                    repulsion.x = -repulsionForce * penetration;
                }

                // Apply repulsion force on Y axis when near boundaries
                if (pos.y < minBounds.y + boundaryThickness)
                {
                    float penetration = (minBounds.y + boundaryThickness - pos.y) / boundaryThickness;
                    repulsion.y = repulsionForce * penetration;
                }
                else if (pos.y > maxBounds.y - boundaryThickness)
                {
                    float penetration = (pos.y - (maxBounds.y - boundaryThickness)) / boundaryThickness;
                    repulsion.y = -repulsionForce * penetration;
                }

                // Apply repulsion force on Z axis when near boundaries
                if (pos.z < minBounds.z + boundaryThickness)
                {
                    float penetration = (minBounds.z + boundaryThickness - pos.z) / boundaryThickness;
                    repulsion.z = repulsionForce * penetration;
                }
                else if (pos.z > maxBounds.z - boundaryThickness)
                {
                    float penetration = (pos.z - (maxBounds.z - boundaryThickness)) / boundaryThickness;
                    repulsion.z = -repulsionForce * penetration;
                }

                // Apply repulsion to velocity
                if (repulsion != Vector3.zero)
                {
                    vel += repulsion * Time.deltaTime;
                    particles[i].velocity = vel;
                }

                // Hard clamp as fallback if particle escapes
                pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
                pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
                pos.z = Mathf.Clamp(pos.z, minBounds.z, maxBounds.z);

                particles[i].position = pos;
            }

            ps.SetParticles(particles, count);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(boundsCenter, boundsSize);
        }
    }
}