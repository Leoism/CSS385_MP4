using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class EnemyCamBehavior : MonoBehaviour
{
    
    private Camera cam;
    public Transform hero = null;
    private Vector3 offset = new Vector3(0, 0, -10);
    //private float smoothTime = .5f;


    private Transform target;
    [Range(1, 10)]
    public float smoothFactor;
    private bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        Disable();
    }


    void LateUpdate()
    {
        if (isActive)
        {
            Move();
        }
    }

    public void SetTarget(Transform target)
    {
        if(isActive == false)
        this.target = target;
    }
    public void Enable()
    {
        cam.backgroundColor = new Color(.7f, 0.2f, 0.2f);
        isActive = true;
        Debug.Log("Enemy Cam Enabled");
    }
    public void setInactive()
    {
        isActive = false;
        target = null;
        Disable();
    }
    public void Disable()
    {
        cam.backgroundColor = new Color(0, 0, 0);
        cam.transform.position = new Vector3(255, 255, 1);
        isActive = false;
        Debug.Log("Enemy Cam Disabled");
    }
    
    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;
        cam.orthographicSize = (hero.position - target.position).magnitude;
        transform.position = newPosition;
    }

    private Vector3 GetCenterPoint()
    {

        var bounds = new Bounds(hero.position, Vector3.zero);
        
        bounds.Encapsulate(target.position);
        
        return bounds.center;
    }
    public void SetViewportMinPos(float x, float y)
    {
        Rect r = cam.rect;
        cam.rect = new Rect(x, y, r.width, r.height);
    }

    public void SetViewportSize(float w, float h)
    {
        Rect r = cam.rect;
        cam.rect = new Rect(r.x, r.y, w, h);
    }
    public bool IsActive()
    {
        return isActive;
    }
    public string GetEnemyCamState()
    {
        return "Enemy Chase Cam: " + (IsActive() ? "Active" : "Shut off");
    }

}
