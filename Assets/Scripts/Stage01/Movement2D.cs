using UnityEngine;

public class Movement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Initial Direction")]
    [SerializeField] private Vector3 initialDirection = Vector3.down;

    [Header("Rotation Settings")]
    [SerializeField] private bool useRotation = false; // 발사체 프리팹에서만 true로 체크하세요!

    private Vector3 currentMoveDirection = Vector3.zero;

    public Vector3 MoveDirection
    {
        get => currentMoveDirection;
        set
        {
            currentMoveDirection = value;
            if (useRotation) UpdateRotation();
        }
    }

    private void Start()
    {
        // 핵심 수정: 이미 외부(Weapon)에서 방향을 정해줬다면(zero가 아니라면)
        // 인스펙터의 기본값(initialDirection)으로 덮어쓰지 않습니다.
        if (currentMoveDirection == Vector3.zero && initialDirection != Vector3.zero)
        {
            currentMoveDirection = initialDirection;
            if (useRotation) UpdateRotation();
        }
    }

    private void Update()
    {
        if (currentMoveDirection != Vector3.zero)
        {
            transform.position += currentMoveDirection * moveSpeed * Time.deltaTime;
        }
    }

    private void UpdateRotation()
    {
        if (currentMoveDirection != Vector3.zero)
        {
            // 위(baseDirection = Vector3.up)를 바라보는 발사체 기준 회전 계산
            float angle = Mathf.Atan2(currentMoveDirection.y, currentMoveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    public void MoveTo(Vector3 direction)
    {
        currentMoveDirection = direction;
        if (useRotation) UpdateRotation();
    }
}