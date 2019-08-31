using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //これモバイルでも動くんかな

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    GameObject PlayCharacter;
    [SerializeField]
    Camera cam;

    [SerializeField]
    float lensShift;
    [SerializeField, Range(1, 1000)]
    int smoothFrame = 1;

    Vector3 playCharPos;
    float camPosX, camPosY;

    float[] smooth;

    [SerializeField]
    float defaultCamPosY;

    private void Start()
    {
        smooth = Enumerable.Repeat<float>(0, smoothFrame).ToArray();
    }

    private void LateUpdate()
    {
        playCharPos = PlayCharacter.transform.position;

        camPosX = -lensShift * playCharPos.x;
        /*
        for (int i = smoothFrame - 1; i > 0; i--)
        {
            smooth[i] = smooth[i - 1];
        }
        smooth[0] = playCharPos.y;
        camPosY = 0;
        for (int j = 0; j < smoothFrame; j++)
        {
            //camPosY += ((j < smoothFrame / 2 ? 1 : -1) * ((float)2 / smoothFrame) * ((float)2 / smoothFrame) * (j - smoothFrame / 2) + (float)2 / smoothFrame) * smooth[j]; //これを前後半で重みを変える
            camPosY += (float)smooth[j] / smoothFrame;
        }
        */

        float delta = playCharPos.y - (cam.transform.position.y - defaultCamPosY);
        float destination = cam.transform.position.y + delta;
        cam.transform.position = new Vector3(camPosX, Mathf.SmoothDamp(cam.transform.position.y, destination, ref m_velocity, m_dampTime), -0.8f);

        //cam.transform.position = new Vector3(camPosX, camPosY + defaultCamPosY, -0.8f);
        cam.lensShift = new Vector2(-camPosX / 1.2f, -0.1f);
    }


    public float m_dampTime = 0.15f;

    private float m_velocity;

    /*
    private void Update()
    {
        var selfPosition = transform.position;
        var targetPosition = PlayCharacter.transform.position;
        var point = cam.WorldToViewportPoint(targetPosition);
        var delta = targetPosition - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        var destination = selfPosition + delta;
        transform.position = Vector3.SmoothDamp(selfPosition, destination, ref m_velocity, m_dampTime);
    }
    */
}
