using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggBehavior : MonoBehaviour
{
    // All instance of EggBehavior share this one EggSystem
    private static EggSpawnSystem sEggSystem = null;
    public static void InitializeEggSystem(EggSpawnSystem e) { sEggSystem = e; }
    public Vector3 eggPos;
    private const float kEggSpeed = 40f;
    private float currentTime = 0f;
    private bool movePlane = false;
    private GameObject planeHit;
    // Start is called before the first frame update
    void Start()
    {
        eggPos = this.transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * (kEggSpeed * Time.smoothDeltaTime);
        if (movePlane)
        {
            Vector3 targetPos = Vector3.Scale(eggPos, new Vector3(10f, 10f, 10f));
            //Debug.Log(sEggSystem.eggPos + " " );
            if (currentTime <= 2f)
            {
                currentTime += Time.deltaTime;
                targetPos += planeHit.transform.position;
                Debug.Log(eggPos + " " + targetPos + " " + planeHit.transform.position);
                planeHit.transform.position = Vector3.Lerp(planeHit.transform.position, targetPos, Time.deltaTime);
            }
        }
        // Figure out termination
        bool outside = GameManager.sTheGlobalBehavior.CollideWorldBound(GetComponent<Renderer>().bounds) == CameraSupport.WorldBoundStatus.Outside;
        if (outside)
        {
            DestroyThisEgg("Self");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Egg OnTriggerEnter");
        // Collision with hero (especially when first spawned) does not count
        if (collision.gameObject.name != "Hero")
        {
            DestroyThisEgg(collision.gameObject.name);
            if (collision.gameObject.name == "Enemy(Clone)")
            {
                movePlane = true;
                planeHit = collision.gameObject;

            }

        }

    }

    private void DestroyThisEgg(string name)
    {
        // Watch out!! a collision with overlap objects (e.g., two objects at the same location 
        // will result in two OnTriggerEntger2D() calls!!
        if (gameObject.activeSelf)
        {
            sEggSystem.DecEggCount();
            gameObject.SetActive(false);  // set inactive!
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Calling Egg Destroy on a destroyed egg: " + name);
        }
    }
}