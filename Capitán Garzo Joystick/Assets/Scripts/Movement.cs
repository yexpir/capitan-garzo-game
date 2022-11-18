using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Movement : MonoBehaviour {

    bool isGameRunning = false;
    public Canvas menu;
    public Text links;
    public Canvas health;
    public Text hpAmount;

    public GameObject cam;
    Vector3 camFollow;
    public float camSpeed = 10;


    public GameObject player;
    BoxCollider2D playerCollider;
    Vector3 pos;
    Quaternion rot;
    public bool G = false;
    public float speed = 5f;

    //Variables muerte y respawn - checkpoints
    public bool isPlayerDead = false;
    public GameObject[] checkPoints;
    public int respawnSet = 0;
    public float respawnCd = 1;

    //Variables vida, daño, victoria y derrota
    public GameObject[] spikes;
    public GameObject victoryDoor;
    public int hp = 5;
    public Canvas victory;
    public Canvas defeat;
    bool replay = true;

    //Variables de salto
    public GameObject feet;
    public GameObject[] floor;
    public float vel = 15;
    public float fallM = 5f;
    public float lowM = 3f;
    public bool grounded = false;

    //Variables de gancho
    public GameObject gancho;
    public GameObject chainPrefab;
    GameObject[] chain;
    public Transform chainSpawnPoint;
    public Transform firstChainSpawnPoint;
    Vector3 gPos;
    Quaternion gRot;
    public int range = 0;
    public bool reset = true;
    bool jumpChance = false;
    public bool hookWall = false;
    Vector3 auxPos;

    //Sistema Retractil (SR)
    public bool heavy = false;
    public bool thin = false;
    Vector3 gHoldPos;
    Quaternion gHoldRot;
    GameObject[] still;
    Transform lastLinkPosition;

    //Variables key

    /*public GameObject keyPrefab;
    public Transform keySpawnPoint;
    public GameObject playerKey;*/
    int keyAuxIndex = 0;
    GameObject[] keyClones;
    public int keys = 0;

    //Variables joystick de la muerte
    float[] axis = new float[4];
    bool axisDown = false;
    bool axisUp = false;
    bool axisRead = false;

    public float hookSpeed;
    public float hookTimer;

    // Use this for initialization
    void Start () {
        floor = GameObject.FindGameObjectsWithTag("Floor");
        gancho.SetActive(false);
        //Instantiate(keyPrefab, new Vector3(Random.Range(-39, 39), Random.Range(-14, 20), 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        playerCollider = player.GetComponent<BoxCollider2D>();
        if (!isGameRunning)
        {
            menu.gameObject.SetActive(true);
            links.gameObject.SetActive(false);
            health.gameObject.SetActive(false);
        }
        if (!replay)
        {
            defeat.gameObject.SetActive(true);
        }
        if (isGameRunning && replay)
        {
            menu.gameObject.SetActive(false);
            links.gameObject.SetActive(true);
            health.gameObject.SetActive(true);

            //Set respawn
            for (int i = 0; i < checkPoints.Length; i++)
            {
                BoxCollider2D checkPointCollider = checkPoints[i].GetComponent<BoxCollider2D>();
                if (playerCollider.bounds.Intersects(checkPointCollider.bounds))
                {
                    respawnSet = i;
                    print("respawn setted " + respawnSet);
                }
            }

            //MUERTO!
            if (isPlayerDead)
            {
                respawnCd -= Time.deltaTime;
                if (respawnCd <= 0)
                {
                    hp--;
                    isPlayerDead = false;
                    respawnCd = 0.5f;
                    player.SetActive(true);
                    player.transform.position = checkPoints[respawnSet].transform.position;
                }
            }
            //DERROTA
            if(hp == 0)
            {
                defeat.gameObject.SetActive(true);
                replay = false;
                respawnSet = 0;
            }
            BoxCollider2D victoryDoorCollider = victoryDoor.GetComponent<BoxCollider2D>();
            if (playerCollider.bounds.Intersects(victoryDoorCollider.bounds))
            {
                victory.gameObject.SetActive(true);
            }

            if (Input.GetButton("Esc"))
            {
                isGameRunning = false;
            }


            //VIVO
            pos = Vector3.zero;
            if (!isPlayerDead)
            {
                if (!G)
                {
                    camFollow = new Vector3(player.transform.position.x, player.transform.position.y, -20);
                    cam.transform.position = Vector3.Lerp(cam.transform.position, camFollow, camSpeed * Time.deltaTime);
                    //MOVIMIENTO
                    /*if (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal"))
                    {
                        rot.y = 180;
                        pos = player.transform.right;
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        rot.y = 0;
                        pos = player.transform.right;
                    }*/
                    
                    pos = new Vector3(Input.GetAxis("Horizontal"), 0, 0);


                    if(Input.GetAxis("Horizontal") < 0)
                    {
                        rot.y = 180;
                    }
                    else if (Input.GetAxis("Horizontal") > 0)
                    {
                        rot.y = 0;
                    }
                    player.transform.rotation = rot;
                    player.transform.position += pos * speed * Time.deltaTime;

                    
                    if (Input.GetButtonDown("Jump") && (grounded || jumpChance))
                    {
                        player.GetComponent<Rigidbody2D>().velocity = Vector2.up * vel;
                        jumpChance = false;
                    }
                    if (player.GetComponent<Rigidbody2D>().velocity.y < 0)
                    {
                        player.GetComponent<Rigidbody2D>().velocity += Vector2.up * Physics2D.gravity.y * (fallM) * Time.deltaTime;
                    }
                    else if (player.GetComponent<Rigidbody2D>().velocity.y > 0 && !Input.GetButton("Jump"))
                    {
                        player.GetComponent<Rigidbody2D>().velocity += Vector2.up * Physics2D.gravity.y * (lowM) * Time.deltaTime;
                    }

                    //Direccion de la cadena según adónde esté mirando el personaje
                    if (rot.y == 180)
                    {
                        gPos.x = -0.833f;
                        gPos.y = 0;
                        gRot = Quaternion.Euler(0, 0, 90);
                    }
                    if (rot.y == 0)
                    {
                        gPos.x = 0.833f;
                        gPos.y = 0;
                        gRot = Quaternion.Euler(0, 0, -90);
                    }
                    
                }


                //---------------------------------------------------------------------------------------------------------------------------\\
                //CADENA
                if (Input.GetButtonDown("Hook"))
                {
                    reset = true;
                }
                if (Input.GetButton("Hook") && reset == true)
                {
                    gancho.SetActive(true);
                    axisRead = true;
                    G = true;
                    if(Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(Input.GetAxis("Vertical")))
                    {
                        if(Input.GetAxis("Horizontal") > 0 && gRot != Quaternion.Euler(0, 0, 90))
                        {
                            gPos.x = 0.833f;
                            gPos.y = 0;
                            gRot = Quaternion.Euler(0, 0, -90);
                        }
                        else if(Input.GetAxis("Horizontal") < 0 && gRot != Quaternion.Euler(0, 0, -90))
                        {
                            gPos.x = -0.833f;
                            gPos.y = 0;
                            gRot = Quaternion.Euler(0, 0, 90);
                        }
                    }
                    if (Mathf.Abs(Input.GetAxis("Vertical")) > Mathf.Abs(Input.GetAxis("Horizontal")))
                    {
                        if (Input.GetAxis("Vertical") > 0 && gRot != Quaternion.Euler(0, 0, 180))
                        {
                            gPos.y = 0.833f;
                            gPos.x = 0;
                            gRot = Quaternion.Euler(0, 0, 0);
                        }
                        else if (Input.GetAxis("Vertical") < 0 && gRot != Quaternion.Euler(0, 0, 0))
                        {
                            gPos.y = -0.833f;
                            gPos.x = 0;
                            gRot = Quaternion.Euler(0, 0, 180);
                        }
                    }

                    if (!heavy)
                    {
                        camFollow = new Vector3(gancho.transform.position.x, gancho.transform.position.y, -20);
                        cam.transform.position = Vector3.Lerp(cam.transform.position, camFollow, camSpeed * Time.deltaTime);
                    }
                    if (!heavy && !thin && !hookWall)
                    {
                        if (hookTimer < hookSpeed)
                        {
                            hookTimer += Time.deltaTime;
                            return;
                        }
                        hookTimer = 0f;
                        
                        chainSpawnPoint.transform.rotation = gRot;
                        Instantiate(chainPrefab, chainSpawnPoint.position, chainSpawnPoint.transform.rotation);
                        chainSpawnPoint.transform.position += gPos;
                        if(range == 0)
                        {
                            auxPos = player.transform.position;
                        }
                        player.transform.position = auxPos;
                        player.GetComponent<Rigidbody2D>().velocity *= 0;
                        range++;
                    }
                }
                else
                {
                    G = false;
                    reset = false;
                    gancho.SetActive(false);
                }
                chain = GameObject.FindGameObjectsWithTag("Chain");

                //Rango y reset
                if ((Input.GetButtonUp("Hook") && !thin) || range > (20 + keys) || hookWall)
                {
                    G = false;
                    reset = false;
                    gancho.SetActive(false);
                    hookWall = false;
                    heavy = false;
                    thin = false;
                    for (int i = 0; i < chain.Length; i++)
                    {
                        Destroy(chain[i]);
                    }
                    range = 0;
                }

                
                //---------------------------------------------------------------------------------------------------------------------------\\
                //SISTEMA RETRÁCTIL
                BoxCollider2D ganchoCollider = gancho.GetComponent<BoxCollider2D>();
                still = GameObject.FindGameObjectsWithTag("Still");
                CircleCollider2D[] stillCollider = new CircleCollider2D[still.Length];



                //Si el gancho agarra algo fijo
                if (axisRead)
                {
                    axis[1] = Input.GetAxisRaw("Grab");
                    axis[3] = Input.GetAxisRaw("Grab");
                    if(axis[1] < axis[0])
                    {
                        axisDown = true;
                        print("down " + axisDown);
                    }
                    else if(axis[0] == axis[1])
                    {
                        axisDown = false;
                        //print("down" + axisDown);
                    }
                    if (axis[1] > axis[0])
                    {
                        axisUp = true;
                        print("up " + axisUp);
                    }
                    else if (axis[1] == axis[0])
                    {
                        axisUp = false;
                        //print("up" + axisUp);
                    }
                    axis[0] = Input.GetAxisRaw("Grab");
                    axis[2] = Input.GetAxisRaw("Grab");
                }



                for (int i = 0; i < still.Length; i++)
                {
                    stillCollider[i] = still[i].GetComponent<CircleCollider2D>();
                    if (axisDown && ganchoCollider.bounds.Intersects(stillCollider[i].bounds))
                    {
                        heavy = true;
                        if (thin)
                        {
                            heavy = false;
                        }
                    }
                }

                if (Input.GetAxisRaw("Grab") < 0 && Input.GetButton("Hook") && heavy)
                {
                    if (chain.Length > 0)
                    {
                        camFollow = new Vector3(player.transform.position.x, player.transform.position.y, -20);
                        cam.transform.position = Vector3.Lerp(cam.transform.position, camFollow, camSpeed * Time.deltaTime);
                        player.transform.position = chain[0].transform.position;
                        player.transform.rotation = chain[0].transform.rotation;
                        Destroy(chain[0]);
                    }
                    if (chain.Length == 0)
                    {
                        player.transform.position = gancho.transform.position;
                        jumpChance = true;
                    }
                    player.GetComponent<Rigidbody2D>().velocity *= 0;
                }
                else if (axisUp && heavy)
                {
                    print("printeame estaasdasd");
                    heavy = false;
                    reset = false;
                    axisDown = false;
                    jumpChance = true;
                }

                //Si el gancho agarra algo liviano
                keyClones = GameObject.FindGameObjectsWithTag("Key");
                BoxCollider2D[] keyCollider = new BoxCollider2D[keyClones.Length];

                for (int i = 0; i < keyClones.Length; i++)
                {
                    keyCollider[i] = keyClones[i].GetComponent<BoxCollider2D>();
                    if (axisDown && ganchoCollider.bounds.Intersects(keyCollider[i].bounds))
                    {
                        thin = true;
                        heavy = false;
                        keyAuxIndex = i;
                        break;
                    }
                }
                chain = GameObject.FindGameObjectsWithTag("Chain");
                if (thin)
                {
                    if (chain.Length > 0)
                    {
                        chainSpawnPoint.transform.position = chain[chain.Length - 1].transform.position;
                        chainSpawnPoint.transform.rotation = chain[chain.Length - 1].transform.rotation;
                        Destroy(chain[chain.Length - 1]);
                        range--;
                    }
                    keyClones[keyAuxIndex].transform.position = gancho.transform.position;
                    if (chain.Length == 0)
                    {
                        reset = false;
                        thin = false;
                        gancho.SetActive(false);
                        range = 0;
                        Destroy(keyClones[keyAuxIndex]);
                        keys+=5;
                    }
                }
                if (!gancho.activeInHierarchy)
                {
                    chainSpawnPoint.position = firstChainSpawnPoint.position;
                    chainSpawnPoint.rotation = firstChainSpawnPoint.rotation;
                    range = 0;
                }
            }
            links.text = "Chain : " + keys;
            hpAmount.text = "HP " + hp;
        }
    }

    public void startButton()
    {
        isGameRunning = true;
    }

    public void replayButton()
    {
        Application.LoadLevel("Lvl1");
        replay = true;
    }

    public void exit()
    {
        Application.Quit();
    }
}
