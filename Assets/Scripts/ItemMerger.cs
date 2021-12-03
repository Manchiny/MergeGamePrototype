using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Libraries.RSG;

public class ItemMerger : MonoBehaviour
{
    public static ItemMerger Instance { get; private set; }
    public static HashSet<Item> mergeItems;
    public static HashSet<Ground> chekedNeighbors;
    public static bool IsMerging;

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
    public bool TryMergeItems(Item mergeItem, Ground callingCell, bool isCaller)
    {
        List<Ground> Neighbors = callingCell.GetNeighbors();
        
        if (isCaller == true)
        {
            mergeItems = new HashSet<Item>();
            chekedNeighbors = new HashSet<Ground>();

            mergeItems.Add(mergeItem);
            mergeItems.Add(callingCell.ContentItem);
        }
        else
        {
            foreach (var neighbor in chekedNeighbors)
            {
                if (Neighbors.Contains(neighbor))
                {
                    Neighbors.Remove(neighbor);
                }
            }
        }

        foreach (var neighbor in Neighbors)
        {
            chekedNeighbors.Add(callingCell);
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

    private void MergeItems(Ground cell, Item mergeItem)
    {
        Debug.Log($"Merging {mergeItems.Count} items");
        int nextItemPhaseId = mergeItem.Phase + 1;

        MoveItemToCell(cell)
            .Then(() => CreateNextItem(cell, nextItemPhaseId));
    }

    private IPromise MoveItemToCell(Ground cell)
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

    private IPromise CreateNextItem(Ground cell, int nextItemPhaseId)
    {
        Debug.Log("Создаю новый предмет:");
        var promise = new Promise();

        ItemFabric.Instance.CreateItem(cell, nextItemPhaseId);

        return Promise.Resolved();
    }

}
