using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    [ Header("외부 컴포넌트") ]
    [SerializeField] public Transform player;

    [HideInInspector] private Vector3 positionOffset       = new Vector3(0, 3, -5);        // 위치 기본 값  
    [HideInInspector] private Vector3 rotationOffset       = new Vector3(0, 1.8f, 0);      // 회전 기본 값
    [HideInInspector] public float mouseSensitivity       = 800f;                         // 마우스 감도
    [HideInInspector] private float xRotation             = 0f;                           // 수평 회전
    [HideInInspector] private float yRotation             = 0f;                           // 수직 회전 
    [HideInInspector] public float distanceFromPlayer;                                    // 플레이어와의 거리
    [HideInInspector] public float yMin                   = -30f;                         // 수직 시점 최소값
    [HideInInspector] public float yMax                   = 70f;                          // 수직 시점 최댓값값
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;                       //마우스 숨김 & 고정
    }

    void LateUpdate()
    {
        //플레이어 트래킹시 최종 위치값으로 위치 변경, 플레이어가 없으면 트래킹 하지 않음
        if(player==null)
            {return;}
        Vector3 finalPosition = player.position + positionOffset;
        transform.position = finalPosition;

        transform.position = transform.position;
        transform.LookAt(player.position + rotationOffset);

        distanceFromPlayer = Math.Abs(Vector3.Distance(player.transform.position, transform.position));
        CameraMoveByMouse();
    }

    void CameraMoveByMouse()
    {
        // 마우스 입력값
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 수직 회전 제한
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, yMin, yMax);

        // 회전 각도로 방향 계산 (구 궤도 상 위치)
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Vector3 negDistance = new Vector3(0, 0, -distanceFromPlayer);
        Vector3 position = rotation * negDistance + player.position;

        // 위치와 방향 설정
        transform.rotation = rotation;
        transform.position = position;
    }
}
