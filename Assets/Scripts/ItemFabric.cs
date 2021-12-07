using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemFabric : MonoBehaviour
{
    public static ItemFabric Instance { get; set; }

    [SerializeField] private GameObject[] _itemPrefabs;

    private Vector3 _offset = new Vector3(0, 0, -0.01f);
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            return;
        }
        Destroy(gameObject);
    }
    public void CreateItem(Ground cell, int itemID, int newPhaseID)
    {
        GameObject newItem = Instantiate(_itemPrefabs[itemID], cell.transform.position + _offset, Quaternion.identity);
        Item item = newItem.GetComponent<Item>();
        item.SetStage(newPhaseID);
        item.SetCell(cell);
    }
}
