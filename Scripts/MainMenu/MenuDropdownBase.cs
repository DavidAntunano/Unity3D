using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Dropdown))]
public class MenuDropdownBase : MonoBehaviour
{
    public enum DropdownTypes { QualitySetting = 0 }
    public DropdownTypes dropdownType;
    public delegate void OnClickHandler(MenuDropdownBase sender);
    public event OnClickHandler OnDropdownClicked;

    Dropdown dropdown;
    
    void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(OnDropdownClick);
    }

    protected virtual void OnDropdownClick(int valueSelected)
    {
        if (OnDropdownClicked != null)
        {
            OnDropdownClicked(this);
        }
    }
}