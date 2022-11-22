using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getjumpedoutsideofmcdonaldsforyourbigmac : MonoBehaviour
{
    [Header("Jump Variables")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] float jumpForce;
    float maxForce = 36;
    float jumpForceBig = 26;
    float timeTillFall = .2f;
    int jumpCount = 0;
    private int jumpTimer = 2;
    int jumpTimerCount = 2;
    [SerializeField] Rigidbody rb;
    [SerializeField] LayerMask groundLayer;
    bool grounded;
    bool jumping = false;

    // Start is called before the first frame update
    void Start()
    {
        rb.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    //use bool to stop the change in physics to occur
    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, 1.75f * 0.5f + 0.2f, groundLayer);

        if (grounded && Input.GetKeyDown(jumpKey) == true && rb.velocity.y <= maxForce && jumpCount != 2)
        {
            jumpCount++;
            jumpTimer = jumpTimerCount;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        else if (grounded && Input.GetKeyDown(jumpKey) == true && rb.velocity.y <= maxForce && jumpCount >= 2 && jumpTimer != 0 )
        {
            jumpCount = 0;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForceBig, ForceMode.Impulse);
        }
        else if (Input.GetKeyUp(jumpKey) == true && (rb.velocity.y < maxForce - 1) && (rb.velocity.y > (maxForce / 4)))
        {
            Invoke(nameof(AddForceToObject), timeTillFall);
        }
    }
    //used for smoother jump-ends
    void AddForceToObject()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * -0.5f, ForceMode.Force);
    }
}
