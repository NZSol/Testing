using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetection : MonoBehaviour
{

    [SerializeField] SpawnTree spawner = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            spawner.MeshesToCombine.Remove(collision.gameObject.GetComponent<MeshFilter>());
            Destroy(collision.gameObject);
            spawner.CombineMesh();
        }
    }
}
