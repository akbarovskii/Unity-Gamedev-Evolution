using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    [Header("Настройки")]
    public float energyAmount = 25f;

    [Header("Эффекты")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;

    private FlashlightSystem flashlight;
    private bool isPickupAllowed = true;
    private AudioSource audioSourceParent;
    private GameObject parentObject;

    void Start()
    {
        flashlight = FindObjectOfType<FlashlightSystem>();
        parentObject = transform.parent.gameObject;
        audioSourceParent = parentObject.GetComponent<AudioSource>();

        if (flashlight == null)
        {
            Debug.LogError("FlashlightSystem не найден в сцене!");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isPickupAllowed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    PickupBattery();
                }
            }
        }
    }

    void PickupBattery()
    {
        isPickupAllowed = false;

        if (flashlight != null)
        {
            flashlight.AddEnergy(energyAmount);
            Debug.Log($"+{energyAmount} энергии");
        }

        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        if (audioSourceParent != null && pickupSound != null)
        {
            audioSourceParent.PlayOneShot(pickupSound);
            Destroy(parentObject, pickupSound.length);
        }

        Destroy(gameObject);
    }

    void OnMouseEnter()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.yellow;
        }
    }

    void OnMouseExit()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.white;
        }
    }
}