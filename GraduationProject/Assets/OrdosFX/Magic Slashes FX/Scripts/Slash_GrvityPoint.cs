using UnityEngine;

[ExecuteInEditMode]
public class Slash_GrvityPoint : MonoBehaviour
{
    public Transform Target;
    public float Force = 1;
    public float StopDistance = 0;
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;

    ParticleSystem.MainModule mainModule;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        mainModule = ps.main;
    }

    void LateUpdate()
    {
        if (Target == null) return;
        var maxParticles = mainModule.maxParticles;
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }
        int particleCount = ps.GetParticles(particles);

        var targetTransformedPosition = Vector3.zero;
        if (mainModule.simulationSpace == ParticleSystemSimulationSpace.Local)
            targetTransformedPosition = transform.InverseTransformPoint(Target.position);
        if (mainModule.simulationSpace == ParticleSystemSimulationSpace.World)
            targetTransformedPosition = Target.position;


        float forceDeltaTime = Time.deltaTime * Force;

        for (int i = 0; i < particleCount; i++)
        {
            var distanceToParticle = targetTransformedPosition - particles[i].position;
           
            if (StopDistance > 0.001f && distanceToParticle.magnitude < StopDistance)
            {
                particles[i].velocity = Vector3.zero;
            }
            else
            {
                var directionToTarget = Vector3.Normalize(distanceToParticle);
                var seekForce = directionToTarget*forceDeltaTime;

                particles[i].velocity += seekForce;
            }
        }

        ps.SetParticles(particles, particleCount);
    }
}
