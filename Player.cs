using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    private Vector3 direction;
    private CharacterController controller;
    private float verticalVelocity;
    public float speed;

    public Transform cameraTransform;

    private Transform destination;                                      //자동이동의 목적지 
    private bool isMoveto;

    public event Action EvArrive;

    private float h, v;
    public FieldItem closetItem;
    public float radius;

    private CombatManager combatManager;
    public TutorialGuide tutorialGuide;
    public static Action OnMoveTClear;

    public Action<float> OnTakeDamage;


    private Vector3 moveDir;
    private uint moveTimeStamp = 0;

    private float moveSyncTimer;
    private const float MOVE_SYNC_INTERVAL = 0.05f;

    public event Action OnDied;

    public PlayerStatManager statManager;
    public void takeDamage(float damage, Transform attacker)
    {
        EffectManager.Instance.PlayEffect("Damaged",this.transform.position);
        if(combatManager.targetEnemy==null)
        {
            combatManager.targetEnemy = attacker.gameObject;
        }
        statManager.set_hp(statManager.get_hp() - statManager.calculate_damaged(damage));
        OnTakeDamage?.Invoke(statManager.get_hp());
        if(statManager.get_hp()<=0)
        {
            OnDied?.Invoke();
            return;
        }
        combatManager.knockBack();
        Debug.Log(statManager.get_hp() + "는 현재 남은hp이다");
    }

    public Vector3 getInputVector()
    {
        if (isMoveto)
            return new Vector3(1, 0, 0);
        Vector3 vec = new Vector3(h, 0, v);
        return vec;
    }

    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        combatManager = this.GetComponent<CombatManager>();
        speed = 4.0f;
        radius = 50.0f;
        statManager = this.GetComponent<PlayerStatManager>();
        
    }
    void LateUpdate()
    {
        moveSyncTimer += Time.deltaTime;
        if (moveSyncTimer >= MOVE_SYNC_INTERVAL)
        {
            moveSyncTimer = 0f;
            SendMoveSync();
        }
    }
    void SendMoveSync()
    {
        moveTimeStamp++;
        Vector3 pos = transform.position;

        bool isMoving =
               moveDir.sqrMagnitude > 0.001f || isMoveto;

        float speedParam = isMoving ? 1f : 0f;

        byte[] packet = PacketMethod.BuildMoveSyncRequest(
            NetworkClient.Instance.userDBId,
            pos,
            speedParam,moveTimeStamp
        );

        NetworkClient.Instance.Send(packet);
    }
    void Update()
    {

        if (combatManager.playerStateContexter.getCurrState() is AttackState || combatManager.playerStateContexter.getCurrState() is DamagedState)
            return;
        /*=========플레이어 정상 이동코드=========*/

        h = InputManager.Instance.GetHorizontal();
        v = InputManager.Instance.GetVertical();



        if (isMoveto)
        {
            if (h != 0 || v != 0)                  //플레이어 입력이 있으면 자동이동 중단
            {
                stopMoveto();
            }
            else
            {
                moveTo();
            }
        }



        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        moveDir = cameraForward * v + cameraTransform.right * h;

        direction = moveDir * speed;

        if (controller.isGrounded)
        {
            verticalVelocity = -2.0f;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        direction.y = verticalVelocity;
        controller.Move(direction * Time.deltaTime);

        // 회전은 이동 여부와 상관없이 적용
        if (moveDir.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * 10f);
        }

    }

    bool moveTo()
    {

        Vector3 dir_vec = destination.position - this.transform.position;                     // 방향벡터를 구함

        if (dir_vec.magnitude < 2.0f)                                                         // 목표 지점에 근접하면 중지
        {
            isMoveto = false;

            if (EvArrive != null)
            {
                EvArrive();
            }
            return true;
        }


        Vector3 u_vec = dir_vec.normalized;                                                   // 유닛벡터를 구한 해당 방향으로 천천히 이동

        controller.Move(u_vec * speed * Time.deltaTime);

        Vector3 horizontalDirection = new Vector3(u_vec.x, 0, u_vec.z);


        if (horizontalDirection != Vector3.zero)                                              //x축으로 플레이어가 뒤집어지는걸 방지
        {
            // 이동 방향으로 캐릭터 회전
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(horizontalDirection), Time.deltaTime * 10f);
        }

        return false;
    }

    public void lookAtTarget(Transform target_transform)
    {
        if (target_transform == null)
            return;

        Vector3 directionToTarget = target_transform.position - transform.position;


        Vector3 horizontalDirection = new Vector3(directionToTarget.x, 0, directionToTarget.z);

        if (horizontalDirection == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(horizontalDirection);

        transform.rotation = targetRotation;
    }
    public void startMoveto(Transform target_transform)
    {
        destination = target_transform;
        isMoveto = true;
    }


    public void stopMoveto()
    {
        destination = null;
        isMoveto = false;
    }

    public GameObject detectObtainable()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, radius);

        if (colliders.Count() <= 0)
            return null;

        float min_distance = float.MaxValue;
        GameObject closetObj = null;
        foreach (var it in colliders)
        {
            var temp = it.gameObject.GetComponent<IObtainable>();
            if (temp == null)
                continue;

            float distance = (it.transform.position - this.transform.position).sqrMagnitude;

            if (distance <= min_distance)
            {
                closetObj = it.gameObject;
                min_distance = distance;
            }

        }

        closetItem = closetObj.GetComponent<FieldItem>();
        if (closetItem != null)
        {
            return closetObj;
        }

        return null;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TutorialBox"))
        {
            other.gameObject.SetActive(false);
            OnMoveTClear?.Invoke();
        }
    }

    public CombatManager getCombatManager()
    {
        return combatManager;
    }
}
