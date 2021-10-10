using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTree : MonoBehaviour
{
    [SerializeField] GameObject groundObj = null;   //GroundObj = ground plane
    public int maxTreeCount = 10;                   //Maximum number of object to instantiate/spawn
    public int rangeInt = 5;                        //Range in which to look for ground for spawning of object
    public float objGroundOffset = 0.5f;            //Offset to ensure that objects are placed on ground

    public LayerMask groundMask;                    //A Layermask for raycast interaction

    Vector3 target = Vector3.zero;                  //A variable we access and edit to find the target to spawn at
    public GameObject[] TreeArray = null;           //An array of objects we use to spawn from
    GameObject ObjHolder;

    public List<MeshFilter> MeshesToCombine = new List<MeshFilter>();

    [SerializeField] Material defaultMat = null;

    Vector3 randomPoint (Vector3 centerPoint)       //A vector 3 variable that we use to find a random point to shoot a ray from. Requires a Vector3 be passed in
    {
        Vector2 targetPoint = Random.insideUnitCircle * rangeInt;       //Create a Vector2 and assign to a random point within our range settings
        Vector3 randomPoint = centerPoint + new Vector3(targetPoint.x, 0, targetPoint.y);       //Create a vector3 and assign it to the center point being passed through. Add to that using the x and y values from our vector2, cast as x and z

        return randomPoint;     //Return our new Vector3 giving us a location to spawn in
    }

    void Start()
    {
        ObjHolder = new GameObject("holder");
        ObjHolder.AddComponent<MeshFilter>();
        MeshRenderer render = ObjHolder.AddComponent<MeshRenderer>();
        render.material = defaultMat;
        for (int i = 0; i < maxTreeCount; i++)         //Create a forloop that runs as long as there are fewer trees than the maximum count
        {
            SpawnTrees();
        }

        CombineMesh();
    }

    void SpawnTrees()
    {
        target = randomPoint(groundObj.transform.position);         //Assign our target variable to be a Random point (the previous Vector3 function), and pass through the ground objects position
        RandomRecast();             //Call the RandomRecast function

        GameObject mySpawnedObj = GameObject.Instantiate(TreeArray[Random.Range(0, TreeArray.Length)], target, transform.rotation);     //Spawn a random object inside our treeArray, at location(target), and assign rotation to be equal to its own

        mySpawnedObj.transform.parent = ObjHolder.transform;
        mySpawnedObj.tag = "Finish";
        MeshesToCombine.Add(mySpawnedObj.GetComponent<MeshFilter>());
    }

    public void CombineMesh()
    {
        Vector3 position = ObjHolder.transform.position;
        ObjHolder.transform.position = Vector3.zero;

        MeshFilter[] meshFilter = MeshesToCombine.ToArray();
        CombineInstance[] combine = new CombineInstance[meshFilter.Length];
        print(meshFilter.Length);
        int i = 0;
        while (i < meshFilter.Length)
        {
            combine[i].mesh = meshFilter[i].sharedMesh;
            combine[i].transform = meshFilter[i].transform.localToWorldMatrix;
            meshFilter[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
            i++;
        }
        ObjHolder.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        ObjHolder.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);
        ObjHolder.transform.gameObject.SetActive(true);

        ObjHolder.transform.position = position;
    }


    void RandomRecast()         //New Function
    {
        Vector3 rayFrom = target + (Vector3.up * 100);      // Create a new Vector3 to send our raycast from this is our target value, plus 100 on the y axis

        RaycastHit hit;         //Create a raycastHit variable (Records a point in which the raycast collides)
        if (Physics.Raycast(rayFrom, Vector3.down * 200, out hit, Mathf.Infinity, layerMask: groundMask))       //Shoot raycast using desired position, Direction + Range, Hit condition, Maximum distance, LayerMask
        {       //if Raycast collides with object containing ground layer:
            target = new Vector3(hit.point.x, hit.point.y + objGroundOffset, hit.point.z);      //Assign our target Vector to the spot where the raycast hits, plus our offset value on the y axis
        }
        else
        {       //if the raycast does NOT collide with an object containing the ground layer,
            target = randomPoint(groundObj.transform.position);     //Find a new Random position to cast from, and pass through the position we want the search centered on
            RandomRecast();     //Go through the process of recasting again
        }
    }
}
