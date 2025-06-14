using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClassSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float defaultScale = 1f;
    [SerializeField] private float highlightedScale = 1.2f;
    [SerializeField] private int playerClassID = 0;

    private Button button;
    private bool isMouseOver = false;
    private RectTransform rectT;
    private Vector3 initialScale;

    private void Awake()
    {
        rectT = GetComponent<RectTransform>();
        initialScale = rectT.localScale;

        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    private void Update()
    {
        if (isMouseOver)
        {
            rectT.localScale = new Vector3(initialScale.x * highlightedScale,
                initialScale.y * highlightedScale, 1f);
        }
        else
        {
            rectT.localScale = new Vector3(initialScale.x * defaultScale,
                initialScale.y * defaultScale, 1f);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isMouseOver = true;

        AudioManager.Instance.PlaySound("ButtonHover");
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isMouseOver = false;
    }

    private void OnButtonClick()
    {
        isMouseOver = false;

        rectT.localScale = new Vector3(initialScale.x * defaultScale,
                initialScale.y * defaultScale, 1f);

        SerializeManager.SetPlayerClassID(playerClassID);

        AudioManager.Instance.PlaySound("ButtonClick");
    }
}