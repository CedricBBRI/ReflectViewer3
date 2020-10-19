using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Links an InputField to a Slider
/// The Slider is expected to be on the same gameObject.
/// </summary>
[AddComponentMenu("Reflect/Template/Slider InputField Linker")]
[RequireComponent (typeof(Slider))]
public class SliderInputField : MonoBehaviour
{
    [SerializeField] InputField inputField = default;
    Slider slider;

    void Start()
    {
        if (inputField == null || inputField.contentType != InputField.ContentType.DecimalNumber)
            return;

        slider = GetComponent<Slider>();

        slider.onValueChanged.AddListener((v) =>
        {
            inputField.SetTextWithoutNotify(v.ToString());
        });

        inputField.onEndEdit.AddListener((v) =>
        {
            //slider.SetValueWithoutNotify(float.Parse(v));
            // setting the value with notification so that events on the slider get raised.
            // eventually coming back to the listener above to update the inputfield value.
            slider.value = Mathf.Clamp(float.Parse(v), slider.minValue, slider.maxValue);
        });
    }
}