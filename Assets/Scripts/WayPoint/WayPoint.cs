using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
  private WaypointCameraBehavior waypointCamera = null;
  private Vector3 mInitPosition = Vector3.zero;
  private int mHitCount = 0;
  private const int kHitLimit = 3;
  private const float kRepositionRange = 15f; // +- this value
  private Color mNormalColor = Color.white;
  private ShakePosition mShake = new ShakePosition(0, 5);
  private TimedLerp mPositionLerp = new TimedLerp(0, 0);
  private bool prevShakeStatus = false;
  private Vector3 posBeforeShake;
  // Start is called before the first frame update
  void Start()
  {
    mInitPosition = transform.position;
  }

  void Update()
  {
    if (!mShake.ShakeDone())
    {
      transform.position = mShake.UpdateShake();
    }
    if (!prevShakeStatus && mShake.ShakeDone())
    {
      waypointCamera.TurnOff();
    }
    prevShakeStatus = mShake.ShakeDone();
  }

  public void SetWaypointCamera(WaypointCameraBehavior cam)
  {
    waypointCamera = cam;
  }


  private void Reposition()
  {
    if (mShake.ShakeDone())
    {
      Vector3 p = mInitPosition;
      p += new Vector3(Random.Range(-kRepositionRange, kRepositionRange),
                       Random.Range(-kRepositionRange, kRepositionRange),
                       0f);
      transform.position = p;
      if (waypointCamera.IsOn())
        waypointCamera.SetCameraPosition(new Vector3(p.x, p.y, -10));
    }
    GetComponent<SpriteRenderer>().color = mNormalColor;
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.gameObject.name == "Egg(Clone)")
    {
      mHitCount++;
      Color c = mNormalColor * (float)(kHitLimit - mHitCount + 1) / (float)(kHitLimit + 1);
      GetComponent<SpriteRenderer>().color = c;
      if (mHitCount > kHitLimit)
      {
        mHitCount = 0;
        Reposition();
      }
      else
        Hit();
    }
  }

  private void Hit()
  {
    if (!waypointCamera.IsOn())
    {
      waypointCamera.TurnOn();
      waypointCamera.SetCameraPosition(transform.position);
      posBeforeShake = transform.position;
    }
    SetShakeParameters(10, mHitCount);
    Shake(new Vector2(mHitCount, mHitCount));
  }

  public void SetShakeParameters(float frequency, float durationSeconds)
  {
    mShake.SetShakeParameters(frequency, durationSeconds);
  }

  public void Shake(Vector2 delta)
  {
    mShake.SetShakeMagnitude(delta, posBeforeShake);
  }
}
