using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemStageView : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI _phaseNumber;
    // [SerializeField] private SpriteRenderer _phaseImage; 
    [SerializeField] private Transform _imageHolder;
    [SerializeField] private SpriteRenderer _footing;

    private Item _item;
    private void Awake()
    {
        _item = GetComponent<Item>();
        _item.OnStageChanged += UpdatePhase;
        _footing.color = new Color32(255, 255, 93, 255);
        //_footing.gameObject.SetActive(false);
    }

    private void UpdatePhase()
    {
        // _phaseNumber.text = $"{_item.Phase}";
        Instantiate(_item.GetStageImage(_item.Stage), _imageHolder);
    }
}
