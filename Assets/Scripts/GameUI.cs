using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public Text newWaveTile;
    public Text newWaveEnemyCount;
    public Text scoreUI;
    public RectTransform heathBar;
    public Text gameOverScoreUI;

    Spawner spawner;
    Player player;


	// Use this for initialization
	void Start () {
        player = FindObjectOfType<Player>();
        player.OnDeath += onGameOver;
	}

    void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        float heathPercent = 0;
        if (player != null)
        {
            heathPercent = player.health / player.startingHealth;
            
        }
        heathBar.localScale = new Vector3(heathPercent, 1, 1);

    }

    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += onNewWave;
    }

    void onNewWave(int waveNumber)
    {

        string[] numbers = { "One", "Two", "Three", "Four", "Five" };
        newWaveTile.text = "- Wave " + numbers[waveNumber - 1];
        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "");
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }
	
	void onGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0, 0, 0, 0.9f), 1));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive(false);
        heathBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1f;
            float speed = 2.5f;
            float animatePercent = 0;
        float dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while (animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * dir;

            if (animatePercent >= 1)
            {
                animatePercent = 1;
                if (Time.time > endDelayTime)
                {
                    dir = -1;
                }
            }
            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-346, -200, animatePercent);
            
            yield return null;
        }
        ;
    }

    IEnumerator Fade(Color from,Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    //UI input
    public void StartNewGame()
    {
        SceneManager.LoadScene("Game 01");

        ScoreKeeper.score = 0;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
