using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private StageData stageData;
    private Weapon weapon;

    private Movement2D movement2D;
    private Vector2 moveInput = Vector2.zero;
    private bool isDie = false;
    private Animator animator;

    private int score;
    public int Score
    {
        set => score = Mathf.Max(0, value);
        get => score;
    }

    private void Awake()
    {
        movement2D = GetComponent<Movement2D>();
        animator = GetComponent<Animator>();

        // Weapon 컴포넌트 찾기
        if (weapon == null)
        {
            weapon = GetComponent<Weapon>();
        }

        Debug.Log($"PlayerController 초기화 완료");
        Debug.Log($"- Movement2D: {movement2D != null}");
        Debug.Log($"- Weapon: {weapon != null}");
        Debug.Log($"- Animator: {animator != null}");
    }

    private void Update()
    {
        if (isDie == true) return;

        movement2D.MoveDirection = moveInput;

        
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, stageData.LimitMin.x, stageData.LimitMax.x),
            Mathf.Clamp(transform.position.y, stageData.LimitMin.y, stageData.LimitMax.y)
        );
    }

    public void OnDie()
    {
        isDie = true;
        animator.SetTrigger("onDie");
        Destroy(GetComponent<CircleCollider2D>());

        // 공격 중지
        if (weapon != null)
        {
            // Weapon에 공격 중지 메서드가 있다면 호출
            // weapon.StopAllAttacks();
        }
    }

    public void OnDieEvent()
    {
        PlayerPrefs.SetInt("Score", score);
        SceneManager.LoadScene(nextSceneName);
    }

    // Input System을 통한 이동 입력 처리
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            moveInput = context.ReadValue<Vector2>();
            Debug.Log($"이동 입력: {moveInput}");
        }
    }

    // 필요하다면 Weapon 클래스에 방향을 설정하는 메서드
    public void SetWeaponDirection(Vector3 direction)
    {
        if (weapon != null)
        {
            // Weapon 클래스에 SetDirection 메서드가 있다고 가정
            // weapon.SetDirection(direction);
        }
    }
}