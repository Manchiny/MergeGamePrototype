using UnityEngine;
using TMPro;

public class ItemPhaseView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _phaseNumber;

    private Item _item;
    private void Awake()
    {
        _item = GetComponent<Item>();
        _item.OnPhaseChanged += UpdatePhaseText;
    }

    private void UpdatePhaseText()
    {
        _phaseNumber.text = $"{_item.Phase}";
    }
}
