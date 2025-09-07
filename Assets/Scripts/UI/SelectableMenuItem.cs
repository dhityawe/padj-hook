using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectableMenuItem : MonoBehaviour
{
    [SerializeField] private RectTransform _indicator;
    [SerializeField] private Vector3 _offset = new(-150f, 0f);
    [SerializeField] private Transform _mainMenuParent;
    private EventSystem eventSystem;
    private GameObject lastSelected;
    private Vector3 selectedPosition;

    void Start()
    {
        eventSystem = EventSystem.current;

        // Pastikan first selected ada di dalam mainMenuParent
        if (eventSystem.firstSelectedGameObject != null &&
            eventSystem.firstSelectedGameObject.transform.IsChildOf(_mainMenuParent))
        {
            lastSelected = eventSystem.firstSelectedGameObject;
            eventSystem.SetSelectedGameObject(lastSelected);
            selectedPosition = lastSelected.GetComponent<RectTransform>().position;
            SetIndicator(selectedPosition);
        }
        else
        {
            _indicator.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        GameObject current = eventSystem.currentSelectedGameObject;

        if (current != null && current.transform.IsChildOf(_mainMenuParent))
        {
            lastSelected = current;
            selectedPosition = lastSelected.GetComponent<RectTransform>().position;
            SetIndicator(selectedPosition);
        }
        else if (lastSelected != null)
        {
            // Jaga supaya tetap di lastSelected yang valid
            eventSystem.SetSelectedGameObject(lastSelected);
            selectedPosition = lastSelected.GetComponent<RectTransform>().position;
            SetIndicator(selectedPosition);
        }
        else
        {
            _indicator.gameObject.SetActive(false);
        }
    }

    private void SetIndicator(Vector3 position)
    {
        _indicator.position = position + _offset;
        _indicator.gameObject.SetActive(true);
    }
}
