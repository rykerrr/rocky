using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class DarknessEffect : MonoBehaviour
{
    [SerializeField] private float timeTilDarken;

    [SerializeField] private float normVal;
    [SerializeField] private float value;

    private Light globalLight;

    float velocity;

    void Start()
    {
        if (!globalLight && GameObject.Find("_Light"))
        {
            globalLight = GameObject.Find("_Light").GetComponent<Light>();
        }

        if (globalLight)
        {
            normVal = globalLight.intensity;
        }

        value = normVal;
    }

    private void Update()
    {
        globalLight.intensity = Mathf.SmoothDamp(globalLight.intensity, value, ref velocity, timeTilDarken);
    }

    public void DarkenScreen()
    {
        value = 0;
    }

    public void ReturnScreenToNormalLight()
    {
        value = normVal;
    }
}
#pragma warning restore 0649