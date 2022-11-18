using System.Collections;   
using System.Collections.Generic;
using UnityEngine;

public class jumpCondition : MonoBehaviour {

    Movement gameManager;

	// Use this for initialization
	void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<Movement>();
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Floor")
        {
            gameManager.grounded = true;
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Floor")
        {
            gameManager.grounded = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        gameManager.grounded = false;
    }
    

}
