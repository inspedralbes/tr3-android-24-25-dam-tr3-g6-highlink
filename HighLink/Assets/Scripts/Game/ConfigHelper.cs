using UnityEngine;

public static class ConfigHelper
{
    public static float GetFloat(string configName)
    {
        var config = LoadAndStoreJSON.GetConfig(configName);
        if (config != null && config.type == "float")
        {
            return config.value;
        }
        Debug.LogError($"Configuración inválida para float: {configName}");
        return 0f;
    }

    public static int GetInt(string configName)
    {
        var config = LoadAndStoreJSON.GetConfig(configName);
        if (config != null && config.type == "int")
        {
            return (int)config.value; // Conversión explícita de float a int
        }
        Debug.LogError($"Configuración inválida para int: {configName}");
        return 0;
    }

    public static string GetString(string configName)
    {
        var config = LoadAndStoreJSON.GetConfig(configName);
        if (config != null)
        {
            return config.value.ToString();
        }
        Debug.LogError($"Configuración no encontrada: {configName}");
        return string.Empty;
    }
}