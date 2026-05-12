using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    public float speed = 5;
    private Rigidbody2D rb;

    private float move;

    public float jumpForce = 4;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer; // Capa que representa el suelo para la comprobación de si el jugador está tocando el suelo

    private Animator animator;

    private int coins;
    public TMP_Text textCoins;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       move = Input.GetAxisRaw("Horizontal"); // Coger teclas definidas de Unity en horizontal (A y D o flechas)
        rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
        if(move != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(move),1,1); // Cambia la escala del jugador para que mire a la dirección en la que se mueve
        }

        if(Input.GetButtonDown("Jump") && isGrounded) // Si se pulsa el botón de salto y el jugador está en el suelo
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Añade una fuerza hacia arriba para saltar
        }

        animator.SetFloat("Speed", Mathf.Abs(move)); // Actualiza el parámetro de velocidad en el Animator para cambiar la animación del jugador dependiendo de si se mueve o no
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y); // Actualiza el parámetro de velocidad vertical en el Animator para cambiar la animación del jugador dependiendo de si está subiendo o bajando al saltar
        animator.SetBool("IsGrounded", isGrounded); // Actualiza el parámetro de si el jugador está en el suelo o no en el Animator para cambiar la animación del jugador dependiendo de si está en el suelo o no
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer); // Comprueba si el jugador está tocando el suelo
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin")) // Si el jugador colisiona con la moneda
        {
            Destroy(collision.gameObject); // Destruye la moneda
            coins++;
            textCoins.text = coins.ToString()
                ; // Actualiza el texto de las monedas recogidas
        }

        if (collision.CompareTag("Spikes")) // Si el jugador colisiona con los pinchos
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reinicia la escena actual para que el jugador vuelva a empezar desde el principio
        }
    }
}
