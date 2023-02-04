using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private static CameraShake instance;
    public static CameraShake Instance => instance;
    [SerializeField] [Range(0, 1)] float power;
    [SerializeField] [Range(0, 1)] float duration;

    Vector3 originalPos;
    
    private void Awake() 
    {
        if(instance == null)
            instance = this;
        else {
            instance = null;
            Destroy(gameObject);
        }

        originalPos = Camera.main.transform.position;        
    }


    public IEnumerator ShakeCamera() 
    {
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float shakePower = Mathf.Lerp(power, 0, timer / duration);

            Vector3 shakePos = originalPos + Random.insideUnitSphere * shakePower;

            shakePos.z = originalPos.z;
            Camera.main.transform.position = shakePos;
            
            yield return null;
        }
        
        Camera.main.transform.position = new Vector3(4, 9.5f, -10);
    }
}
