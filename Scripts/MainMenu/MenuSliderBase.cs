using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(Slider))]
public class MenuSliderBase : MonoBehaviour, IEndDragHandler
{
    public enum SliderTypes { musicVolume = 0, effectVolume = 1 }
    public SliderTypes sliderType;
    public delegate void OnSliderHandler(MenuSliderBase sender);
    public event OnSliderHandler OnSliderDragged;

    Slider slider;
    AudioSource audioSource;

    void Awake() {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderDrag);
        audioSource = GetComponentInChildren<AudioSource>();
    }

    protected virtual void OnSliderDrag(float sliderValue) {
        if (OnSliderDragged != null)
        {
            OnSliderDragged(this);
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        audioSource.volume = slider.value / 100;
        audioSource.PlayDelayed(0.5f);
    }
}