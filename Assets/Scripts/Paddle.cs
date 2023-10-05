using UnityEngine;

public class Paddle : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public Vector2 direction { get; private set; }
    public float speed = 50f;
    public float maxBounceAngle = 75f; //you don't want it to be 90 because then it would go flat
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        ResetPaddle();
    }

    public void ResetPaddle()
    {
        rigidbody.velocity = Vector2.zero;
        //here we set the x axis to 0 but we maintain the y axis position
        transform.position = new Vector2(0f, transform.position.y);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            direction = Vector2.left;
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            direction = Vector2.right;
        } else {
            direction = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (direction != Vector2.zero) {
            rigidbody.AddForce(direction * speed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {   
        //if we collided with something other than the ball and let's break out of the function
        if (!collision.gameObject.CompareTag("Ball")) {
            return;
        }

        Rigidbody2D ballRigidbody2D = collision.rigidbody;
        Collider2D paddle = collision.otherCollider;
        
        Vector3 paddlePosition = this.transform.position; 
        //get first point of contact
        Vector2 contactPoint = collision.GetContact(0).point;

        float offset = paddlePosition.x - contactPoint.x;
        float width = paddle.bounds.size.x / 2;

        //we do SignedAngle because we want it to be negative or positive
        float currentAngle = Vector2.SignedAngle(Vector2.up, ballRigidbody2D.velocity);
        float bounceAngle = (offset / width) * this.maxBounceAngle;
        float newAngle = Mathf.Clamp(currentAngle + bounceAngle, -this.maxBounceAngle, this.maxBounceAngle);

        //Quaternion.AngleAxis() lets us form a rotation given an angle and an axis
        //we use Vector3.forward axis because we want the ball to rotate in whatever its forward rotation would be
        Quaternion rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);

        //we want it relative to the up axis because we want the ball to go up
        //Vector2.up is the y axis
        //remember velocity is just direction times speed. We have our direction with rotation * Vector2.up
        //now we need our speed which we get with
            //ballRigidbody2D.velocity.magnitude is the speed extracted from the velocity
        ballRigidbody2D.velocity = rotation * Vector2.up * ballRigidbody2D.velocity.magnitude; 
    }
}
