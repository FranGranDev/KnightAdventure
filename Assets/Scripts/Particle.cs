using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public GameObject[] Particles;
    public enum ParticlesType {Blood, Sparks};
    public static ParticlesType ParticleType;

    public void CreateParticle(ParticlesType type, Vector3 Postion, Vector2 Direction)
    {
        if(Particles[(int)type] != null)
        {
            GameObject MyParticle = Instantiate(Particles[(int)type], Postion, transform.rotation, null);
            MyParticle.transform.up = Direction;
        }

    }

}
