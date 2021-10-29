using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointCameraBehavior : MonoBehaviour
{
  private int hitCount = 0;
  private int shakeCycles = 0;
  private Camera waypointCamera = null;
  private bool startShutDown = false;
  private bool isShaking = false;
  private bool isOn = false;
  // Start is called before the first frame update
  void Start()
  {
    waypointCamera = GetComponent<Camera>();
    TurnOff();
  }

  // Update is called once per frame
  void Update()
  {
    // if (startShutDown)
    // {
    //   waypointCamera.enabled = false;
    //   startShutDown = false;
    // }
  }

  public bool IsOn()
  {
    return isOn;
  }

  public void SetCameraPosition(Vector3 newPosition)
  {
    waypointCamera.transform.position = new Vector3(newPosition.x, newPosition.y, -10);
  }

  public void TurnOn()
  {
    waypointCamera.backgroundColor = new Color(0.1762193f, 0.3648541f, 0.6792453f);
    isOn = true;
    // waypointCamera.enabled = true;
  }

  public void TurnOff()
  {
    waypointCamera.backgroundColor = new Color(0, 0, 0);
    waypointCamera.transform.position = new Vector3(255, 255, 1);
    isOn = false;
    // startShutDown = true;
  }

  public string GetCameraState()
  {
    return "WayPoint Cam: " + (IsOn() ? "Active" : "Shut Off");
  }
}
