using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class death : MonoBehaviour {

    Movement gameManager;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<Movement>();
    }
	void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Spikes")
        {
            gameObject.SetActive(false);
            print("Muerte");
            gameManager.isPlayerDead = true;
        }
    }
}
