using UnityEngine;
using System.Collections;

public interface IPilotable {

	/// <summary>
	/// Gets a value indicating whether this <see cref="IPilotable"/> is destroyed.
	/// </summary>
	/// <value><c>true</c> if destroyed; otherwise, <c>false</c>.</value>
	bool destroyed {get;}
	
	/// <summary>
	/// Gets a world position for the a chasing camera. 
	/// </summary>
	/// <value>The camera position.</value>
	Vector3 cameraPosition {get;}
	
	/// <summary>
	/// Desired direction to drive in relative to forward.
	/// </summary>
	/// <param name="command">Command.</param>
	void Drive(Vector3 command);
	
	/// <summary>
	/// Desired rotation relative to current rotation. 
	/// </summary>
	/// <param name="command">Command.</param>
	void Steer(Vector3 command);
	
	/// <summary>
	/// Command to fire primary weapon.
	/// </summary>
	void PrimaryFire();
	
	/// <summary>
	/// Command to fire secondary weapon. 
	/// </summary>
	void SecondaryFire();
}
