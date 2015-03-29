using UnityEngine;
using System.Collections;

/// <summary>
/// Handles Input
/// Score
/// Respawning
/// </summary>
public class Player : Photon.MonoBehaviour {

	public static Player local;
	
	/// <summary>
	/// Gets the spawn location away from other players.
	/// </summary>
	/// <value>The spawn location.</value>
	private static Vector3 spawnLocation {
		get {
			return new Vector3(5000,1000,5000);
		}
	}
	
	public GameObject shipPrefab;
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="Player"/> is piloting a ship.
	/// </summary>
	/// <value><c>true</c> if spawned; otherwise, <c>false</c>.</value>
	public bool spawned {
		get; private set;
	}
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="Player"/> can spawn.
	/// </summary>
	/// <value><c>true</c> if can spawn; otherwise, <c>false</c>.</value>
	public bool canSpawn {
		get {
			return PhotonNetwork.inRoom && !spawned;
		}
	}
	
	public Transform ship {
		get {
			return _ship.transform;
		}
	}
	
	public Vector3 cameraPosition {
		get {
			return _shipCockpit.cameraPosition;
		}
	}
	
	private GameObject _ship;
	private IPilotable _shipCockpit;
	
	// Use this for initialization
	void Start () {
		if (local==null) {
			local = this;
		}
		else {
			Destroy(this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// respawn the ship
		if (canSpawn) {
			if (Input.GetKeyDown(KeyCode.Space)) {
				_ship = PhotonNetwork.Instantiate(
					shipPrefab.name,
					spawnLocation,
					Quaternion.identity,
					0);
				spawned = true;
				_shipCockpit = _ship.GetComponent(typeof(IPilotable)) as IPilotable;
			}
		}
				
		// if ship is spawned
		if (spawned) {
			// if destroyed, de-spawn
			if (_shipCockpit.destroyed) {
				spawned = false;
				return;
			}
			
			// make drive command
			float dy = Input.GetAxisRaw("Vertical");
			float dx = Input.GetAxisRaw("Horizontal");
			Vector3 drive = new Vector3(dx, 0, dy);
			_shipCockpit.Drive(drive);
			// make steer command
			float sy = Input.GetAxisRaw("Mouse Y");
			float sx = Input.GetAxisRaw("Mouse X");
			Vector3 steer = new Vector3(-sy, sx);
			_shipCockpit.Steer(steer);
		}
		
	}
	
}
