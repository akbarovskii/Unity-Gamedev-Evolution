using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightSystem : MonoBehaviour
{
    [Header("Flashlight")]
    public Light flashlightLight;
    public float drainRate = 10f;
    public float defaultIntensity = 1.5f;

    [Header("UI")]
    public Image[] batteryIndicators;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip flashlightOnSound;
    public AudioClip flashlightOffSound;
    public AudioClip flashlightFlickerSound;

    [Header("Settings")]
    public float flickerSoundThreshold = 20f;
    public float minFlickerInterval = 0.5f;
    public float maxFlickerInterval = 2f;

    private float currentCharge = 100f;
    private bool isOn = true;
    private bool isFlickering = false;
    private float nextFlickerTime = 0f;
    private float targetIntensity = 1.5f;
    private float currentIntensity = 0f;
    private float originalIntensity;

    void Start()
    {
        if (flashlightLight != null)
        {
            originalIntensity = defaultIntensity;
            flashlightLight.intensity = isOn && currentCharge > 0 ? originalIntensity : 0f;
            targetIntensity = isOn && currentCharge > 0 ? originalIntensity : 0f;
            currentIntensity = targetIntensity;
            flashlightLight.enabled = isOn && currentCharge > 0;
        }
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ToggleFlashlight();
        }

        if (isOn && currentCharge > 0)
        {
            currentCharge -= drainRate * Time.deltaTime;

            if (currentCharge <= 0)
            {
                currentCharge = 0;
                isOn = false;
                targetIntensity = 0f;
                currentIntensity = 0f;
                flashlightLight.intensity = 0f;
                flashlightLight.enabled = false;

                if (audioSource != null && flashlightOffSound != null)
                    audioSource.PlayOneShot(flashlightOffSound);
            }

            UpdateUI();

            if (currentCharge <= flickerSoundThreshold && !isFlickering)
            {
                isFlickering = true;
                nextFlickerTime = Time.time + Random.Range(minFlickerInterval, maxFlickerInterval);
            }
            else if (currentCharge > flickerSoundThreshold && isFlickering)
            {
                isFlickering = false;
            }

            if (isFlickering && Time.time >= nextFlickerTime && isOn)
            {
                if (audioSource != null && flashlightFlickerSound != null)
                {
                    audioSource.pitch = Random.Range(0.8f, 1.2f);
                    audioSource.PlayOneShot(flashlightFlickerSound, 0.5f);
                }
                nextFlickerTime = Time.time + Random.Range(minFlickerInterval, maxFlickerInterval);
            }
        }

        if (flashlightLight != null)
        {
            currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * 15f);
            flashlightLight.intensity = currentIntensity;
        }
    }

    void ToggleFlashlight()
    {
        if (currentCharge <= 0) return;

        isOn = !isOn;

        if (audioSource != null)
        {
            if (isOn && flashlightOnSound != null)
                audioSource.PlayOneShot(flashlightOnSound);
            else if (!isOn && flashlightOffSound != null)
                audioSource.PlayOneShot(flashlightOffSound);
        }

        if (isOn)
        {
            targetIntensity = originalIntensity;
            flashlightLight.enabled = true;
        }
        else
        {
            targetIntensity = 0f;
            currentIntensity = 0f;
            flashlightLight.intensity = 0f;
            flashlightLight.enabled = false;
        }
    }

    public void AddEnergy(float amount)
    {
        currentCharge += amount;
        if (currentCharge > 100f)
            currentCharge = 100f;

        if (!isOn && currentCharge > 0)
        {
            isOn = true;
            targetIntensity = originalIntensity;
            flashlightLight.enabled = true;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        int filledCount = Mathf.CeilToInt(currentCharge / 25f);
        filledCount = Mathf.Clamp(filledCount, 0, 4);

        for (int i = 0; i < batteryIndicators.Length; i++)
        {
            if (batteryIndicators[i] != null)
                batteryIndicators[i].enabled = (i < filledCount);
        }
    }

    public float GetCurrentCharge() => currentCharge;
}
