using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PlayerCamera : MonoBehaviour
{
    private Vector2 mouseLook = Vector2.zero, smoothV = Vector2.zero;

    [SerializeField]
    private float sensitivity = 5.0f, smoothing = 3f;

    public Vector3 posComparedToPlayer = Vector3.zero;

    private Transform player = null, gun = null;

    private PostProcessingBehaviour postProcessing = null;

    public bool camShake { private get; set; } = false;

    private void Awake()
    {
        postProcessing = GetComponent<PostProcessingBehaviour>();
    }

    // Follow player around/ rotate player & gun & camera based on mouse movement
    private void Update()
    {
        if (!player) { return; }

        Vector2 md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        md = Vector2.Scale(md, new Vector2(sensitivity * smoothing, sensitivity * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -40f, 80f);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        gun.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        player.localRotation = Quaternion.AngleAxis(mouseLook.x, player.up);

        // Shake camera if cam shake enabled
        if (camShake)
        {
            ShakeCam(0.005f, posComparedToPlayer);
        } else
        {
            transform.localPosition = posComparedToPlayer;
        }
    }

    // Set the camera ready for the game state
    public void SetCameraPosToPlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;

        player = FindObjectOfType<PlayerMovement>().transform;
        gun = player.GetChild(0);
        transform.SetParent(player);
        transform.localPosition = posComparedToPlayer;

        enabled = true;
        postProcessing.enabled = true;

        RenderSettings.fog = true;
    }

    // Shake camera from given pos
    public void ShakeCam(float magnitude, Vector3 pos)
    {
        float x = Random.Range(-1f, 1f) * magnitude;
        float y = Random.Range(-1f, 1f) * magnitude;
        float z = Random.Range(-1f, 1f) * magnitude;

        transform.localPosition = new Vector3(pos.x + x, pos.y + y, pos.z + z);
    }

    // Turn of post processing
    private void OnDisable()
    {
        postProcessing.enabled = false;
    }
}
