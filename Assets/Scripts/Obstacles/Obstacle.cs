using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
	public virtual void Setup() { }

	public abstract IEnumerator Spawn(TrackSegment segment, float t);

	public virtual void Impacted()
	{
		// Play sounds or instantiate effects
	}
}
