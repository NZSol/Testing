using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffTower : MonoBehaviour
{
    public List<GameObject> Enemies = new List<GameObject>();

    public float targetRange = 85f;

    public float timer = 0f;
    public float timeTillAttack = 5f;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void DetectEnemies(GameObject enemy)
    {
        Enemies.Add(enemy);
    }
    

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            float Dist = Vector3.Distance(gameObject.transform.position, Enemies[i].transform.position);
            if (Dist < targetRange)
            {
                if (timer > timeTillAttack)
                {
                    timer -= timeTillAttack;
                    DoSlow(i);
                }
            }
        }
    }


    void DoSlow(int i)
    {
        //Enemies[i].GetComponent<Controller>().SpeedDebuff();
    }
}
