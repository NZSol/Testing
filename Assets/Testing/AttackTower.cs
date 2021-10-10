using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTower : MonoBehaviour
{
    public enum TowerType{Target, Area }
    public TowerType Tower;

    public float maxRange = 85f;
    public GameObject target = null;
    public int DamageVal = 5;
    public float timer = 0f;
    public float timeTillAttack = 5f;
    
    public List<GameObject> Enemies = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (timer > timeTillAttack)
        {
            timer -= timeTillAttack;
            switch (Tower)
            {
                case TowerType.Target:
                    DoSingleAttack();
                    break;
                case TowerType.Area:
                    DoAreaAttack();
                    break;
            }
        }
    }

    void DoSingleAttack()
    {
        //target.GetComponent<Controller>().ReduceHealth(DamageVal);
    }

    void DoAreaAttack()
    {
        foreach (GameObject hostile in Enemies)
        {
            float Dist = Vector3.Distance(transform.position, hostile.transform.position);
            if (Dist < maxRange)
            {
                //hostile.GetComponent<Controller>().ReduceHealth(DamageVal);
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.tag == "Hostile" && target == null)
        {
            target = col.gameObject;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.tag == "Hostile")
        {
            target = null;
        }
    }

}
