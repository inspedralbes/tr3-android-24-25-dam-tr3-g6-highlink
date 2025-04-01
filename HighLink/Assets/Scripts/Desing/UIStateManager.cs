using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIStateManager : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] private Button logoutButton;    // Button 5
    [SerializeField] private Button loginButton;     // Button 2
    [SerializeField] private Button registerButton;  // Button 1

    [Header("Texto de Bienvenida")]
    [SerializeField] private TMP_Text welcomeText;   // Crea uno nuevo si no existe

    [Header("Opciones")]
    [SerializeField] private float logoutDelay = 0.3f;

    private void Start()
    {
        // Configura listeners
        if (logoutButton != null) logoutButton.onClick.AddListener(OnLogout);
        
        // Actualizar estado inicial
        UpdateUI();
        
        // Suscribirse a cambios de sesión
        if (UserDataHolder.Instance != null)
        {
            UserDataHolder.Instance.OnSessionChanged += UpdateUI;
        }
    }

    private void UpdateUI()
    {
        bool isLoggedIn = UserDataHolder.Instance != null && UserDataHolder.Instance.IsLoggedIn;

        // Mostrar/ocultar elementos
        if (logoutButton != null) logoutButton.gameObject.SetActive(isLoggedIn);
        if (loginButton != null) loginButton.gameObject.SetActive(!isLoggedIn);
        if (registerButton != null) registerButton.gameObject.SetActive(!isLoggedIn);

        // Mensaje de bienvenida
        if (welcomeText != null)
        {
            welcomeText.gameObject.SetActive(isLoggedIn);
            if (isLoggedIn) welcomeText.text = $"¡Hola, {UserDataHolder.Instance.CurrentUser.name}!";
        }
    }

    private void OnLogout()
    {
        if (UserDataHolder.Instance != null) UserDataHolder.Instance.ClearSession();
        UpdateUI(); // Actualizar inmediatamente
    }
}