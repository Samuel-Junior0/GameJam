using UnityEngine;

// Compadre que segue o jogador. Nao luta, nao leva dano (barato e sem bugs).
// Ative o objeto so quando o compadre "entrar no grupo" (ex.: no onFinished do DialogueTrigger).
public class CompanionFollow : MonoBehaviour
{
    public Transform target;            // se vazio, procura a tag Player
    public float speed = 3f;
    public float followDistance = 1.8f;

    SpriteRenderer sprite;

    void Start()
    {
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) target = p.transform;
        }
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (target == null || GameManager.InputLocked) return;

        if (Vector2.Distance(transform.position, target.position) > followDistance)
        {
            Vector2 next = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            if (sprite && Mathf.Abs(next.x - transform.position.x) > 0.001f)
                sprite.flipX = next.x < transform.position.x;
            transform.position = next;
        }
    }
}
