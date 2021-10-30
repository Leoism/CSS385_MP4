using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedLerp
{
  private float mLerpTime;
  private float mRate;
  private Vector3 mEnd;
  private Vector3 mCurrent;

  // the timed parameters
  private float mStartTime; //
  private bool mLerpEnded;

  public TimedLerp(float timeInSeconds, float rate)
  {
    SetLerpParms(timeInSeconds, rate);
    mLerpEnded = true;
  }
  public void SetLerpParms(float timeInSeconds, float rate)
  {
    mLerpTime = timeInSeconds;
    mRate = rate;
  }
  public void BeginLerp(Vector3 start, Vector3 end)
  {
    mCurrent = start;
    mEnd = end;
    mStartTime = Time.realtimeSinceStartup;
    mLerpEnded = false;
  }
  public Vector3 UpdateLerp()
  {
    mLerpEnded = ((Time.realtimeSinceStartup - mStartTime) > mLerpTime);
    if (mLerpEnded)
      mCurrent = mEnd;
    else
      mCurrent += (mEnd - mCurrent) * (mRate * Time.smoothDeltaTime);
    return mCurrent;
  }
  public bool LerpIsActive() { return !mLerpEnded; }
}
