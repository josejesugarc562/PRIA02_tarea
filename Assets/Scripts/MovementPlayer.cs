
using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class MovementPlayer : NetworkBehaviour
{
    [Header("Movimiento")]
    public float moveForce = 5f;
    public float maxSpeed = 50f; // Velocidad máxima editable en Inspector

    private Rigidbody rb;
    private Renderer render;

    // Se ejecuta cuando el jugador se incorpora a la red.
    // El servidor establece su posición aleatoria.
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-5f, 5f),
                2f,
                Random.Range(-5f, 5f)
            );

            transform.position = randomPos;
        }
    }

    // Inicializa las referencias al Rigidbody y al Renderer.
    // También limita los FPS para reducir el uso de recursos.
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        render = GetComponent<Renderer>();

        // Para que no salten los ventiladores.
        Application.targetFrameRate = 30;
    }

    // Lee el movimiento del jugador propietario y envía
    // la dirección al servidor para que aplique la fuerza.
    void FixedUpdate()
    {
        if (!IsOwner) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(h, 0, v);

        MoveRequestRpc(direction);
    }

    // RPC: recibe la petición de movimiento y la ejecuta
    // en el servidor, limitando la velocidad máxima.
    [Rpc(SendTo.Server)]
    private void MoveRequestRpc(Vector3 direction)
    {
        rb.AddForce(direction * moveForce, ForceMode.Force);

        // Limitar la velocidad máxima del jugador.
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        // Actualizar la posición del jugador en los clientes.
        UpdatePositionClientRpc(transform.position);
    }

    // RPC: envía la posición calculada por el servidor
    // a todos los clientes y al propio Host.
    [Rpc(SendTo.ClientsAndHost)]
    private void UpdatePositionClientRpc(Vector3 position)
    {
        transform.position = position;
    }

    // RPC: el servidor genera un color aleatorio
    // para el personaje que ha solicitado el cambio.
    [Rpc(SendTo.Server)]
    public void ChangeColorRandomRpc()
    {
        Color randomColor = new Color(
            Random.value,
            Random.value,
            Random.value
        );

        ChangeColorClientRpc(randomColor);
    }

    // RPC: aplica el color recibido al personaje correspondiente
    // en todos los jugadores conectados.
    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeColorClientRpc(Color newColor)
    {
        if (render != null)
            render.material.color = newColor;
    }
}