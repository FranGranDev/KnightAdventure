using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayTime : MonoBehaviour
{
    public float StartTime; //Time in hour; 12 hour per day; 1 hour = 1 minut real time
    public float Speed;
    public static float ClockTime = 5f;
    public static float LightIntensity;
    private Light2D Light;

    void Start()
    {
        StartCoroutine("Timer");
        Light = GetComponent<Light2D>();
        if (ClockTime == 0f)
            ClockTime = StartTime;
    }
    void GetTimeSpeed(float speed)
    {
        Speed = speed;
    }

    IEnumerator Timer()
    {
        while (true)
        {
            if (Speed > 0)
            {
                ClockTime += 1f / 60f;
                if (ClockTime >= 12)
                {
                    ClockTime = 0;
                }
            }
            yield return new WaitForSeconds(1 / Speed);
        }
        
    }

    void FixedUpdate()
    {
        if (Speed > 0)
        {
            LightIntensity = ((Mathf.Sin((ClockTime - 3) * Mathf.PI / 6) + 1) / 2) + 0.1f;
            Light.intensity = LightIntensity;
        }
    }
}
