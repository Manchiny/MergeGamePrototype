using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Libraries.RSG;
using System.Collections;

public class Item : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private Item _nextItem;
    public int? NextItemID
    {
        get
        {
            return _nextItem == null ? null : (int?)_nextItem.ID;
        }
    }
    public int ID { get => _id; }
    public Cell CurrentCell { get; private set; }


    public IPromise MoveToMergCell(Cell cell )
    {
        var promise = new Promise();
        float moveTime = 0.25f;
        float scaleTime = 1f;
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(cell.transform.position, moveTime).SetEase(Ease.Linear));
        sequence.Append(transform.DOScale(1.1f, scaleTime));
        sequence.Append(transform.DOScale(0.9f, scaleTime));
        sequence.OnComplete(promise.Resolve);

        Debug.Log("Объект прибыл");
        SetCell(cell);
        Debug.Log("Cell переназначен");

        return promise;
    }

    public void SetCell(Cell newCell)
    {
        if (CurrentCell != null)
        {
            CurrentCell.ContentItem = null;
        }

        if (newCell == null)
        {
            CurrentCell.ContentItem = null;
            CurrentCell = null;
        }
        else
        {
            CurrentCell = newCell;
            CurrentCell.ContentItem = this;
        }
    }
}
