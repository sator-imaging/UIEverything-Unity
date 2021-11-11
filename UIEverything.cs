using UnityEngine;
using UnityEngine.UI;


public class UIEverything : MonoBehaviour
{
    public bool initializeOnStart = false;

    public Button button;
    public Slider slider;
    public Toggle toggle;
    public InputField inputField;
    //[Space]
    public Scrollbar scrollbar;
    public Dropdown dropdown;
    public ScrollRect scrollRect;



    void Reset() => Initialize();
    void Start()
    {
        if (initializeOnStart) Initialize();
    }

    void Initialize()
    {
        button = GetComponent<Button>();
        slider = GetComponent<Slider>();
        toggle = GetComponent<Toggle>();
        inputField = GetComponent<InputField>();
        scrollbar = GetComponent<Scrollbar>();
        dropdown = GetComponent<Dropdown>();
        scrollRect = GetComponent<ScrollRect>();
    }




    public void Synchronize_Slider_to_Text(Text targetText)
    {
        if (targetText && slider)
            targetText.text = slider.value.ToString("F2");

    }

    public void Synchronize_Slider_to_InputField(InputField targetInputField)
    {
        if (targetInputField && slider)
            targetInputField.text = slider.value.ToString();
        //Debug.Log($"{nameof(SyncSliderToInputField)}: Loop Check.");
    }

    public void Synchronize_InputField_to_Slider(Slider targetSlider)
    {
        if (targetSlider && inputField)
        {
            float value;
            if (float.TryParse(inputField.text, out value))
            {
                targetSlider.value = value;
            }
        }
        //Debug.Log($"{nameof(SyncInputFieldToSlider)}: Loop Check.");
    }




    public void OnSliderChanged(GameObject target)
    {
        if (target && slider)
            target.SendMessage(nameof(OnSliderChanged), slider, SendMessageOptions.DontRequireReceiver);
    }

    public void OnButtonClick(GameObject target)
    {
        if (target && button)
            target.SendMessage(nameof(OnButtonClick), button, SendMessageOptions.DontRequireReceiver);
    }

    public void OnToggleChanged(GameObject target)
    {
        if (target && toggle)
            target.SendMessage(nameof(OnToggleChanged), toggle, SendMessageOptions.DontRequireReceiver);
    }

    public void OnInputFieldChanged(GameObject target)
    {
        if (target && inputField)
            target.SendMessage(nameof(OnInputFieldChanged), inputField, SendMessageOptions.DontRequireReceiver);
    }

    public void OnInputFieldChangedInteractive(GameObject target)
    {
        if (target && inputField)
            target.SendMessage(nameof(OnInputFieldChangedInteractive), inputField, SendMessageOptions.DontRequireReceiver);
    }

    public void OnScrollbarChanged(GameObject target)
    {
        if (target && scrollbar)
            target.SendMessage(nameof(OnScrollbarChanged), scrollbar, SendMessageOptions.DontRequireReceiver);
    }

    public void OnDropdownChanged(GameObject target)
    {
        if (target && dropdown)
            target.SendMessage(nameof(OnDropdownChanged), dropdown, SendMessageOptions.DontRequireReceiver);
    }

    public void OnScrollRectChanged(GameObject target)
    {
        if (target && scrollRect)
            target.SendMessage(nameof(OnScrollRectChanged), scrollRect, SendMessageOptions.DontRequireReceiver);
    }


}//class
