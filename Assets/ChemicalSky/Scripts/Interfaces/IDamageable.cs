using UnityEngine;
using System.Collections;

public interface IDamageable {

	int health {get;}
	void Damage(int dmg);
	
}
