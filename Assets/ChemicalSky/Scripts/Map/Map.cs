using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the current environment aesthetics.
/// Sets up terrain generator
/// Sets render settings and materials from library of Scene objects
/// Scene objects are located in ChemicalSkyArena gameobject
/// </summary>
public static class Map {

	public static int seed;
	
	public static void Change() {
		TerrainGenerator.Instance.Generate(seed);
	}
}
