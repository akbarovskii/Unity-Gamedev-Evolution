using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float openAngle = -90f;
    public float speed = 2f;
    public AudioClip doorSound; 

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isOpen = false;
    private AudioSource audioSource;
    private bool wasOpen = false;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

       
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

       
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; 
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isOpen = !isOpen;
                    PlayDoorSound();
                }
            }
        }

        if (isOpen)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, speed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, closedRotation, speed * Time.deltaTime);
        }
    }

    void PlayDoorSound()
    {
        if (doorSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(doorSound);
        }
    }
}
