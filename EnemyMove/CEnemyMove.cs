using UnityEngine;
using System.Collections;

public abstract class CEnemyMove : MonoBehaviour
{
    protected CUnit m_GetStat;
    protected CWeapon m_Weapon = new CW_Default();
    [SerializeField]
    protected CEnemy m_Enemy = new CEnemy();
    public enum State
    { // 상태 별 행동
        Idle,               // 가만히 움직임
        Move,            // 이동 중(몬스터의 경우 혼자서 자율 움직임 // 만약에 상황)
        Attack,           // 공격 중
        Trace,            // 추적중(몬스터 해당)
        Die
    };             // 죽음
    public State m_State;      // 현재 상태를 나타내는

    public float m_Speed;            // 유닛 스피드
    public float m_DieTime;           // 죽는 시간(필요 예상)

    public int SkillProbability;

    public bool m_AttackCheck;        // 현재 공격상태인지 체크
    protected bool m_IsDie;           // 죽음 상태
    protected bool m_EnemyHitCheck;
    protected Transform m_EnemyTr;     // 유닛 트랜스폼
    protected Transform m_TargetTr;      // 타겟 트랜스폼
    protected Animator m_EnemyAnim;  // 유닛 애니메이터

    protected NavMeshAgent m_NvAgent;     // 네비게이션

    void Awake()
    {
        m_State = State.Idle;
        m_Speed = 0;
        m_IsDie = false;
        m_AttackCheck = false;
        
    }

    void Start()
    {
        
        

    }

    void Update()
    {

    }

    protected void SetCoroutine()           // 코루틴 시작하기
    {
        StartCoroutine(StateAction());
        StartCoroutine(StateMove());
    }

    protected void SetStopCoroutin()
    {
        StopCoroutine(StateAction());
        StopCoroutine(StateMove());
    }


    protected IEnumerator StateAction()   // 상태별 행동 변화
    {
        while (!m_IsDie)
        {

            // 상태별로 변경시켜준다.
            if (SetStateAttack())
                m_State = State.Attack;
            else if (SetStateMove())
                m_State = State.Move;
            else if (SetStateTrace())
                m_State = State.Trace;
            else
                m_State = State.Idle;
            yield return null;
        }
    }
    protected IEnumerator StateMove() // 변화된 상태에서 사용되는 기능
    {
        while (!m_IsDie)
        {
            if (!m_EnemyHitCheck)
            {
                // 상태별로 움직여준다.
                switch (m_State)
                {
                    case State.Idle:
                        SetIdle();
                        break;
                    case State.Move:
                        SetMove();
                        break;
                    case State.Attack:
                        SetAttack();
                        break;
                    case State.Trace:
                        SetTrace();
                        break;
                    case State.Die:
                        break;
                }
            }
            yield return null;
        }
    }
    public abstract IEnumerator SetTimer(float time);   // 시간초 만큼 움직임( 필요 예상 )

    protected abstract bool SetStateMove();              // 움직이는 조건
    protected abstract bool SetStateAttack();             // 공격하는 조건
    protected abstract bool SetStateTrace();              // 플레이어는 추적하는 조건

    protected abstract void SetIdle();                       // 가만히 있는 부분
    protected abstract void SetMove();                    // 움직이는 부분
    protected abstract void SetAttack();                   // 공격하는 부분
    public abstract void SetHit();                       // 히트 부분
    public abstract void SetDie();                      // 죽는 부분
    protected abstract void SetTrace();                   // 몬스터가 플레이어 추적 부분

    public Transform GetUnitTr()
    {
        return m_EnemyTr;
    }

    public Transform GetTargetTr()
    {
        return m_TargetTr;
    }

    public Animator GetUnitAnim()
    {
        return m_EnemyAnim;
    }

    public void HitCheck()
    {
        m_EnemyHitCheck = false;
    }

    public bool GetCheckDie()
    {
        return m_IsDie;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetHit();           // 히트 모션
            m_Enemy.FromOnDamage(other.GetComponent<PlayerManager>().GetStat().OnAttack());
            if (m_Enemy.Health < 0)
            {
                SetDie();       // 다이 모션
            }
        }
    }

    public CUnit GetStat()
    {
        return m_GetStat;
    }
}
