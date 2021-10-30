 using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeroBehavior : MonoBehaviour {
    
    public EggSpawnSystem mEggSystem = null;
    // public GameObject heroPos;
    private const float kHeroRotateSpeed = 90f/2f; // 90-degrees in 2 seconds
    private const float kHeroSpeed = 20f;  // 20-units in a second
    private float mHeroSpeed = kHeroSpeed;
    private bool mMouseDrive = true;
    public GameObject heroPos = null;
    //  Hero state
    private bool isChased = false;
    //  Hero state
    private int mHeroTouchedEnemy = 0;
    private int hitByEnemy = 0;

    public void TouchedEnemy() { mHeroTouchedEnemy++; }

    private void EnemyHit() 
    {
        if (isChased){
            hitByEnemy++;
        }
    }
    public string GetHeroState() { return "HERO: Drive(" + (mMouseDrive?"Mouse":"Key") + 
                                          ") Hit(" + hitByEnemy + ")   " 
                                            + mEggSystem.EggSystemStatus(); }
    private void Awake()
    {
        // Actually since Hero spwans eggs, this can be done in the Start() function, but, 
        // just to show this can also be done here.
        Debug.Assert(mEggSystem != null);
        EggBehavior.InitializeEggSystem(mEggSystem);
    }

    void Start ()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<EnemyBehavior>().IsChased())
            {
                isChased = true;
                return;
            }
        }
        isChased = false;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateMotion();
        ProcessEggSpwan();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");

        foreach (GameObject enemy in enemies)
        {
        if (enemy.GetComponent<EnemyBehavior>().IsChased())
        {
            isChased = true;
            return;
        }
        }
        isChased = false;
    }

    private int EggsOnScreen() { return mEggSystem.GetEggCount();  }

    private void UpdateMotion()
    {
        if (Input.GetKeyDown(KeyCode.M))
            mMouseDrive = !mMouseDrive;
            
        // Only support rotation
        transform.Rotate(Vector3.forward, -1f * Input.GetAxis("Horizontal") *
                                    (kHeroRotateSpeed * Time.smoothDeltaTime));
        if (mMouseDrive)
        {
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0f;
            transform.position = p;
        } else
        {
            mHeroSpeed += Input.GetAxis("Vertical");
            transform.position += transform.up * (mHeroSpeed * Time.smoothDeltaTime);
        }
    }

    private void ProcessEggSpwan()
    {
        if (mEggSystem.CanSpawn())
        {
            if (Input.GetKey("space"))
                mEggSystem.SpawnAnEgg(transform.position, transform.up);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Enemy(Clone)")
            EnemyHit();
    }
}