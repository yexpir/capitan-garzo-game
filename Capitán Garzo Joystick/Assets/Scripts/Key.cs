using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {
    Movement gameManager;
	// Use this for initialization
	void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<Movement>();
	}
	
	// Update is called once per frame
	void Update () {
        BoxCollider2D playerCollider = gameManager.player.GetComponent<BoxCollider2D>();
        BoxCollider2D keyCollider = gameObject.GetComponent<BoxCollider2D>();
        
        if (playerCollider.bounds.Intersects(keyCollider.bounds) && (gameManager.heavy || !gameManager.G))
        {
            Destroy(gameObject);
            gameManager.keys+=5;
        }
        /*else if (gameManager.range == 0 && gameManager.thin)
        {
            gameManager.reset = false;
            gameManager.thin = false;
            Destroy(gameObject);
            gameManager.keys++;
        }*/
    }
}
