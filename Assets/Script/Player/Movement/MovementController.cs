using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Header("플레이어 스텟")]
    [SerializeField] public float walkSpeed = 2.0f;         // 해당 속도 이하 걷기 판정
    [SerializeField] public float runSpeed = 5.0f;          // ==최고속도


    [Header("외부 컴포넌트")]
    [SerializeField] private InputKeyManager key;
    [SerializeField] private Animator animator;
    [SerializeField] private Camera mainCam;

    /* 플레이어 회전 */
    private float rotateSpeed = 720f;
    private float rotationSmoothFactor = 0.15f;

    /* 플레이어 이동 */
    private int comboStep = 0;                                    // 콤보 번호
    private float[] inputBufferTime = new float[2] { 0.6f, 0.7f };            // 선입력 체크 시간 범위
    const int BUFFER_TIME_MIN = 0;
    const int BUFFER_TIME_MID = 1;
    private bool inputBuffer = false;                                // 선입력 저장 버퍼
    private bool waitComboAttack = false;                                // 다음 콤보까지 대기 (중복입력 방지)

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

    /* 플레이어 공격 */
    void PlayerAttack()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);  //현 애니메이션 상태
        float animProgress = state.normalizedTime;  // 애니메이션 재생 진척도(0~1)
        bool att = Input.GetButtonDown("Attack1");

        // 공격 시작
        if (att)
        {
            if (comboStep == 0)
            {
                comboStep++;
                animator.SetInteger("Attack", comboStep);
            }
            // 이미 공격 중에 공격 상호작용시, 선입력 판정에 대해 계산
            else if (animProgress >= inputBufferTime[BUFFER_TIME_MIN])
            {
                inputBuffer = true;
            }
        }

        // 다음 공격모션이 재생될 수 있는 시간이 넘어가면 콤보 판정 후 즉시 공격모션
        if (comboStep > 0 && animProgress >= inputBufferTime[BUFFER_TIME_MID])
        {
            if (inputBuffer && !waitComboAttack)  // 선입력이 대기중이거나 공격 입력시 (또한 해당 판정을 아직 안했다면)
            {
                comboStep++;
                animator.SetInteger("Attack", comboStep);
                waitComboAttack = true;
            }
            waitComboAttack = false;
            inputBuffer = false;
        }

        // 공격 모션이 끝났을 때까지 위의 조건이 달성되지 않으면 (입력이 없으면)
        if (state.normalizedTime >= 0.95f)
        {
            waitComboAttack = false;
            inputBuffer = false;
            comboStep = 0;
            animator.SetInteger("Attack", comboStep);
        }
    }
}
