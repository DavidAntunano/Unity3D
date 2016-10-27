using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class MenuCheckboxBase : MonoBehaviour
{
    public enum CheckboxTypes {Vibration = 0}
    public CheckboxTypes checkboxType;
    public delegate void OnClickHandler(MenuCheckboxBase sender);
    public event OnClickHandler OnCheckboxClicked;

    Toggle toggle;

    void Awake() {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnCheckboxClick);
    }

    protected virtual void OnCheckboxClick(bool Status) {
        if (OnCheckboxClicked != null)
        {
            OnCheckboxClicked(this);
        }
    }
}