
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkButtons : MonoBehaviour
{
    private void Awake()
    {
        RegisterButton("Button_Host", StartHost);
        RegisterButton("Button_Client", StartClient);
    }

    private void RegisterButton(string buttonName, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = GameObject.Find(buttonName);

        if (buttonObject == null)
        {
            Debug.LogError($"No se encuentra el objeto '{buttonName}' en la escena.");
            return;
        }

Button button = buttonObject.GetComponent<Button>()
    ?? buttonObject.GetComponentInParent<Button>()
    ?? buttonObject.GetComponentInChildren<Button>(true);
    
        if (button == null)
        {
            Debug.LogError($"El objeto '{buttonName}' no tiene el componente Button.");
            return;
        }

        button.onClick.AddListener(action);
        Debug.Log($"Botón '{buttonName}' conectado correctamente.");
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
