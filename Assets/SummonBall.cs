using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonBall : MonoBehaviour
{
    public GameObject prefab;
    public int StartSpawn = 20;
    private int ballsSpawned;
    public List<GameObject> balls = new List<GameObject>();
    public int BallsSpawned => balls.Count;
    bool awaitingFirstSummon = true;

    
    void Start()
    {
        ballsSpawned = 0;
        summonBall();
    }

    private void summonBall() 
    {
        if (ballsSpawned < StartSpawn) 
        {
            Summon();
            awaitingFirstSummon = false;
            ballsSpawned++;
            Invoke("summonBall", Random.Range(0f,2f));
        }
    }

    private void StartSummon() 
    {
        if (!awaitingFirstSummon) 
        {
            summonBall();
            awaitingFirstSummon = true;
        }
    }

    public void Reset() 
    {
        foreach (GameObject obj in balls) 
        {
            if(obj != null)
                Destroy(obj);
        }
        balls.Clear();
        ballsSpawned = 0;
        StartSummon();
    }

    public void Summon() 
    {
        balls.Add(Instantiate(prefab, this.transform.position + 
            new Vector3(Random.Range(-0.5f, 0.5f),0, Random.Range(-0.5f, 0.5f)),
            Quaternion.identity));
    }
}
