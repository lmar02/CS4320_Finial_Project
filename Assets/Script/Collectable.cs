using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{

    public ParticleSystem part;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other)
        {
            Vector3 place = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Instantiate(part,place, transform.rotation);
          
            Destroy(gameObject);
            Score.scoreValue += 100;
        }
            
        
    }
}
