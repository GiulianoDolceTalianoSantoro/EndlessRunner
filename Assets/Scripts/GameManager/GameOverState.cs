using UnityEngine;
using UnityEngine.UI;

public class GameOverState : AState
{
    public TrackManager trackManager;
    public Canvas canvas;
    public Text score;

    public override void Enter(AState from)
    {
        canvas.gameObject.SetActive(true);

        //TestDebug.Debugging(from.GetName());
        score.text = trackManager.score.ToString();
    }

    public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);
    }

    public override string GetName()
    {
        return "GameOver";
    }

    public override void Tick()
    {

    }

    public void GoToLoadout()
    {
        trackManager.isRerun = false;
        manager.SwitchState("Loadout");
    }

    public void RunAgain()
    {
        trackManager.isRerun = false;
        manager.SwitchState("Game");
    }
}
