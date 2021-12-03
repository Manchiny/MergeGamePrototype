using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Libraries.RSG;
using System.Linq;

public class ItemMerger : MonoBehaviour
{
    public static ItemMerger Instance { get; private set; }
    public static HashSet<Item> mergeItems;
    public static HashSet<Ground> chekedCells;
    public static HashSet<Ground> freeCells;
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
            chekedCells = new HashSet<Ground>();

            mergeItems.Add(mergeItem);
            mergeItems.Add(callingCell.ContentItem);
        }
        else
        {
            foreach (var cell in chekedCells)
            {
                if (Neighbors.Contains(cell))
                {
                    Neighbors.Remove(cell);
                }
            }
        }

        foreach (var cell in Neighbors)
        {
            chekedCells.Add(callingCell);
            if (cell.ContentItem != null && cell.ContentItem.Phase == mergeItem.Phase)
            {
                mergeItems.Add(cell.ContentItem);
                TryMergeItems(mergeItem, cell, false);
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
        int countForMerge = 3;
        int itemsNewPhase = mergeItems.Count / countForMerge;
        int itemsOldPhase = mergeItems.Count % countForMerge;
        Debug.Log($"Merging {mergeItems.Count} items, old phase count = {itemsOldPhase}, new phase count = {itemsNewPhase} ");

        int nextItemPhaseId = mergeItem.Phase + 1;

        MoveItemToCell(cell)
            .Then(() => CreateNextPhaseItems(cell, nextItemPhaseId, itemsNewPhase, itemsOldPhase));
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
        Debug.Log("All Items destoyed, creating new.");

        return Promise.All(promises);
    }
    private IPromise DestroyItem(Item item)
    {
        Destroy(item.gameObject);
        return Promise.Resolved();
    }

    private IPromise CreateNextPhaseItems(Ground cell, int nextItemPhaseId, int newPhaseItemsCount, int oldPhaseItemsCount)
    {
        Debug.Log("Создаю новый предмет:");
        var promise = new Promise();

        ItemFabric.Instance.CreateItem(cell, nextItemPhaseId);

        if(newPhaseItemsCount+ oldPhaseItemsCount > 1)
        {  
            FindFreeCells(cell, newPhaseItemsCount - 1 + oldPhaseItemsCount, true);
            var cells = freeCells.ToArray();

            int counter = 0;
            for (int i = 0; i < newPhaseItemsCount-1; i++)
            {
                ItemFabric.Instance.CreateItem(cells[counter], nextItemPhaseId);
                counter++; 
            }
            for (int i = 0; i < oldPhaseItemsCount; i++)
            {
                ItemFabric.Instance.CreateItem(cells[counter], nextItemPhaseId-1);
                counter++;
            }
        }

        return Promise.Resolved();
    }

    private bool FindFreeCells(Ground baseCell, int count, bool isCaller)
    {
        List<Ground> neighbors = baseCell.GetNeighbors();

        if (isCaller == true)
        {
            chekedCells = new HashSet<Ground>();
            freeCells = new HashSet<Ground>();
        }
        else
        {
            foreach (var cell in chekedCells)
            {
                if (neighbors.Contains(cell))
                {
                    neighbors.Remove(cell);
                }
            }
        }

        foreach (var cell in neighbors)
        {
            chekedCells.Add(baseCell);

            if (cell.ContentItem == null)
            {
                freeCells.Add(cell);
                if (freeCells.Count >= count)
                {
                    Debug.Log($"{freeCells.Count} positions finded successful 1");
                    return true;
                }
                else
                {
                    FindFreeCells(cell, count, false);
                }             
            }
        }

        if (isCaller)
        {
            if (freeCells.Count >= count)
            {
                Debug.Log($"{freeCells.Count} positions finded successful");
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

}
