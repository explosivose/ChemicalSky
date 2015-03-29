using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(AudioSource))]
public class RecycleEffect : MonoBehaviour {
	
	void OnEnable() {
		StartCoroutine(Lifetime());
	}
	
	IEnumerator Lifetime() {
		float lifetime = Mathf.Max(
			audio.clip.length,
			particleSystem.startLifetime + particleSystem.duration);
		yield return new WaitForSeconds(lifetime + 0.1f);
		transform.Recycle();
	}
}
