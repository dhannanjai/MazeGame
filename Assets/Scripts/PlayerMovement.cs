using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private Rigidbody rb;
    public float speed = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Move(h, v);
    }

    void Move(float _h,float _v)
    {
        Vector3 movement = new Vector3(_h, 0, _v);

        movement = movement.normalized * speed * Time.deltaTime;

        rb.MovePosition(transform.position + movement);
    }
}
