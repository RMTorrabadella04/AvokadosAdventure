using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class enemiesController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius;
    public float attackRadius;

    private Vector2 movement;

    private Rigidbody2D Rigidbody2D;
    private Animator Animator;

    private bool search = true;
    public float Speed;
    public float Vida;
    public float Danyo;
    public bool takeDamage;
    private bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Movimiento();

        if (canAttack && Vector2.Distance(transform.position, player.position) < attackRadius)
        {
            StartCoroutine(attack());
        }
    }
    
    public void Movimiento()
    {

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius && search)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            movement = new Vector2(direction.x, 0);

            if (direction.x > 0) // Jugador a la derecha
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (direction.x < 0) // Jugador a la izquierda
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }

            Animator.SetBool("Running", true);

        }
        else
        {
            movement = Vector2.zero;

            Animator.SetBool("Running", false);
        }

        Rigidbody2D.MovePosition(Rigidbody2D.position + movement * Speed * Time.deltaTime);

    }



    IEnumerator attack()
    {
        canAttack = false; // Bloquea el ataque mientras está en espera

        Animator.SetBool("attacking", true); // Inicia la animación de ataque
        yield return new WaitForSeconds(0.5f); // Espera la duración de la animación

        yield return new WaitForSeconds(3f); // Espera antes de permitir otro ataque

        canAttack = true; // Ahora puede volver a atacar
    }

    public void desactivateAttack()
    {
        Animator.SetBool("attacking", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Vida>().vidaActual.Equals(0))
        {
            search = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword"))
        {
            EnemyTakingDamage(collision);
        }


    }

    // Recibir Daño

    public void EnemyTakingDamage(Collider2D collision)
    {
        if (!takeDamage)
        {
            Vida = Vida - 10f;

            if (Vida.Equals(0f))
            {
                Animator.SetBool("IsDead", true);
            }
            else if (collision.gameObject.CompareTag("Sword"))
            {
                takeDamage = true;
                Animator.SetBool("TakeDamage", true); // Inicia la animación de daño

                // Agregar un rebote más o menos (no me convence)
                Vector2 dashDirection = (transform.position - collision.transform.position).normalized;

                // Aplicamos el "dash" al personaje usando AddForce
                Rigidbody2D.AddForce(dashDirection, ForceMode2D.Impulse);
            }
        }
    }

    public void DesactivateDamage()
    {
        takeDamage = false; // Permite que el personaje se mueva de nuevo
        Animator.SetBool("TakeDamage", false); // Desactiva la animación de daño
        Rigidbody2D.velocity = Vector2.zero;
    }

    // Destruir el enemigo

    public void CambiarEscenaAVictoria()
    {
        SceneManager.LoadScene(3);
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);  // Esto destruye el GameObject al que está asociado este script
    }


    // Area de la busqueda y de daño
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
