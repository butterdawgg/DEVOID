using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string defaultText = "button";
    [SerializeField] private string highlightedText = "> button <";
    [SerializeField] private int defaultFontSize = 100;
    [SerializeField] private int highlightedFontSize = 120;
    [SerializeField] private Color defaultTextColor = Color.gray;
    [SerializeField] private Color highlightedTextColor = Color.white;

    public bool IsSelectable { get; set; } = true;

    private TextMeshProUGUI text;
    private Button button;

    private bool isMouseOver = false;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void Update()
    {
        if (!IsSelectable)
        {
            text.text = defaultText;
            text.fontSize = defaultFontSize;
            text.color = defaultTextColor;

            return;
        }

        if (isMouseOver)
        {
            text.text = highlightedText;
            text.fontSize = highlightedFontSize;
            text.color = highlightedTextColor;
        }
        else
        {
            text.text = defaultText;
            text.fontSize = defaultFontSize;
            text.color = defaultTextColor;
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isMouseOver = true;

        if (!IsSelectable)
            return;

        AudioManager.Instance.PlaySound("ButtonHover");
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isMouseOver = false;
    }

    private void OnButtonClick()
    {
        isMouseOver = false;

        text.text = defaultText;
        text.fontSize = defaultFontSize;
        text.color = defaultTextColor;

        AudioManager.Instance.PlaySound("ButtonClick");
    }
}