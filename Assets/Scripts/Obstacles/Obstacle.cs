using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
	public virtual void Setup() { }

	public abstract IEnumerator Spawn(TrackSegment segment, float t);

	public virtual void Impacted()
	{
		//Animation anim = GetComponentInChildren<Animation>();
		//AudioSource audioSource = GetComponent<AudioSource>();

		//if (anim != null)
		//{
		//	anim.Play();
		//}

		//if (audioSource != null && impactedSound != null)
		//{
		//	audioSource.Stop();
		//	audioSource.loop = false;
		//	audioSource.clip = impactedSound;
		//	audioSource.Play();
		//}
	}
}
