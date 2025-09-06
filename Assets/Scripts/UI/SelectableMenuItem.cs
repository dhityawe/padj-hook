using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectableMenuItem : MonoBehaviour
{
    [SerializeField] private RectTransform _indicator;
    [SerializeField] private Vector3 _offset = new(-150f, 0f);
    private EventSystem eventSystem;
    private GameObject lastSelected;
    private Vector3 selectedPosition;

    void Start()
    {
        eventSystem = EventSystem.current;
        lastSelected = eventSystem.firstSelectedGameObject;
        eventSystem.SetSelectedGameObject(lastSelected);
        selectedPosition = lastSelected.GetComponent<RectTransform>().position;
        SetIndicator(selectedPosition);
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            lastSelected = eventSystem.currentSelectedGameObject;
            selectedPosition = lastSelected.GetComponent<RectTransform>().position;
        }
        else if (lastSelected != null)
        {
            eventSystem.SetSelectedGameObject(lastSelected);
            selectedPosition = lastSelected.GetComponent<RectTransform>().position;
        }
        SetIndicator(selectedPosition);
    }

    private void SetIndicator(Vector3 position)
    {
        _indicator.position = position + _offset;
        _indicator.gameObject.SetActive(true);
    }
}
