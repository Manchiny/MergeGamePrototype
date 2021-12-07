using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Libraries.RSG;
using System;

public class Item : MonoBehaviour
{
   
    [SerializeField] private int _itemID;
    public int ItemID 
    { 
        get => Mathf.Clamp(_itemID, 0, StagesCount-1); 
        private set 
        { 
            _itemID = Mathf.Clamp(value, 0, StagesCount - 1); 
        } 
    } 

    [SerializeField] private int _stage;
    [SerializeField] private GameObject[] _stageImages;
    public int StagesCount { get => _stageImages.Length; }
    public int Stage 
    { 
        get => _stage;
        private set
        {
            _stage = value;
            OnStageChanged?.Invoke();
        }
    }

    private void Start()
    {
        OnStageChanged?.Invoke();
    }
    public Ground CurrentCell { get; private set; }

    public event Action OnStageChanged;
    public IPromise MoveToMergCell(Ground cell )
    {
        var promise = new Promise();
        float moveTime = 0.25f;
        float scaleTime = 0.4f;
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(cell.transform.position, moveTime).SetEase(Ease.Linear));
        sequence.Append(transform.DOScale(1.1f, scaleTime/2));
        sequence.Append(transform.DOScale(0.9f, scaleTime));
        sequence.OnComplete(promise.Resolve);

        Debug.Log("Объект прибыл");
        SetCell(cell);
        Debug.Log("Cell переназначен");

        return promise;
    }

    public void SetCell(Ground newCell)
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

    public void SetStage(int stageID)
    {
        Stage = stageID;
    }
    public GameObject GetStageImage(int stageID)
    {
        return _stageImages[stageID];
    }

    public void RemoveItem()
    {
        SetCell(null);
        Destroy(gameObject);
    }
}
