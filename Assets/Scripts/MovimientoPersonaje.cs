using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class MovimientoPersonaje : MonoBehaviour
{
    public float Speed;

    public bool Grounded;

    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;

    public PhysicsMaterial2D bounceMat, normalMat;
    public bool canJump = true;
    public float jumpValue = 0.0f;

    public bool withShield;
    public bool takeDamage;
    public bool isDashing = false;
    private bool attacking;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        // Para no poder moverte mientras atacas

        if (!attacking || !takeDamage)
        {
            Movimiento();
        }

        Ataque();
        Dash();
        Shield();

        // Animaciones 
        Animator.SetBool("Attacking", attacking);
        Animator.SetBool("TakeDamage", takeDamage);
        Animator.SetBool("WithShield", withShield);
    }

    // Movimiento y Salto
    public void Movimiento()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        if (Horizontal < 0.0f) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (Horizontal > 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        Animator.SetBool("Running", Horizontal != 0.0f && Animator.GetBool("chargingJump") == false);

        Debug.DrawRay(transform.position, Vector3.down * 0.4f, Color.red);

        if (Physics2D.Raycast(transform.position, Vector3.down, 0.4f))
        {
            Grounded = true;
            Rigidbody2D.sharedMaterial = normalMat;
            Animator.SetBool("jumping", false);
        }
        else
        {

            Animator.SetBool("chargingJump", false);
            Grounded = false;
            Rigidbody2D.sharedMaterial = bounceMat;
            Animator.SetBool("jumping", true);
        }

        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;
        collider.enabled = true;

        if (Input.GetKey("space") && Grounded && canJump)
        {

            jumpValue += 0.1f;
        }

        if (jumpValue >= 5f && Grounded)
        {
            float tempx = Horizontal * Speed;
            float tempy = jumpValue;
            Rigidbody2D.velocity = new Vector2(tempx, tempy);
            Invoke("ResetJump", 0.2f);
        }

        if (Input.GetKeyDown("space") && Grounded && canJump)
        {
            Animator.SetBool("Running", false);
            Animator.SetBool("chargingJump", true);
            Rigidbody2D.velocity = new Vector2(0.0f, Rigidbody2D.velocity.y);
        }

        if (Input.GetKeyUp("space"))
        {
            Animator.SetBool("chargingJump", false);
            Animator.SetBool("jumping", true);
            Animator.SetBool("Running", Horizontal != 0.0f && Animator.GetBool("jumping") == false);
            if (Grounded)
            {
                Rigidbody2D.velocity = new Vector2(Horizontal * Speed, jumpValue);
                jumpValue = 0.0f;
            }
            canJump = true;
        }
    }

    void ResetJump()
    {
        canJump = false;
        jumpValue = 0;
    }

    // Ataque

    public void Ataque()
    {
        if (Input.GetKey(KeyCode.Q) && !attacking && Grounded)
        {
            Attacking();
        }
    }

    public void Attacking()
    {
        attacking = true;
    }
    public void DesactivatingAttack()
    {
        attacking = false;
    }

    // Recibir Daño

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // Para recibir daño debe estar habilitado el daño y no tener el escudo puesto
        if (collision.gameObject.CompareTag("Enemy") && !takeDamage && !withShield)
        {
            Debug.Log("Llego primero al collision de MovimientoPersonaje");

            takeDamage = true;

            Animator.SetBool("TakeDamage", true); // Activamos la animación de daño

            float damage = collision.gameObject.GetComponent<enemiesController>().Danyo;

            gameObject.GetComponent<Vida>().TakeDamage(damage);

            // Calculamos la dirección contraria a la que viene el daño
            Vector2 dashDirection = (transform.position - collision.transform.position).normalized;

            // Aplicamos el "dash" al personaje usando AddForce
            Rigidbody2D.AddForce(dashDirection*1.2f, ForceMode2D.Impulse); // Impulso instantáneo
        }

        // Si tiene puesto el escudo solo hara el dash para atras

        else if (collision.gameObject.CompareTag("Enemy") && withShield)
        {
            Vector2 dashDirection = (transform.position - collision.transform.position).normalized;

            // Aplicamos el "dash" al personaje usando AddForce
            Rigidbody2D.AddForce(dashDirection * 1.2f, ForceMode2D.Impulse); // Impulso instantáneo

            StartCoroutine(DetenerDespuesDeGolpe());
        }
    }

    private IEnumerator DetenerDespuesDeGolpe()
    {
        yield return new WaitForSeconds(0.2f); // Espera 0.2 segundos (puedes ajustar el tiempo)

        Rigidbody2D.velocity = Vector2.zero; // Detiene al personaje completamente
    }


    public void DesactivateDamage()
    {
        takeDamage = false; // Permite que el personaje se mueva de nuevo
        Animator.SetBool("TakeDamage", false); // Desactiva la animación de daño
    }

    // Put the Shield

    public void Shield()
    {
        if (Input.GetKeyDown(KeyCode.E) && !takeDamage && Grounded && !isDashing)
        {
            withShield = true;
        }
        if (Input.GetKeyUp(KeyCode.E) && !takeDamage && Grounded && !isDashing)
        {
            withShield = false;

        }
    }

    // Dash
    public void Dash()
    {
        if (Input.GetKeyDown(KeyCode.F) && !takeDamage && Grounded && !isDashing) // Solo ejecutar cuando se presiona F y no estamos ya en un dash
        {
            Animator.SetBool("dashing", true);
            isDashing = true;

            // Congelar la posición vertical
            Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;

            Debug.Log("Entró en la función del dash");

            // Calculamos la dirección del dash según la dirección en la que está mirando el personaje
            Vector2 dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            // Limpiamos la velocidad horizontal antes de aplicar el dash
            Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y); // Mantiene la velocidad vertical

            // Aplicamos el "dash" usando AddForce con una mayor fuerza
            Rigidbody2D.AddForce(dashDirection * 2f, ForceMode2D.Impulse); // Fuerza mayor para el dash

             // Activamos la bandera de dash
            Debug.Log("Dash realizado con fuerza.");

        }
    }
    
    public void TerminarDash()
    {
        Animator.SetBool("dashing", false);
        isDashing = false;
        Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // Muerte

    public void Muerte()
    {
        Animator.SetBool("Dead", true);
    }

    public void CambiarEscenaAMuerte()
    {
        SceneManager.LoadScene(2);
    }

    public void DestroyPlayer()
    {
        Destroy(gameObject);  // Esto destruye el GameObject al que está asociado este script, se ejecuta cuando el prota muere
    }


    // Mas Movimiento

    private void FixedUpdate()
    {
        if (isDashing)
        {
            // Si estamos en medio de un dash, mantenemos la velocidad del Rigidbody2D constante
            Rigidbody2D.velocity = new Vector2(Rigidbody2D.velocity.x, Rigidbody2D.velocity.y); // Mantiene solo el movimiento horizontal
        }
        else
        {
            // Movimiento horizontal normal si no estamos en medio de un dash
            if (!withShield && !takeDamage && jumpValue == 0.0f && Grounded)
            {
                Rigidbody2D.velocity = new Vector2(Horizontal * Speed, Rigidbody2D.velocity.y);
            }
        }
    }

}
