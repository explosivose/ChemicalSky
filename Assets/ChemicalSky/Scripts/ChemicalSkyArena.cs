using UnityEngine;
using System.Collections;

/// <summary>
/// Chemical sky arena game mode behaviour.
/// </summary>
public class ChemicalSkyArena : MonoBehaviour {

	public static ChemicalSkyArena Instance;
	
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
	
	void Start() {
		PhotonNetwork.ConnectUsingSettings(Strings.gameVersion);
	}
	
	/// <summary>
	/// Local initialise arena game
	/// </summary>
	public void Init() {
		
	}
	
}
