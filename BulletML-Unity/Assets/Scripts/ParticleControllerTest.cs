using UnityEngine;

public class ParticleControllerTest : MonoBehaviour {

    public ParticleSystem ParticleSystem;

	void Update ()
    {
	    if (Input.GetKey("space"))
        {
            //ParticleSystem.Emit(1);

            var emitParams = new ParticleSystem.EmitParams
            {
                position = Vector2.zero,
                startSize = 0.25f,
                startLifetime = 999999f,
                startColor = Color.red
            };


            ParticleSystem.Emit(emitParams, 1);
        }

        var bulletParticles = new ParticleSystem.Particle[ParticleSystem.particleCount];
        ParticleSystem.GetParticles(bulletParticles);

        for (int i = 0; i < bulletParticles.Length; i++)
        {
            if (i < bulletParticles.Length)
            {
                var newPosition = bulletParticles[i].position;
                newPosition.x += 0.1f;
                bulletParticles[i].position = newPosition;
            }
        }

        if (bulletParticles.Length > 0)
        {
            ParticleSystem.SetParticles(bulletParticles, bulletParticles.Length);
        }
    }
}
