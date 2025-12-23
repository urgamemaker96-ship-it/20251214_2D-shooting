using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackRate = 0.1f;
    [SerializeField] private int maxAttackLevel = 3;
    [SerializeField] private GameObject boomPrefab;
    [SerializeField] private Vector3 baseDirection = Vector3.up;

    // 두 개의 사운드 클립 추가 
    [SerializeField] private AudioClip attackSound;  // Z키 공격 사운드
    [SerializeField] private AudioClip bombSound;    // X키 폭탄 사운드

    private int attackLevel = 1;
    private int boomCount = 3;
    private AudioSource audioSource;
    private Coroutine attackCoroutine;
    private bool isFiring = false;

    public int AttackLevel
    {
        set => attackLevel = Mathf.Clamp(value, 1, maxAttackLevel);
        get => attackLevel;
    }
    public int BoomCount
    {
        set => boomCount = Mathf.Max(0, value);
        get => boomCount;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.zKey.wasPressedThisFrame)
            {
                Debug.Log("Weapon: Z키 눌림 - 공격 시작");
                StartFiring();
            }

            if (Keyboard.current.zKey.wasReleasedThisFrame)
            {
                Debug.Log("Weapon: Z키 뗌 - 공격 중지");
                StopFiring();
            }

            if (Keyboard.current.xKey.wasPressedThisFrame)
            {
                Debug.Log("Weapon: X키 눌림 - 폭탄 발사");
                StartBoom();
            }
        }
    }

    public void StartFiring()
    {
        if (isFiring) return;

        isFiring = true;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }

        attackCoroutine = StartCoroutine(TryAttack());
    }

    public void StopFiring()
    {
        if (!isFiring) return;

        isFiring = false;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    public void StartBoom()
    {
        if (boomCount > 0)
        {
            boomCount--;

            // 폭탄 생성
            Instantiate(boomPrefab, transform.position, Quaternion.identity);
            Debug.Log($"폭탄 발사! 남은 폭탄: {boomCount}");

            // 폭탄 발사음 재생 (bombSound 사용) 
            if (audioSource != null)
            {
                if (bombSound != null)
                {
                    audioSource.PlayOneShot(bombSound);
                    Debug.Log("폭탄 발사음 재생 (bombSound)");
                }
                else if (audioSource.clip != null)
                {
                    // bombSound가 없으면 기존 클립 사용
                    audioSource.PlayOneShot(audioSource.clip);
                    Debug.Log("폭탄 발사음 재생 (기존 클립)");
                }
            }
        }
        else
        {
            Debug.Log("폭탄이 없습니다!");
        }
    }

    private IEnumerator TryAttack()
    {
        while (isFiring)
        {
            AttackByLevel();

            // 일반 공격 사운드 재생 (attackSound 사용) 
            if (audioSource != null)
            {
                if (attackSound != null)
                {
                    audioSource.PlayOneShot(attackSound);
                }
                else if (audioSource.clip != null)
                {
                    // attackSound가 없으면 기존 클립 사용
                    audioSource.PlayOneShot(audioSource.clip);
                }
            }

            yield return new WaitForSeconds(attackRate);
        }
    }

    private void AttackByLevel()
    {
        GameObject cloneProjectile = null;
        Movement2D movement2D = null;

        switch (attackLevel)
        {
            case 1:
                cloneProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                movement2D = cloneProjectile.GetComponent<Movement2D>();
                movement2D.MoveDirection = baseDirection;
                break;

            case 2:
                // 왼쪽 발사체
                cloneProjectile = Instantiate(projectilePrefab, transform.position + Vector3.left * 0.2f, Quaternion.identity);
                movement2D = cloneProjectile.GetComponent<Movement2D>();
                movement2D.MoveDirection = baseDirection;

                // 오른쪽 발사체
                cloneProjectile = Instantiate(projectilePrefab, transform.position + Vector3.right * 0.2f, Quaternion.identity);
                movement2D = cloneProjectile.GetComponent<Movement2D>();
                movement2D.MoveDirection = baseDirection;
                break;

            case 3:
                // 1. 중앙 전방 발사체 (회전 없음)
                cloneProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                movement2D = cloneProjectile.GetComponent<Movement2D>();
                movement2D.MoveDirection = baseDirection;

                // 2. 왼쪽 대각선 발사체 (약 20도 왼쪽으로 회전)
                Vector3 leftPos = transform.position + Vector3.left * 0.2f;
                // Z축을 20도 회전 (위쪽 기준 왼쪽은 양수)
                Quaternion leftRotation = Quaternion.Euler(0, 0, 20f);
                cloneProjectile = Instantiate(projectilePrefab, leftPos, leftRotation);
                movement2D = cloneProjectile.GetComponent<Movement2D>();
                // 방향도 회전값에 맞춰 계산
                movement2D.MoveDirection = (leftRotation * baseDirection).normalized;

                // 3. 오른쪽 대각선 발사체 (약 20도 오른쪽으로 회전)
                Vector3 rightPos = transform.position + Vector3.right * 0.2f;
                // Z축을 -20도 회전 (위쪽 기준 오른쪽은 음수)
                Quaternion rightRotation = Quaternion.Euler(0, 0, -20f);
                cloneProjectile = Instantiate(projectilePrefab, rightPos, rightRotation);
                movement2D = cloneProjectile.GetComponent<Movement2D>();
                // 방향도 회전값에 맞춰 계산
                movement2D.MoveDirection = (rightRotation * baseDirection).normalized;
                break;
        }
    }
}