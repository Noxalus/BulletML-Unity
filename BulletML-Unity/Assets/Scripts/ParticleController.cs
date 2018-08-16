using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem ParticleSystem;

    ParticleSystem.Particle[] cloud;
    bool bPointsUpdated = false;

    void Start ()
    {
		
	}
	
	void Update ()
    {
        if (bPointsUpdated)
        {
            ParticleSystem.SetParticles(cloud, cloud.Length);
            bPointsUpdated = false;
        }
    }

    public void SetPoints(Vector3[] positions, Color[] colors)
    {
        cloud = new ParticleSystem.Particle[positions.Length];

        for (int ii = 0; ii < positions.Length; ++ii)
        {
            cloud[ii].position = positions[ii];
            cloud[ii].startColor = colors[ii];
            cloud[ii].startSize = 0.1f;
        }

        bPointsUpdated = true;
    }
}
