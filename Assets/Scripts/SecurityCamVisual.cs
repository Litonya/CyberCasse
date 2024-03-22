using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamVisual : MonoBehaviour
{
    private bool _isBlinking = false;
    private bool _lightOn = true;

    [SerializeField] private float _blinskingTime = 1f;
    private float currentTime;

    [SerializeField] private Color onColor = Color.red;
    [SerializeField] private Color offColor = Color.green;

    private Renderer _renderer;
    private Light _light;



    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _light = GetComponent<Light>();
        currentTime = _blinskingTime;
    }

    private void Update()
    {
        if (_isBlinking)
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0) Blink();
        }
    }

    private void Blink()
    {
        currentTime = _blinskingTime;
        if (_lightOn)
        {
            _renderer.material.color = Color.black;
            _light.enabled = false;
            _lightOn = false;
            return;
        }
        _renderer.material.color = offColor;
        _light.enabled = true;
        _lightOn = true;
    }

    public void ActivateLight()
    {
        _renderer.material.color = onColor;
        _light.enabled = true;
        _lightOn = true;
        _isBlinking = false;
    }

    public void DeactivateLight()
    {
        _renderer.material.color = offColor;
        _light.enabled = true;
        _lightOn = true;
        _isBlinking = true;
    }
}
