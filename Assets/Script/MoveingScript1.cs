using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveingScript1 : MonoBehaviour
{

    private float journeyLength;
    private float startTime;
    public Transform start;
    public Transform end;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(start.position, end.position);
    }

    // Update is called once per frame
    void Update()
    {
        float distCovered = (Time.time - startTime) * 1.0f;

        float fracOfJourn = distCovered / journeyLength;

        transform.position = Vector3.Lerp(start.position, end.position, fracOfJourn);
    }
}
