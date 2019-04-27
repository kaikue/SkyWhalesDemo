using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleMovement : MonoBehaviour
{
	private const float ACCELERATION = 2.5f;
	private const float DECELERATION = 5f;
	private const float MAX_SPEED = 10f;
	private const float TURN_SENSITIVITY = 0.75f;
	private const float MIN_VERT_ROTATION = -90;
	private const float MAX_VERT_ROTATION = 90;

	private Rigidbody rb;
	private float horizRotation = 0;
	private float vertRotation = 0;
	private float speed = 0;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}
	
	private void FixedUpdate()
	{
		float horizTurn = TURN_SENSITIVITY * Input.GetAxis("Horizontal");
		float vertTurn = TURN_SENSITIVITY * -Input.GetAxis("Vertical");
		horizRotation += horizTurn;
		vertRotation += vertTurn;
		vertRotation = Mathf.Clamp(vertRotation, MIN_VERT_ROTATION, MAX_VERT_ROTATION);
		transform.eulerAngles = new Vector3(vertRotation, horizRotation, 0);

		if (Input.GetButton("Brake"))
		{
			speed = Mathf.Max(speed - DECELERATION * Time.fixedDeltaTime, 0);
		}
		else if (Input.GetButton("Accelerate"))
		{
			speed = Mathf.Min(speed + ACCELERATION * Time.fixedDeltaTime, MAX_SPEED);
		}

		rb.MovePosition(rb.position + GetVel() * Time.fixedDeltaTime);
	}

	private Vector3 GetVel()
	{
		return transform.rotation.normalized * Vector3.forward * speed;
	}

	private void OnCollisionEnter(Collision collision)
	{
		speed -= Vector3.Dot(GetVel(), -collision.contacts[0].normal);
	}
}
