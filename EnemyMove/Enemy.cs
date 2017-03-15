using UnityEngine;
using System.Collections;
using System;

public class Enemy : CEnemyMove{

    public float m_TraceDist;              // 추적 거리
    public float m_RangeDist;              // 공격 사정 거리

    public float m_TargetDist;
    void Awake(){
        m_NvAgent = GetComponent<NavMeshAgent>();
        m_TargetTr = GameObject.FindWithTag("Player").transform;
        m_NvAgent.speed = m_Speed;
        m_NvAgent.stoppingDistance = m_RangeDist;
        m_TargetDist = Vector3.Distance(m_TargetTr.position, transform.position);
        m_EnemyAnim = GetComponent<Animator>();
        SetCoroutine();
    }

    void Start () {
        
    }
	
	void Update () {
        m_TargetDist = Vector3.Distance(m_TargetTr.position, transform.position);
    }

    protected override bool SetStateMove()
    {
        // 움직이는 조건
        if (m_TargetDist > m_TraceDist)
            return true;
        else
            return false;
    }
    protected override bool SetStateAttack()
    {
        // 공격하는 조건
        if (m_TargetDist <= m_RangeDist)
            return true;
        else
            return false;
    }
    protected override bool SetStateTrace()
    {
        // 플레이어는 추적하는 조건
        if (m_TargetDist <= m_TraceDist)
            return true;
        else
            return false;
    }

    protected override void SetIdle()
    {
        // 가만히 있는 부분
        m_EnemyAnim.SetBool("IsMove", false);
        m_EnemyAnim.SetBool("IsTrace", false);
        m_EnemyAnim.SetBool("IsAttack", false);
    }

    protected override void SetMove()
    {
        // 움직이는 부분
        //        Debug.Log("적이 움직인다.");
        m_EnemyAnim.SetBool("IsMove", true);
        m_EnemyAnim.SetBool("IsTrace", false);
        m_EnemyAnim.SetBool("IsAttack", false);
        m_NvAgent.Stop();
    }

    protected override void SetAttack()
    {
        // 공격하는 부분
        //        Debug.Log("적이 공격한다.");
        m_EnemyAnim.SetBool("IsMove", false);
        m_EnemyAnim.SetBool("IsTrace", false);
        m_EnemyAnim.SetBool("IsAttack", true);
        m_NvAgent.Stop();
    }

    public override void SetDie()
    {
        // 죽는 부분
//        Debug.Log("몬스터가 죽었다.");
        m_IsDie = true;
        m_EnemyAnim.SetTrigger("IsDie");
        GetComponent<CapsuleCollider>().enabled = false;
        Destroy(gameObject, 2.1f);
    }

    public override void SetHit()
    {
        // 히트 부분
        m_EnemyHitCheck = true;
        Camera.main.GetComponent<MouseOrbit>().DoShake(0.1f, 0.01f);
        m_EnemyAnim.SetTrigger("IsHit");

    }

    protected override void SetTrace()
    {
        // 추적 부분
        //        Debug.Log("적이 플레이어를 추적한다.");
        m_EnemyAnim.SetBool("IsMove", false);
        m_EnemyAnim.SetBool("IsTrace", true);
        m_EnemyAnim.SetBool("IsAttack", false);
        m_NvAgent.destination = m_TargetTr.position;
        m_NvAgent.Resume();
    }

    public override IEnumerator SetTimer(float time) // (예상)
    {
        yield return new WaitForSeconds(time);
        // 타이머 지나고 실행될 코드 작성
    }

    public void PlayerLookAt()
    {
        Quaternion lookAt = Quaternion.identity;    // Querternion 함수 선언
        Vector3 lookatVec = (m_TargetTr.transform.position - transform.position).normalized; //타겟 위치 - 자신 위치 -> 노멀라이즈
        lookAt.SetLookRotation(lookatVec);  // 쿼터니언의 SetLookRotaion 함수 적용

        iTween.RotateTo(gameObject, new Vector3(0, lookAt.eulerAngles.y, 0), 1f);
    }
}
