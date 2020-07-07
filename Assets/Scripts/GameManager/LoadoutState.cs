using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class LoadoutState : AState
{
    public Canvas inventoryCanvas;

    protected GameObject m_Character;
    protected const float k_CharacterRotationSpeed = 45f;

    public override void Enter(AState from)
    {
        inventoryCanvas.gameObject.SetActive(true);
    }

    public override void Exit(AState to)
    {
        inventoryCanvas.gameObject.SetActive(false);

        if (m_Character != null) Addressables.ReleaseInstance(m_Character);
    }

    public override string GetName()
    {
        return "Loadout";
    }

    public void StartGame()
    {
        manager.SwitchState("Game");
    }

    public override void Tick()
    {
        if (m_Character != null)
        {
            m_Character.transform.Rotate(0, k_CharacterRotationSpeed * Time.deltaTime, 0, Space.Self);
        }
    }
}
