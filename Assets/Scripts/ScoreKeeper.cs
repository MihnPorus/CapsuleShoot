using UnityEngine;
using System.Collections;

public class ScoreKeeper : MonoBehaviour {

	public static int score { get; set; }

    float lastEnemyKillTime;
    int streakCount;
    float streakExpiryTime = 1;

	void Start () {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += onPlayerDeath;
	}

	void OnEnemyKilled()
    {
        if (Time.time < lastEnemyKillTime + streakExpiryTime)
        {
            streakCount++;
        }
        {
            streakCount = 0;
        }

        lastEnemyKillTime = Time.time;
        score += 5 + (int) Mathf.Pow(2, streakCount);
    }

    void onPlayerDeath()
    {
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }

}
