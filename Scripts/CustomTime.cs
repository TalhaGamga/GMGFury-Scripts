using System.Collections;
using UnityEngine;

public sealed class CustomTime
{
    private static CustomTime instance;
    public static CustomTime Instance
    {
        get
        {

            if (instance == null)
            {
                instance = new CustomTime();
            }

            return instance;
        }
    }

    private CustomTime()
    {
    }

    private float originalTimeFactor = 1f;
    public float timeFactor = 1f;

    public float deltaTime
    {
        get
        {
            return Time.deltaTime * timeFactor;
        }
    }

    public IEnumerator IETimeSlow(float duration, float factor)
    {
        Debug.Log("IE");
        timeFactor *= factor;
        yield return new WaitForSeconds(duration);
        timeFactor = originalTimeFactor;
    }
}