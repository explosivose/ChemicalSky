using UnityEngine;
using System.Collections;

/// <summary>
/// Handles all the high level PUN messages
/// </summary>
public class Multiplayer : Photon.MonoBehaviour {

	public static Multiplayer Instance;
	
	public PhotonLogLevel logLevel;
	
	private bool _master;
	private bool _waitingForEnvSeed;
	
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake() {
		// singleton pattern
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(this);
		}
	}
	
	/// <summary>
	/// Remote message for changing the environment
	/// </summary>
	/// <param name="seed">Seed.</param>
	[RPC]
	void ChangeEnvironment(int seed) {
		Debug.Log("RPCrx: ChangeEnvironment");
		Map.seed = seed;
		Map.Change();
	}
	
	/// <summary>
	/// Event fired when connected to PUN master server
	/// </summary>
	void OnConnectedToMaster() {
		PhotonNetwork.logLevel = logLevel;
	}
	
	/// <summary>
	/// Event fired when joined the PUN lobby
	/// </summary>
	void OnJoinedLobby() {
		// Try joining a random game.
		PhotonNetwork.JoinRandomRoom();
	}
	
	/// <summary>
	/// Event fired when JoinRandomRoom() fails.
	/// </summary>
	void OnPhotonRandomJoinFailed() {
		Debug.Log("Cannot find a random room. Creating one instead.");
		PhotonNetwork.CreateRoom(null);
	}
	
	/// <summary>
	/// Event fired when successfully entered a game
	/// </summary>
	IEnumerator OnJoinedRoom() {
		
		bool timeout = false;
		
		if (PhotonNetwork.isMasterClient) {
			_master = true;
			Map.seed = Random.seed;
			Map.Change();
			ChemicalSkyArena.Instance.Init();
		}
		else {
			_waitingForEnvSeed = true;
			float waitStarted = Time.time;
			while (_waitingForEnvSeed && !timeout) {
				yield return new WaitForSeconds(0.1f);
				if (waitStarted + 10f < Time.time) {
					Debug.LogError("Timed out waiting for environment seed.");
					timeout = true;
				}
			}
		}
		
		_waitingForEnvSeed = false;
	}
	
	/// <summary>
	/// Event fired when a player connects to this room.
	/// </summary>
	/// <param name="player">Player.</param>
	void OnPhotonPlayerConnected(PhotonPlayer player) {
		// if master, send envSeed 
		if (PhotonNetwork.isMasterClient) {
			int seed = Map.seed;
			photonView.RPC("ChangeEnvironment", player, seed);
			Debug.Log("RPCtx: ChangeEnvironment");
		}
	}
	
	/// <summary>
	/// Raises the photon player disconnected event.
	/// </summary>
	/// <param name="player">Player.</param>
	void OnPhotonPlayerDisconnected(PhotonPlayer player) {
		if (PhotonNetwork.isMasterClient && !_master) {
			Debug.LogWarning("Master left the game. I'm the new master!");
		}
	}
}
