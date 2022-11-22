using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCamPos : MonoBehaviour
{
    [Header("Camera Variables")]
    public Transform camPos;

    // Update is called once per frame
    void Update()
    {
        transform.position = camPos.position;
    }
}
