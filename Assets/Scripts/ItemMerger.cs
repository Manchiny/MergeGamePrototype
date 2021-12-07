using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Libraries.RSG;
using System.Linq;

public class ItemMerger : MonoBehaviour
{
    public static ItemMerger Instance { get; private set; }

    private HashSet<Item> mergeItems;
    private HashSet<Ground> chekedCells;
    private HashSet<Ground> freeCells;

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

    /// <summary>
    /// Вычисляет цепочку находящихся рядом предметов, соответствующих пермещаемому
    /// </summary>
    /// <param name="mergeItem"></param>
    /// <param name="callingCell"></param>
    /// <param name="isCaller"></param>
    /// <returns></returns>
    public bool TryMergeItems(Item mergeItem, Ground callingCell, bool isCaller)
    {
        HashSet<Ground> neighbors = callingCell.GetNeighbors();
        
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
                if (neighbors.Contains(cell))
                {
                    neighbors.Remove(cell);
                }
            }
        }

        foreach (var cell in neighbors)
        {
            chekedCells.Add(callingCell);
            if (cell.ContentItem != null && cell.ContentItem.ItemID == mergeItem.ItemID && cell.ContentItem.Stage == mergeItem.Stage)
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
        int minCountForMerge = 3;
        int bonusMerge = 5;
        int bonusItems = mergeItems.Count / bonusMerge;
        int itemsNewStage = 0; /*= mergeItems.Count / minCountForMerge;*/
        int itemsOldStage = 0;
        if (mergeItems.Count >= bonusMerge)
        {
            int a = mergeItems.Count - bonusItems * bonusMerge; // предметов осталось после вычета бонусного мерджа
            int b = (a / minCountForMerge) * minCountForMerge; // сколько предметов мерджится из остатка (забираеся из колекции)              
            int c = mergeItems.Count - bonusItems * bonusMerge - b; // остаток общий
                                                                    // 
            itemsNewStage = bonusItems * 2 + b / minCountForMerge;
            itemsOldStage = c;          
        }
        else
        {
            itemsNewStage = mergeItems.Count / minCountForMerge;
            itemsOldStage = mergeItems.Count % minCountForMerge;
        }

        Debug.Log($"Merging {mergeItems.Count} items, old phase count = {itemsOldStage}, new phase count = {itemsNewStage} ");

        int itemID = mergeItem.ItemID;
        int nextStageId = mergeItem.Stage + 1;

        MoveItemsToCellAndDestroy(cell)
            .Then(() => CreateNextPhaseItems(cell, itemID, nextStageId, itemsNewStage, itemsOldStage));
    }

    private IPromise MoveItemsToCellAndDestroy(Ground cell)
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
        //Destroy(item.gameObject);
        item.RemoveItem();
        return Promise.Resolved();
    }

    private IPromise CreateNextPhaseItems(Ground cell, int itemID, int nextItemStageId, int newStageItemsCount, int oldStageItemsCount)
    {
        Debug.Log("Создаю новый предмет:");
        var promise = new Promise();

        ItemFabric.Instance.CreateItem(cell, itemID, nextItemStageId);

        if(newStageItemsCount+ oldStageItemsCount > 1)
        {  
            FindFreeCells(cell, newStageItemsCount - 1 + oldStageItemsCount, true);
            var cells = freeCells.ToArray();

            int counter = 0;
            for (int i = 0; i < newStageItemsCount-1; i++)
            {
                ItemFabric.Instance.CreateItem(cells[counter], itemID, nextItemStageId);
                counter++; 
            }
            for (int i = 0; i < oldStageItemsCount; i++)
            {
                ItemFabric.Instance.CreateItem(cells[counter], itemID, nextItemStageId -1);
                counter++;
            }
        }

        return Promise.Resolved();
    }

    private bool FindFreeCells(Ground baseCell, int count, bool isCaller)
    {
        HashSet<Ground> neighbors = baseCell.GetNeighbors();

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
                    Debug.Log($"{freeCells.Count} positions finded successful");
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
                Debug.Log($"{freeCells.Count} positions finded successful 0");
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
