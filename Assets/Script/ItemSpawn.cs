using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{

    public GameObject spawnedItems;
    private float xAxis;
    private float zAxis;
    private Vector3 positions;




    // Start is called before the first frame update
    void Start()
    {

        for(int i = 0; i < 30; i++)
        {

            SpawnItems();
         

        }
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //this method checks for objects in a given Vecotr3
    //then it instantiates a prefab

    void SpawnItems()
    {
        xAxis = Random.Range(45, -45);
        zAxis = Random.Range(-5, -95);
        positions = new Vector3(xAxis, -3.39f, zAxis);
        Collider[] hitCollider = Physics.OverlapSphere(positions, 0.1f);
        if (hitCollider.Length > 0)
        {
            SpawnItems();
        }
        else
        {
            Instantiate(spawnedItems, positions, spawnedItems.transform.rotation);

        }
    }
}
