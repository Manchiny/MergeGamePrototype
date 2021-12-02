using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Libraries.RSG;

public class ItemMerger : MonoBehaviour
{
    public static ItemMerger Instance { get; private set; }
    public static HashSet<Item> mergeItems;
    public static bool IsMerging;

    public static Cell lastCell;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            IsMerging = false;
            return;
        }
        Destroy(gameObject);
    }


    public bool TryMergeItems(Item mergeItem, Cell callingCell, bool isCaller)
    {
        List<Cell> Neighbors = callingCell.GetNeighbors();

        if (isCaller == true)
        {
            mergeItems = new HashSet<Item>();
            mergeItems.Add(mergeItem);
            mergeItems.Add(callingCell.ContentItem);
            lastCell = callingCell;
        }
        else
        {
            Neighbors.Remove(lastCell);
        }

        foreach (var neighbor in Neighbors)
        {
            lastCell = callingCell;
            if (neighbor.ContentItem != null && neighbor.ContentItem.ID == mergeItem.ID)
            {
                mergeItems.Add(neighbor.ContentItem);
                TryMergeItems(mergeItem, neighbor, false);
            }

        }

        if (isCaller)
        {
            if (mergeItems.Count > 2)
            {
                MergeItems(callingCell, mergeItem);
                return true;
            }
        }
        return false;
    }

    private void MergeItems(Cell cell, Item mergeItem)
    {
        Debug.Log($"Merging {mergeItems.Count} items");
        int? nextItemId = mergeItem.NextItemID;

        MoveItemToCell(cell)
            .Then(() => CreateNextItem(cell, nextItemId));
    }

    private IPromise MoveItemToCell(Cell cell)
    {
        var promises = new List<IPromise>();

        foreach (var item in mergeItems)
        {
            var curItem = item;
            promises.Add(item.MoveToMergCell(cell)
                .Then(() => DestroyItem(curItem)));           
        }
        Debug.Log("All Items destoyed, creating new...");

        return Promise.All(promises);
    }
    private IPromise DestroyItem(Item item)
    {
        Debug.Log("Item destoyed");
        return Promise.Resolved();
    }

    private Promise CreateNextItem(Cell cell, int? nextItemId)
    {
        Debug.Log("Создаю новый предмет:");
        var promise = new Promise();

        if (nextItemId != null)
            ItemFabric.Instance.CreateItem(cell, (int)nextItemId);

        return promise;
    }

}
