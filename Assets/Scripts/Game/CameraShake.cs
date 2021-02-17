using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Camera mainCamera;

    public float ShakeTime;
    Vector3 initialPosition;
    Vector3 cameraPos;

    [SerializeField] [Range(0.01f, 0.1f)] float shakeRange = 0.01f;

    public void VibrateForTime(float time)
    {
        ShakeTime = time;
    }

    public void Shake()
    {

        float cameraPosX = Random.value * shakeRange * 2 - shakeRange;
        float cameraPosY = Random.value * shakeRange * 2 - shakeRange;
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos.x += cameraPosX;
        cameraPos.y += cameraPosY;
        mainCamera.transform.position = cameraPos;
        ShakeTime -= Time.deltaTime;
    }

    public void cameraReset()
    {
        mainCamera.transform.position = initialPosition;
    }

    private void Start()
    {
        initialPosition = new Vector3(0f, 0f, -10f);
        cameraPos = mainCamera.transform.position;
    }
}