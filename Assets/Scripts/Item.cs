using UnityEngine;
using DG.Tweening;
using Assets.Scripts.Libraries.RSG;
using System;

public class Item : MonoBehaviour
{
    [SerializeField] private int _phase;

    public int Phase 
    { 
        get => _phase;
        private set
        {
            _phase = value;
            OnPhaseChanged?.Invoke();
        }
    }

    private void Start()
    {
        OnPhaseChanged?.Invoke();
    }
    public Cell CurrentCell { get; private set; }

    public event Action OnPhaseChanged;

    public IPromise MoveToMergCell(Cell cell )
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

    public void SetPhase(int phaseID)
    {
        Phase = phaseID;
    }
}
