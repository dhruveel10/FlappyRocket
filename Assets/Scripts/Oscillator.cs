using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f,10f,10f);
    [SerializeField] float period = 5f;

    [Range(0, 1)] [SerializeField]float movementFactor;
    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(period!=0)
        {
            float cycles = Time.time / period;
            const float tau = Mathf.PI * 2f;
            float rawSine = Mathf.Sin(cycles * tau);
            movementFactor = rawSine / 2f + 0.5f;

            Vector3 offset = movementFactor * movementVector;
            transform.position = startingPos + offset;
        }
        else if(period <= Mathf.Epsilon)
        {
            return; 
        }

    }
}
