using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		TerrainGenerator.Instance.Generate(Random.seed);
	}
	

}
