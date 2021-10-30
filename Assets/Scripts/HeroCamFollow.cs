using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCamFollow : MonoBehaviour
{
    public Transform hero;
    public Vector3 offset;

    void LateUpdate()
    {
        Vector3 targetPos = hero.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, targetPos, 0.125f);
        transform.position = smoothedPos;
    }
    public void Shake(float duration, float magnitutde)
    {
        StartCoroutine(ShakeWaypoint(duration, magnitutde));
        return;
    }
    private IEnumerator ShakeWaypoint(float totalShakeDuration, float magnitutde)
    {
        Transform objTransform = gameObject.transform;
        Vector3 defaultPos = objTransform.position;
        Quaternion defaultRot = objTransform.rotation;

        float counter = 0f;

        const float angleRot = 1.0f;

        while(counter < totalShakeDuration)
        {
            counter += Time.deltaTime;
            Vector3 tempPos = defaultPos + UnityEngine.Random.insideUnitSphere * magnitutde;
            tempPos.z = defaultPos.z;
            objTransform.position = tempPos;
            objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-angleRot, angleRot), new Vector3(0f,0f,1f));

            yield return null;
        }
        objTransform.position = defaultPos;
        objTransform.rotation = defaultRot;

        Debug.Log("Done");
    }
}

//     public IEnumerator Shake(float duration, float magnitude){
//         Vector3 originalPos = transform.localPosition;
//         float elapsed = 0.0f;
//         while (elapsed < duration){
//             float x = Random.Range(-1f, 1f) * magnitude;
//             float y = Random.Range(-1f, 1f) * magnitude;
//             transform.localPosition = new Vector3(x, y, originalPos.z);
//             elapsed += Time.deltaTime;
//             yield return null;
//         }
//         transform.localPosition = originalPos;

//     }
// }
