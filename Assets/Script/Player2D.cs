using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2D : MonoBehaviour
{
    public Rigidbody2D rb;
    Vector2 veloc;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        veloc = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * 10;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + veloc * Time.fixedDeltaTime);
    }
}
