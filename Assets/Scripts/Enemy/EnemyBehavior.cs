using UnityEngine;
using System.Collections;

public partial class EnemyBehavior : MonoBehaviour
{

    // All instances of Enemy shares this one WayPoint and EnemySystem
    static private WayPointSystem sWayPoints = null;
    static private EnemySpawnSystem sEnemySystem = null;
    static private HeroBehavior sHeroSystem = null;
    static private EnemyCamBehavior sEnemyCamera = null;

    private bool enemCamActive = false;
    static public void InitializeEnemySystem(EnemySpawnSystem s, WayPointSystem w, HeroBehavior h, EnemyCamBehavior e) { sEnemySystem = s; sWayPoints = w; sHeroSystem = h; sEnemyCamera = e; }

    private const float kSpeed = 5f;
    private int mWayPointIndex = 0;

    private const float kTurnRate = 0.03f / 60f;
    private int mNumHit = 0;
    //private const int kHitsToDestroy = 4;
    private const float kEnemyEnergyLost = 0.8f;

    private float currentScale;
    private Vector3 scale;
    private Vector3 scaleEnlarge;
    Color c;

    private float ccwRotation = 90;
    private float FrameEnd = 0;
    private float FrameStart = 0;

    private bool ccwStateCheck = false;
    private bool cwStateCheck = false;
    private bool chaseStateCheck = false;
    private bool enalargeStateCheck = false;
    private bool shrinkStateCheck = false;
    private bool stunnedStateCheck = false;
    private bool eggStateCheck = false;
    // Use this for initialization
    void Start()
    {
        scale = transform.localScale;
        scaleEnlarge = scale * 2;
        c = GetComponent<Renderer>().material.color;
        mWayPointIndex = sWayPoints.GetInitWayIndex();
    }

    // Update is called once per frame
    void Update()
    {
        //Replace with cases
        //Plane State machine
        if (ccwStateCheck)
            ccwState();
        else if (cwStateCheck)
            cwState();
        else if (chaseStateCheck)
            chaseState();
        else if (enalargeStateCheck)
            enlargeState();
        else if (shrinkStateCheck)
            shrinkState();
        else if (stunnedStateCheck)
            stunnedState();
        else if (eggStateCheck)
            eggState();
        else
            moveForward();

    }

    private void PointAtPosition(Vector3 p, float r)
    {
        Vector3 v = p - transform.position;
        transform.up = Vector3.LerpUnclamped(transform.up, v, r);
    }
    private void moveForward()
    {
        sWayPoints.CheckNextWayPoint(transform.position, ref mWayPointIndex);
        PointAtPosition(sWayPoints.WayPoint(mWayPointIndex), kTurnRate);
        transform.position += (kSpeed * Time.smoothDeltaTime) * transform.up;
    }

    public bool IsChased()
    {
        return chaseStateCheck;
    }

    #region Trigger into chase or die
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Emeny OnTriggerEnter");
        TriggerCheck(collision.gameObject, collision);
    }

    private void TriggerCheck(GameObject g, Collider2D c)
    {
        if (g.name == "Hero" && !stunnedStateCheck && !eggStateCheck && !chaseStateCheck && !cwStateCheck && !ccwStateCheck)
        {
            //ThisEnemyIsHit();
            ccwStateCheck = true;
            FrameStart = Time.frameCount;
            FrameEnd = Time.frameCount + 60;
        }
        else if (g.name == "Hero" && !stunnedStateCheck && !eggStateCheck && chaseStateCheck)
        {
            ThisEnemyIsHit();
            sHeroSystem.TouchedEnemy();
        }
        else if (g.name == "Egg(Clone)")
        {
            enemCamActive = false;
            ccwStateCheck = cwStateCheck = chaseStateCheck
                = enalargeStateCheck = shrinkStateCheck = false;

            mNumHit++;
            if (stunnedStateCheck)
            {
                //Vector3 targetPos = Vector3.Scale(sEggSystem.eggPos, new Vector3 (4,4,4));

                /*targetPos += transform.position;
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime/2f);*/
                eggStateCheck = true;
                stunnedStateCheck = false;
            }
            else if (eggStateCheck)
            {
                ThisEnemyIsHit();
            }
            else
            {
                stunnedStateCheck = true;
            }

        }
    }

    private void ThisEnemyIsHit()
    {

        sEnemyCamera.Disable();
        sEnemySystem.OneEnemyDestroyed();
        Destroy(gameObject);
    }

    private void ccwState()
    {
    
        enemCamActive = true;
        Color redState = Color.red;
        GetComponent<Renderer>().material.color = redState;
        if (Time.frameCount <= FrameEnd)
        {
            //float lerpPercentage = (Time.frameCount - ccwFrameStart) / 60f;
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 90f / 60f);
        }
        else
        {
            FrameStart = Time.frameCount;
            FrameEnd = Time.frameCount + 60;
            ccwStateCheck = false;
            cwStateCheck = true;
        }


        Debug.Log("ccwState");
    }

    private void cwState()
    {
        if (Time.frameCount <= FrameEnd)
        {
            float lerpPercentage = (Time.frameCount - FrameStart) / 60f;
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - 90f / 60f);
        }
        else
        {
            cwStateCheck = false;
            chaseStateCheck = true;
        }
        Debug.Log("cwState");
    }
    private void chaseState()
    {
        sEnemyCamera.SetTarget(transform);
        sEnemyCamera.Enable();
        float distance = Vector3.Distance(transform.position, sHeroSystem.heroPos.transform.position);
        if (distance <= 40)
        {
            PointAtPosition(sHeroSystem.heroPos.transform.position, 1);
            transform.position += (kSpeed * Time.smoothDeltaTime) * transform.up;
        }
        else
        {
            sEnemyCamera.Disable();
            FrameStart = Time.frameCount;
            FrameEnd = Time.frameCount + 60;
            chaseStateCheck = false;
            enalargeStateCheck = true;
            currentScale = transform.localScale.x;
        }
        Debug.Log("chaseState");
    }
    private void enlargeState()
    {
        if (Time.frameCount <= FrameEnd)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, scaleEnlarge, Time.deltaTime * 2f);
        }
        else
        {
            FrameStart = Time.frameCount;
            FrameEnd = Time.frameCount + 60;
            currentScale = transform.localScale.x;
            enalargeStateCheck = false;
            shrinkStateCheck = true;
        }


        Debug.Log("enlargeState");
    }
    private void shrinkState()
    {
        if (Time.frameCount <= FrameEnd)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, scale, Time.deltaTime * 2f);
        }
        else
        {
            enalargeStateCheck = false;
            shrinkStateCheck = false;
            GetComponent<Renderer>().material.color = c;
        }
        Debug.Log("shrinkState");
    }

    private void stunnedState()
    {
        if (enemCamActive == false)
        {
            sEnemyCamera.Disable();
            enemCamActive = true;
        }
        //GetComponent<Renderer>().material.color = c;
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/EnemyStunned");
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + 90f / 60f);
        Debug.Log("stunnedState");
    }
    private void eggState()
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/Egg");
        Debug.Log("eggState");
    }
    #endregion
}
