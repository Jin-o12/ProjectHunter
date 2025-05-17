using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [ Header("플레이어 스텟") ]
    [SerializeField] public float walkSpeed = 2.0f;         // 해당 속도 이하 걷기 판정
    [SerializeField] public float runSpeed = 5.0f;          // ==최고속도

    [ Header("세부 설정 인자")]
    //플레이어 회전
    [SerializeField] private float rotateSpeed = 720f;           
    [SerializeField] private float rotationSmoothFactor = 0.15f;

    [ Header("외부 컴포넌트") ]
    [SerializeField] private InputKeyManager key;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera mainCam;



    

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {      
        PlayerMove();
        PlayerAttack();
    }

    void PlayerMove()
    {
        // 방향 입력 및 달리기 (WASD)
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");
        bool spr = Input.GetButton("Sprint");

        Vector3 moveDir = new Vector3(hor, 0, ver).normalized;
        float moveSpeed = 0.0f;    // 이동 속도

        // 이동
        if (moveDir != Vector3.zero)
        {
            moveSpeed = spr ? runSpeed : walkSpeed;                 // 달리기 확인인
            //카메라 기준 방향 계산
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0;
            camForward.Normalize();
            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;
            camRight.Normalize();

            Vector3 targetDir = (camForward * ver + camRight * hor).normalized;
            RotateTowardsDirection(targetDir);
        }
        // 속도값 정규화
        float normalizedSpeed = moveSpeed / runSpeed;
        animator.SetFloat("playerSpeed", normalizedSpeed);
    }

    void RotateTowardsDirection(Vector3 targetDir)
    {
        if (targetDir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(targetDir, Vector3.up);

        float t = 1 - Mathf.Exp(-rotateSpeed * rotationSmoothFactor * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, t);
    }

    void PlayerAttack()
    {

    }
}
