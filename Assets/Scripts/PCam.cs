using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCam : MonoBehaviour
{
    [Header("Camera Variables")]
    public int _mainCam = 0;
    public Transform pos;
    public float _sensX;
    public float _sensY;
    float _rotaX;
    float _rotaY;


    // Start is called before the first frame update
    void Start()
    {
        if (_mainCam == 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_mainCam == 0)
        {
            //mouse pos
            float mX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * (_sensX * 100);
            float mY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * (_sensY * 100);

            _rotaY += mX;
            _rotaX -= mY;
            _rotaX = Mathf.Clamp(_rotaX, -90f, 90f);

            // rotation
            transform.rotation = Quaternion.Euler(_rotaX, _rotaY, 0);
            pos.rotation = Quaternion.Euler(0, _rotaY, 0);
        }
    }
}
