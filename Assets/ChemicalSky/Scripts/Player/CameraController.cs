using UnityEngine;
using System.Collections;

/// <summary>
/// Camera controller controls camera position and orientation. 
/// </summary>
public class CameraController : MonoBehaviour {

	public static CameraController Instance;
	
	public float moveSpeed;
	public float rotateSpeed;
	
	void Awake() {
		// singleton pattern
		if (Instance==null) {
			Instance = this;
		}
		else {
			Destroy(this);
		}
	}
	
	void FixedUpdate() {
		if (Player.local.spawned) {
			transform.position = Vector3.Lerp(
				transform.position,
				Player.local.cameraPosition,
				Time.deltaTime * moveSpeed);
			Vector3 look = Player.local.ship.position;
			look += Player.local.ship.rigidbody.velocity;
			transform.LookAt(look);
		}
	}
}
