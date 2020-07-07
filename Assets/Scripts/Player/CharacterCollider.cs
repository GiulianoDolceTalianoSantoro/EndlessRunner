using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public class CharacterCollider : MonoBehaviour
{
	static int s_HitHash = Animator.StringToHash("Hit");
	//static int s_BlinkingValueHash;

	// Used mainly by by analytics, but not in an analytics ifdef block 
	// so that the data is available to anything (e.g. could be used for player stat saved locally etc.)
	public struct DeathEvent
	{
		public string character;
		public string obstacleType;
		public string themeUsed;
		public int coins;
		public int premium;
		public int score;
		public float worldDistance;
	}

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
		//if (c.gameObject.layer == k_CoinsLayerIndex)
		//{
		//	if (magnetCoins.Contains(c.gameObject))
		//		magnetCoins.Remove(c.gameObject);

		//	if (c.GetComponent<Coin>().isPremium)
		//	{
		//		Addressables.ReleaseInstance(c.gameObject);
		//		PlayerData.instance.premium += 1;
		//		controller.premium += 1;
		//		m_Audio.PlayOneShot(premiumSound);
		//	}
		//	else
		//	{
		//		Coin.coinPool.Free(c.gameObject);
		//		PlayerData.instance.coins += 1;
		//		controller.coins += 1;
		//		m_Audio.PlayOneShot(coinSound);
		//	}
		//}
		//else 
		if (c.gameObject.CompareTag("Obstacle"))
		{
			controller.currentLife -= 1;
			Debug.Log(controller.currentLife);

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
		//	controller.character.animator.SetTrigger(s_HitHash);

		//	if (controller.currentLife > 0)
		//	{
		//		m_Audio.PlayOneShot(controller.character.hitSound);
		//		SetInvincible();
		//	}
		//	// The collision killed the player, record all data to analytics.
		//	else
		//	{
		//		m_Audio.PlayOneShot(controller.character.deathSound);

		//		m_DeathData.character = controller.character.characterName;
		//		m_DeathData.themeUsed = controller.trackManager.currentTheme.themeName;
		//		m_DeathData.obstacleType = ob.GetType().ToString();
		//		m_DeathData.coins = controller.coins;
		//		m_DeathData.premium = controller.premium;
		//		m_DeathData.score = controller.trackManager.score;
		//		m_DeathData.worldDistance = controller.trackManager.worldDistance;

		//	}
		//}
		//else if (c.gameObject.layer == k_PowerupLayerIndex)
		//{
		//	Consumable consumable = c.GetComponent<Consumable>();
		//	if (consumable != null)
		//	{
		//		controller.UseConsumable(consumable);
		//	}
		//}
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
			//Shader.SetGlobalFloat(s_BlinkingValueHash, currentBlink);

			// We do the check every frame instead of waiting for a full blink period as if the game slow down too much
			// we are sure to at least blink every frame.
			// If blink turns on and off in the span of one frame, we "miss" the blink, resulting in appearing not to blink.
			yield return null;
			time += Time.deltaTime;
			lastBlink += Time.deltaTime;

			if (blinkPeriod < lastBlink)
			{
				lastBlink = 0;
				currentBlink = 1.0f - currentBlink;
			}
		}

		//Shader.SetGlobalFloat(s_BlinkingValueHash, 0.0f);

		_isInvincible = false;
	}
}
