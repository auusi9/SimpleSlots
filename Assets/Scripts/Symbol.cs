using UnityEngine;
using UnityEngine.UI;

public class Symbol : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private GameObject _prizeMarker;

    private GameObject _createdMarker;
    
    public RectTransform RectTransform => _rectTransform;

    public void OnCreate(Sprite sprite)
    {
        _image.sprite = sprite;
    }

    public void SetPosition(Vector2 vector)
    {
        _rectTransform.anchoredPosition = vector;
    }
    
    public void AddPosition(Vector2 vector)
    {
        _rectTransform.anchoredPosition += vector;
    }

    public void ActivateMarker(int size)
    {
        _createdMarker = Instantiate(_prizeMarker, transform);
        RectTransform marker = _createdMarker.GetComponent<RectTransform>();
        
        Vector2 sizeDelta = marker.sizeDelta;
        sizeDelta = new Vector2((sizeDelta.x * size) + (20f * (size-1)), sizeDelta.y);
        marker.sizeDelta = sizeDelta;
    }

    public void DesactivateMarker()
    {
        if (_createdMarker == null)
        {
            return;
        }

        Destroy(_createdMarker);
        _createdMarker = null;
    }
}
