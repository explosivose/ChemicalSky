using UnityEngine;
using System.Collections;

/// <summary>
/// ShipAlpha defines ship kinematics
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ShipAlpha : Photon.MonoBehaviour, IPilotable, IDamageable {

	public int maxHealth;
	public float maxSpeed;
	public float idleSpeed;
	public float turnSpeed;
	
	public Transform explosionEffect;
	
	public int health {
		get; private set;
	}
	
	public bool destroyed {
		get; private set;
	}
	
	public bool canControl {
		get {
			return !_destructing && !destroyed;
		}
	}
	
	public Vector3 cameraPosition {
		get; private set;
	}
	
	public void Drive (Vector3 command) {
		_driveCmd = command.normalized;
	}
	
	public void Steer(Vector3 command) {
		_turnCmd = command;
	}

	/// <summary>
	/// Destroys this ship!
	/// </summary>
	public void Destruction() {
		if (!_destructing) StartCoroutine(DestructionRoutine());
	}

	public void PrimaryFire () {
		
	}

	public void SecondaryFire () {
		
	}

	public void Damage (int dmg) {
		health -= dmg;
	}

	private Transform _camPos;
	private Transform _collider;

	private bool _destructing;

	private Vector3 _driveCmd;
	private Vector3 _turnCmd;
	
	private Vector3 _latestPosUpdate;
	private Vector3 _onUpdatePos;
	private Quaternion _latestRotUpdate;
	private Quaternion _onUpdateRot;
	private float _updateFraction;
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake() {
		
		health = maxHealth;
		_camPos = transform.Find("CameraPosition");
		
		if (photonView.isMine) {
			_collider = transform.Find("Collider");
			_collider.gameObject.SetActive(true);
			rigidbody.isKinematic = false;
		}
		
	}
	
	/// <summary>
	/// Update this instance every frame
	/// </summary>
	void Update() {
		
		if (photonView.isMine) {
			// ship rotation
			if (canControl) {
				// use input from local player
				Vector3 targetv = transform.rotation.eulerAngles + _turnCmd;
				Quaternion target = Quaternion.Euler(targetv);
				transform.rotation = Quaternion.Lerp(
					transform.rotation,
					target,
					Time.deltaTime * turnSpeed);
				cameraPosition = _camPos.position;
			}
		} else {
			// use data from photonView
			
			// We get 10 updates per sec. sometimes a few less or one or two more, depending on variation of lag.
			// Due to that we want to reach the correct position in a little over 100ms. This way, we usually avoid a stop.
			// Lerp() gets a fraction value between 0 and 1. This is how far we went from A to B.
			//
			// Our fraction variable would reach 1 in 100ms if we multiply deltaTime by 10.
			// We want it to take a bit longer, so we multiply with 9 instead.
			_updateFraction += Time.deltaTime * 9;
			transform.localPosition = Vector3.Lerp(
				_onUpdatePos,
				_latestPosUpdate,
				_updateFraction);
			transform.localRotation = Quaternion.Lerp(
				_onUpdateRot,
				_latestRotUpdate,
				_updateFraction);
		}
		
	}
	
	/// <summary>
	/// Fixed update 
	/// </summary>
	void FixedUpdate() {
		if (photonView.isMine) {
			// drive force
			if (canControl) {
				// idle drive force
				Vector3 drive = transform.forward;
				drive *= idleSpeed * rigidbody.mass * rigidbody.drag;
				// add extra drive as per Drive(command)
				drive += transform.TransformDirection(_driveCmd) * (maxSpeed-idleSpeed);
				rigidbody.AddForce(drive);
			}
		}
	}
	
	/// <summary>
	/// Raises the photon serialize view event.
	/// Send and recieve position and rotation
	/// </summary>
	/// <param name="stream">Stream.</param>
	/// <param name="info">Info.</param>
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			// send pos and rot
			Vector3 pos = transform.localPosition;
			Quaternion rot = transform.localRotation;
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
		} else {
			// receive pos and rot
			Vector3 pos = Vector3.zero;
			Quaternion rot = Quaternion.identity;
			stream.Serialize(ref pos);
			stream.Serialize(ref rot);
			_latestPosUpdate = pos;
			_onUpdatePos = transform.localPosition;
			_latestRotUpdate = rot;
			_onUpdateRot = transform.localRotation;
			_updateFraction = 0f;
		}
	}
	
	/// <summary>
	/// Raises the collision enter event.
	/// Should only be raised on Owner because our collider is disabled otherwise.
	/// </summary>
	/// <param name="info">Info on this collision event.</param>
	void OnCollisionEnter(Collision info) {
		Destruction();
	}
	
	IEnumerator DestructionRoutine() {
		_destructing = true;
		health = 0;
		rigidbody.constraints = RigidbodyConstraints.None;
		rigidbody.useGravity = true;
		rigidbody.drag = 0.1f;
		Vector3 explodePos = transform.position + Random.onUnitSphere;
		rigidbody.AddExplosionForce(5f, explodePos, 30f);
		explosionEffect.Spawn(transform.position);
		yield return new WaitForSeconds(2f);
		destroyed = true;
		_destructing = false;
		yield return new WaitForSeconds(30f);
		transform.Recycle();
		explosionEffect.Spawn(transform.position);
	}
}
