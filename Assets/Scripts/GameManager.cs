using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    const int LIVES = 3;
    
    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI txtScore;
    [SerializeField] TextMeshProUGUI txtMaxScore; 
    [SerializeField] TextMeshProUGUI txtMessage; 
    [SerializeField] GameObject[] imgLives; // Arrastra aquí las 3 imágenes de las naves
 
    private int score = 0;
    private int maxScore = 0; 
    private int lives = LIVES; 
    private int nextExtraLifeScore = 1000; // Control para la vida extra cada 1000 puntos

    private static GameManager instance; 

    // Método estático para obtener la instancia del GameManager
    public static GameManager GetInstance(){
        return instance;
    }

    void Awake() {
        // Singleton: garantiza que solo haya un GameManager y no se destruya entre escenas
        if(instance == null){
            instance = this;    
            DontDestroyOnLoad(gameObject);
        } else if(instance != this) {
            Destroy(gameObject);
        }

        // CARGAR RECORD: Recuperamos la puntuación máxima guardada anteriormente
        maxScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void Start() {
        // Inicializar textos en pantalla al empezar
        txtScore.text = string.Format("{0,4:D4}", score);
        txtMaxScore.text = string.Format("{0,4:D4}", maxScore);
        txtMessage.text = "";
        ActualizarIconosVidas();
    }

    // El sistema del tutorial usa OnGUI para refrescar la interfaz continuamente
    private void OnGUI() {
        ActualizarIconosVidas();
        txtScore.text = string.Format("{0,4:D4}", score);
    } 

    // Método para actualizar visualmente las imágenes de las naves
    private void ActualizarIconosVidas() {
        for(int i = 0; i < imgLives.Length; i++) {
            // Si el índice es menor que las vidas actuales, la imagen se activa
            imgLives[i].SetActive(i < lives); 
        }
    }

    // Método para añadir puntos (Corregido HighScore y Vida Extra)
    public void AddScore(int points) {
        score += points;

        // MEJORA 1: Puntuación máxima persistente
        if (score > maxScore) {
            maxScore = score;
            txtMaxScore.text = string.Format("{0,4:D4}", maxScore);
            // Guardamos el record en el disco
            PlayerPrefs.SetInt("HighScore", maxScore);
        }

        // MEJORA 2: Vida Extra (Lógica de umbral más segura que el %)
        if (score >= nextExtraLifeScore) {
            AddLife();
            nextExtraLifeScore += 1000; // Próxima vida a los siguientes 1000
        }
    }

    // Método para añadir una vida
    public void AddLife() {
        if (lives < LIVES) { 
            lives++;
            txtMessage.text = "¡VIDA EXTRA!";
            Invoke("ClearMessage", 2f);
        }
    }

    // Método para restar una vida (Llamado desde ShipController)
    public void LoseLife() {
        if (lives > 0) {
            lives--; 
            txtMessage.text = "¡NAVE DESTRUIDA!";
            Invoke("ClearMessage", 1.5f);
        }

        if (lives <= 0) {
            txtMessage.text = "GAME OVER";
            Time.timeScale = 0; // Opcional: congela el juego al morir
        }
    }

    void ClearMessage() {
        txtMessage.text = "";
    }
}