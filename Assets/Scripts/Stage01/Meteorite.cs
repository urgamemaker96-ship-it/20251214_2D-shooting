using UnityEngine;

public class Meteorite : MonoBehaviour
{
	[SerializeField]
	private int damage = 5;
	[SerializeField]
	private GameObject explosionPrefab;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// 운석에 부딪힌 오브젝트의 태그가 "Player"이면
		if ( collision.CompareTag("Player") )
		{
			// 운석 공격력만큼 플레이어 체력 감소함.
			collision.GetComponent<PlayerHP>().TakeDamage(damage);
			

			OnDie();
		}
	}
	
	public void OnDie()
	{
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
	}
}


/*
 * File : Meteorite.cs
 * Desc
 *	: 운석 오브젝트에 부착해서 사용
 *
 * Functions
 *	: OnDie() - 운석 사망 시 호출하는 함수
 */