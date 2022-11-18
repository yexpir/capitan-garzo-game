using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hookSpawn : MonoBehaviour {

    Movement gameManager;
    GameObject[] obj;
    // Use this for initialization
    void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<Movement>();
        obj = GameObject.FindGameObjectsWithTag("Floor");
	}
    void Update()
    {
        Collider2D hookSpawnCollider = gameObject.GetComponent<BoxCollider2D>();
        for (int i = 0; i < obj.Length; i++)
        {
            Collider2D objCollider = obj[i].GetComponent<Collider2D>();
            if (hookSpawnCollider.bounds.Intersects(objCollider.bounds))
            {
                gameManager.hookWall = true;
            }
        }
    }
    void OnCollisionExit2D(Collision2D coll)
    {
        gameManager.hookWall = false;
    }
}
