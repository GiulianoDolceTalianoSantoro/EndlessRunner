using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameState : AState
{
    public Canvas canvas;
    public TrackManager trackManager;

    [Header("UI")]
    public Text scoreText;
    public Text livesText;
    public Text distanceText;

    public RectTransform pauseMenu;
    public RectTransform wholeUI;
    public Button pauseButton;

    public GameObject gameOverPopup;

    protected bool m_Finished;
    protected float m_TimeSinceStart;

    protected RectTransform m_CountdownRectTransform;
    protected bool m_WasMoving;

    protected bool m_GameoverSelectionDone = false;

    protected int k_MaxLives = 3;

    protected bool m_CountObstacles = true;
    protected int m_CurrentSegmentObstacleIndex = 0;
    protected TrackSegment m_NextValidSegment = null;
    protected int k_ObstacleToClear = 3;

    public override void Enter(AState from)
    {
        m_GameoverSelectionDone = false;

        //TestDebug.Debugging(GetName());

        StartGame();
    }

    public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        canvas.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        wholeUI.gameObject.SetActive(true);
        gameOverPopup.SetActive(false);

        if (!trackManager.isRerun)
        {
            m_TimeSinceStart = 0;
        }

        m_Finished = false;

        //TestDebug.Debugging("Start Game");

        StartCoroutine(trackManager.Begin());
    }

    public override string GetName()
    {
        return "Game";
    }

    public override void Tick()
    {
        if (trackManager.isLoaded)
        {
            Player chrCtrl = trackManager.player;

            m_TimeSinceStart += Time.deltaTime;

            if (chrCtrl.currentLife <= 0)
            {
                pauseButton.gameObject.SetActive(false);
                StartCoroutine(WaitForGameOver());
            }

            UpdateUI();
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) Pause();
    }

    void OnApplicationFocus(bool focusStatus)
    {
        if (!focusStatus) Pause();
    }

    public void Pause(bool displayMenu = true)
    {
        //check if we aren't finished OR if we aren't already in pause (as that would mess states)
        if (m_Finished || AudioListener.pause == true)
            return;

        //TestDebug.Debugging("Pause");

        Time.timeScale = 0;

        pauseButton.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(displayMenu);
        wholeUI.gameObject.SetActive(false);
        m_WasMoving = trackManager.isMoving;
        trackManager.StopMove();
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        pauseButton.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        wholeUI.gameObject.SetActive(true);
        if (m_WasMoving)
        {
            trackManager.StartMove(false);
        }

        AudioListener.pause = false;
    }

    public void QuitToLoadout()
    {
        // Used by the pause menu to return immediately to loadout, canceling everything.
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        trackManager.End();
        trackManager.isRerun = false;
        manager.SwitchState("Loadout");
    }

    protected void UpdateUI()
    {
        livesText.text = trackManager.player.currentLife.ToString();

        scoreText.text = trackManager.player.points.ToString();

        distanceText.text = Mathf.FloorToInt(trackManager.worldDistance).ToString() + "m";
    }

    IEnumerator WaitForGameOver()
    {
        m_Finished = true;
        trackManager.StopMove();

        yield return new WaitForSeconds(0.5f);
        GameOver();
    }

    public void OpenGameOverPopup()
    {
        gameOverPopup.SetActive(true);
    }

    public void GameOver()
    {
        manager.SwitchState("GameOver");
    }
}
