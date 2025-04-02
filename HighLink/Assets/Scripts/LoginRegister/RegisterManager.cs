using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class RegisterManager : MonoBehaviour
{
    // Referencias a los InputFields (asignar desde el Inspector)
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField confirmPasswordInputField;

    // Referencia al botón de registro (asignar desde el Inspector)
    [Header("Buttons")]
    [SerializeField] private Button registerButton;

    private void Start()
    {
        // Configurar campos de contraseña
        passwordInputField.contentType = TMP_InputField.ContentType.Password;
        confirmPasswordInputField.contentType = TMP_InputField.ContentType.Password;

        // Asignar el evento al botón
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    private void OnRegisterButtonClicked()
    {
        // Obtener los valores de los campos
        string name = nameInputField.text.Trim();
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text;
        string confirmPassword = confirmPasswordInputField.text;

        // Validar campos vacíos
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("Error: El campo Nombre no puede estar vacío");
            return;
        }

        if (string.IsNullOrEmpty(email))
        {
            Debug.LogError("Error: El campo Email no puede estar vacío");
            return;
        }

        // Validar formato de email
        if (!IsValidEmail(email))
        {
            Debug.LogError("Error: El formato del email no es válido");
            return;
        }

        // Validar contraseña
        if (string.IsNullOrEmpty(password))
        {
            Debug.LogError("Error: El campo Contraseña no puede estar vacío");
            return;
        }

        if (password.Length < 8)
        {
            Debug.LogError("Error: La contraseña debe tener al menos 8 caracteres");
            return;
        }

        if (password != confirmPassword)
        {
            Debug.LogError("Error: Las contraseñas no coinciden");
            return;
        }

    
    }

    // Método para validar formato de email
    private bool IsValidEmail(string email)
    {
        try
        {
            // Patrón simple para validación de email
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}