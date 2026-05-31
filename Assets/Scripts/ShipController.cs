using UnityEngine;
using System.Collections;
public class ShipController : MonoBehaviour
{
    [SerializeField] int blinkNum;
    [SerializeField] GameObject shootPrefab;

    // Distancia desde el centro de la nave hasta la posición donde se creará el disparo
    [SerializeField] float shootOffset = 0.5f;

    [SerializeField] private Vector3 endPosition; // Posición final de la nave al inicio
    [SerializeField] private float duration;
    [SerializeField] GameObject explosion;
    Vector3 initialPosition; // Duración de la transición al inicio
    private bool active = true;

    [SerializeField] private float force = 0.02f; // Fuerza del movimiento

    private Rigidbody2D rb; // Referencia al componente Rigidbody

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;

        StartCoroutine("StartPlayer");

        // Inicializamos la referencia al Rigidbody
    }


    IEnumerator StartPlayer()
    {
        Material mat = GetComponent<SpriteRenderer>().material;
        Color color = mat.color;
        Collider2D collider = GetComponent<Collider2D>();
        collider.enabled = false;
        Vector3 initialPosition = transform.position;
        float t = 0, t2 = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            Vector3 newPosition = Vector3.Lerp(initialPosition, endPosition, t / duration);
            transform.position = newPosition;

            t2 += Time.deltaTime;
            float newAlpha = blinkNum * (t2 / duration);
            if (newAlpha > 1)
            {
                t2 = 0;
            }
            color.a = newAlpha;
            mat.color = color;
            yield return null;
        }
        color.a = 1;
        mat.color = color;
        collider.enabled = true;
        active = true;



    }

    private void FixedUpdate()
    {
        if (active)
            CheckMove();
    }

    private void CheckMove()
    {
        // Obtenemos la dirección del movimiento en los ejes horizontal y vertical
        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        direction.Normalize(); // Normalizamos el vector para que tenga magnitud 1

        // Aplicamos una fuerza en la dirección obtenida
        rb.AddForce(direction * force, ForceMode2D.Impulse);
    }
    void Update()
    {
        // Comprobar si la nave está activa y se ha pulsado la tecla de disparo (barra espaciadora)
        if (active && Input.GetKeyDown(KeyCode.Space))
        {
            // Calcular la posición donde se creará el disparo (un poco por delante de la nave)
            Vector3 shootPosition = transform.position + Vector3.up * shootOffset;

            // Crear el disparo en la posición calculada y sin rotación
            Instantiate(shootPrefab, shootPosition, Quaternion.identity);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "AsteroidBig")
        {
            DestroyShip();

        }
    
    }
    void DestroyShip()
    {
        // 1. Desactivar comportamiento para que no pueda moverse ni disparar mientras muere
        active = false;

        // 2. Instanciar la explosión
        Instantiate(explosion, transform.position, Quaternion.identity);

        // --- 3. AÑADE ESTA LÍNEA AQUÍ ---
        // Avisamos al GameManager que reste una vida
        GameManager.GetInstance().LoseLife(); 
        // -------------------------------

        // 4. Resetear posición de la nave al punto de inicio
        transform.position = initialPosition;

        // 5. Reiniciar la nave (vuelve a entrar desde fuera y parpadea)
        StartCoroutine("StartPlayer");
    }

}
