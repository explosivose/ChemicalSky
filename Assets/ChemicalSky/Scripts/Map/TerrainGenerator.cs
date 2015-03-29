using UnityEngine;
using System.Collections;

/// <summary>
/// Terrain generator uses Perlin noise function to make terrain
/// Clients use y_off and x_off to generate terrain that matches master client
/// </summary>
public class TerrainGenerator : MonoBehaviour {
	
	public static TerrainGenerator Instance;
	
	// game object fields
	public float noise = 0.01f;
	public float bumpiness = 5f;
	
	// properties
	public float y_off {
		get; private set;
	}
	public float x_off {
		get; private set;
	}
	
	// private members
	private TerrainData _terrain;
	private System.Random _random;
	
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
	
	public void Generate(int seed) {
		
		Debug.Log("Generating new terrain...");
		
		// local random object with no shared seed
		_random = new System.Random(seed);
		x_off = (float)_random.NextDouble()*100;
		y_off = (float)_random.NextDouble()*100;
		
		_terrain = Terrain.activeTerrain.terrainData;
		int width = _terrain.heightmapWidth;
		int height = _terrain.heightmapHeight;
		float[,] heightmap = new float[width,height];
		
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				heightmap[x,y] = Mathf.PerlinNoise(
					((float)x+x_off)/(float)width * bumpiness,
					((float)y+y_off)/(float)height * bumpiness
					) + Random.value * noise;
			}
		}
		
		_terrain.SetHeights(0,0,heightmap);
	}
}
