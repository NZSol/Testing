using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatGathering : MonoBehaviour
{
    public float currentMatCount = 0;
    public int matsToAdd = 5;
    public float matAddMultiplier = 1f;

    public float timer = 0f;
    public int timeToReceive = 5;

    public List<GameObject> MatTowers = new List<GameObject>();

    [SerializeField] Text txtField = null;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToReceive)
        {
            timer -= timeToReceive;
            GiveMats();
        }
        txtField.text = currentMatCount.ToString();
    }

    void GiveMats()
    {
        currentMatCount += (matsToAdd * matAddMultiplier) * MatTowers.Count;
    }

    public void AddMatTower(GameObject newTower)
    {
        MatTowers.Add(newTower);
    }
}
