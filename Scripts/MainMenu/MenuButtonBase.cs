using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class MenuButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public enum ButtonTypes { NewGame = 0, Continue = 1, Options = 2, Credits = 3, Exit = 4, Back = 5, Resume = 6, ToMainMenu = 7}
    public ButtonTypes buttonType;
	public delegate void OnClickHandler(MenuButtonBase sender);
    public event OnClickHandler OnButtonClicked;
    
    Button button;
	Text text;

    void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        text = GetComponentInChildren<Text>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        text.color = Color.red;
    }

    public void OnPointerExit(PointerEventData eventData) {
		text.color = Color.white;
	}

    protected virtual void OnButtonClick() {
        if(OnButtonClicked != null) {
            OnButtonClicked(this);
        }
    }
}