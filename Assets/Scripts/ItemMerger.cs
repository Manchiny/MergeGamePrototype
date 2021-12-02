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

    /// <summary>
    /// Вычисляет цепочку находящихся рядом предметов, соответствующих пермещаемому
    /// </summary>
    /// <param name="mergeItem"></param>
    /// <param name="callingCell"></param>
    /// <param name="isCaller"></param>
    /// <returns></returns>
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
            if (neighbor.ContentItem != null && neighbor.ContentItem.Phase == mergeItem.Phase)
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
        int nextItemPhaseId = mergeItem.Phase + 1;

        MoveItemToCell(cell)
            .Then(() => CreateNextItem(cell, nextItemPhaseId));
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
        Destroy(item.gameObject);
        return Promise.Resolved();
    }

    private IPromise CreateNextItem(Cell cell, int nextItemPhaseId)
    {
        Debug.Log("Создаю новый предмет:");
        var promise = new Promise();

        ItemFabric.Instance.CreateItem(cell, nextItemPhaseId);

        return Promise.Resolved();
    }

}
