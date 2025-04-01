using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class LoginManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Button goToRegisterButton;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float sceneChangeDelay = 1f;

    [Header("Colors")]
    [SerializeField] private Color errorColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color successColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color loadingColor = Color.yellow;

    [Header("Settings")]
    [SerializeField] private string loginEndpoint = "http://localhost:4000/api/users/login";
    [SerializeField] private string successScene = "MainScene";
    [SerializeField] private string registerScene = "RegisterScene";

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void Start()
    {
        // Configuración inicial
        passwordInputField.contentType = TMP_InputField.ContentType.Password;
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        
        if (goToRegisterButton != null)
        {
            goToRegisterButton.onClick.AddListener(() => 
            {
                StartCoroutine(TransitionToScene(registerScene));
            });
        }

        // Animación de entrada
        LeanTween.alphaCanvas(canvasGroup, 1f, fadeDuration);

        // Auto-relleno para testing (opcional)
        #if UNITY_EDITOR
        emailInputField.text = "test@example.com";
        passwordInputField.text = "password123";
        #endif
    }

    private void OnLoginButtonClicked()
    {
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text;

        if (!ValidateInputs(email, password))
            return;

        StartCoroutine(LoginUser(email, password));
    }

    private bool ValidateInputs(string email, string password)
    {
        if (string.IsNullOrEmpty(email))
        {
            ShowFeedback("Email cannot be empty", errorColor);
            return false;
        }

        if (!IsValidEmail(email))
        {
            ShowFeedback("Invalid email format", errorColor);
            return false;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowFeedback("Password cannot be empty", errorColor);
            return false;
        }

        return true;
    }

    private IEnumerator LoginUser(string email, string password)
    {
        loginButton.interactable = false;
        ShowFeedback("Signing in...", loadingColor);

        // Preparar datos
        UserLoginData loginData = new UserLoginData
        {
            email = email,
            password = password
        };

        string jsonData = JsonUtility.ToJson(loginData);

        // Crear petición
        using (UnityWebRequest request = new UnityWebRequest(loginEndpoint, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Enviar petición
            yield return request.SendWebRequest();

            // Manejar respuesta
            if (request.result == UnityWebRequest.Result.Success)
            {
                HandleLoginSuccess(request);
            }
            else
            {
                HandleLoginError(request);
                loginButton.interactable = true;
            }
        }
    }

    private void HandleLoginSuccess(UnityWebRequest request)
    {
        LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
        
        if (response != null && response.user != null && !string.IsNullOrEmpty(response.token))
        {
            UserDataHolder.Instance.SetUserData(response.user, response.token);
            ShowFeedback($"Welcome {response.user.name}!", successColor);
            StartCoroutine(TransitionToScene(successScene));
        }
        else
        {
            ShowFeedback("Invalid server response", errorColor);
            loginButton.interactable = true;
        }
    }

    private void HandleLoginError(UnityWebRequest request)
    {
        string errorMessage = request.responseCode switch
        {
            400 => "Invalid email or password",
            401 => "Unauthorized access",
            404 => "Server not found",
            500 => "Internal server error",
            _ => $"Connection failed: {request.error}"
        };
        
        ShowFeedback(errorMessage, errorColor);
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        // Animación de salida
        LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        
        // Cargar escena
        SceneManager.LoadScene(sceneName);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private void ShowFeedback(string message, Color color)
    {
        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);
        
        // Ocultar después de 3 segundos
        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), 3f);
    }

    private void HideFeedback()
    {
        if (feedbackText != null)
        {
            LeanTween.alphaCanvas(feedbackText.GetComponent<CanvasGroup>(), 0f, 0.5f)
                .setOnComplete(() => feedbackText.gameObject.SetActive(false));
        }
    }

    [System.Serializable]
    private class UserLoginData
    {
        public string email;
        public string password;
    }

    [System.Serializable]
    private class LoginResponse
    {
        public UserData user;
        public string token;
    }
}