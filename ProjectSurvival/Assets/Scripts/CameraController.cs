using UnityEngine;

public class CameraController : MonoBehaviour
{
	private float _moveSpeed;

	private void Start()
	{
		_moveSpeed = 5f * Time.deltaTime;
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.W))
			transform.Translate(0, 0, _moveSpeed);

		if (Input.GetKey(KeyCode.A))
			transform.Translate(-_moveSpeed, 0, 0);

		if (Input.GetKey(KeyCode.S))
			transform.Translate(0, 0, -_moveSpeed);

		if (Input.GetKey(KeyCode.D))
			transform.Translate(_moveSpeed, 0, 0);

		if (Input.GetMouseButton(1))
			transform.Rotate(0, Input.GetAxis("Mouse X"), 0, Space.World);

		if (Input.GetAxis("Mouse ScrollWheel") < 0)
			transform.Translate(0, _moveSpeed, 0);

		if (Input.GetAxis("Mouse ScrollWheel") > 0)
			transform.Translate(0, -_moveSpeed, 0);
	}
}