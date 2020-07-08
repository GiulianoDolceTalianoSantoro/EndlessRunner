using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public class CharacterCollider : MonoBehaviour
{
	public Player controller;

	public new BoxCollider collider { get { return _Collider; } }


	protected bool _isInvincible;
	protected BoxCollider _Collider;

	protected float _StartingColliderHeight;

	protected const float k_DefaultInvinsibleTime = 2f;

	protected void Start()
	{
		_Collider = GetComponent<BoxCollider>();
		_StartingColliderHeight = _Collider.bounds.size.y;
	}

	public void Init()
	{
		_isInvincible = false;
	}
		
	// Manage all collisions with obstacles and collectables
	protected void OnTriggerEnter(Collider c)
	{
        if (c.gameObject.CompareTag("Coin"))
        {
			c.gameObject.SetActive(false);

			controller.points += 1;
        }
        else if (c.gameObject.CompareTag("Obstacle"))
		{
			controller.currentLife -= 1;

			if (controller.currentLife > 0 || controller.IsCheatInvincible())
				return;

			controller.StopMoving();

			c.enabled = false;

			Obstacle ob = c.gameObject.GetComponent<Obstacle>();

			if (ob != null)
			{
				ob.Impacted();
			}
			else
			{
				Addressables.ReleaseInstance(c.gameObject);
			}
		}
	}

	public void SetInvincibleExplicit(bool invincible)
	{
		_isInvincible = invincible;
	}

	public void SetInvincible(float timer = k_DefaultInvinsibleTime)
	{
		StartCoroutine(InvincibleTimer(timer));
	}

	protected IEnumerator InvincibleTimer(float timer)
	{
		_isInvincible = true;

		float time = 0;
		float currentBlink = 1.0f;
		float lastBlink = 0.0f;
		const float blinkPeriod = 0.1f;

		while (time < timer && _isInvincible)
		{
			yield return null;
			time += Time.deltaTime;
			lastBlink += Time.deltaTime;

			if (blinkPeriod < lastBlink)
			{
				lastBlink = 0;
				currentBlink = 1.0f - currentBlink;
			}
		}

		_isInvincible = false;
	}
}
