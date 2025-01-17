using UnityEngine;
using MixedReality.Toolkit.UX;

public class SliderControl : MonoBehaviour
{
    [SerializeField] private GameObject graySlider;  // The gray background slider (fixed size)
    [SerializeField] private GameObject whiteSlider; // The white slider (moves based on value)
    [SerializeField] private int minValue = 0;       // Minimum value
    [SerializeField] private int maxValue = 1;       // Maximum value
    [SerializeField] public float currentValue = 0f; // Current value (normalized between 0 and 1)

    private float sliderWidth; // The width of the gray slider (scaled)
    private float sliderHeight; // The height of the gray slider (scaled)

    void Start()
    {
        // Get the width and height of the gray slider from its local scale
        sliderWidth = graySlider.transform.localScale.x;
        sliderHeight = graySlider.transform.localScale.y;

        // Initial positioning of the white slider based on the current value
        UpdateSliderPosition();
    }

    void Update()
    {
        // Update the slider position based on the current value (this is typically updated by user interaction)
        UpdateSliderPosition();
    }

    private void UpdateSliderPosition()
    {
        // Clamp the current value between min and max
        currentValue = Mathf.Clamp(currentValue, minValue, maxValue);

        // Calculate the new position for the white slider based on the current value
        // normalized to the width of the gray slider
        float xPos = (currentValue - minValue) / (maxValue - minValue) * sliderWidth;

        // Update the position of the white slider (move it along the X-axis)
        // The gray slider's center is at 0,0, so -sliderWidth / 2 positions the left edge at the origin
        whiteSlider.transform.localPosition = new Vector3(-sliderWidth / 2 + xPos, whiteSlider.transform.localPosition.y, whiteSlider.transform.localPosition.z);

        // Optionally, scale the white slider in the Y direction based on the gray slider's height
        whiteSlider.transform.localScale = new Vector3(whiteSlider.transform.localScale.x, sliderHeight, whiteSlider.transform.localScale.z);
    }

    // You can expose this method to set the value externally (for example, via interactions)
    public void SetValue(float value)
    {
        currentValue = value;
        UpdateSliderPosition();
    }
}
