using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScanDartLoader : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ScanDartData _probe;

    [SerializeField]
    private GameObject _player;
    [SerializeField]
    private float _maxDist = 5f;
    bool reloaded = true;
    [SerializeField]
    private TweenData _tween;
    [SerializeField]
    private Vector3 StartPos;
    [SerializeField]
    private Vector3 endPos;
    [SerializeField]
    private Transform _probeTransform;

 
    public bool CheckInteractable(float distance)
    {
        if (_maxDist < distance)
        {
            return false;
        }
        return !_probe.IsLoaded;

    }

    public string GetInteractableLabel()
    {
        return "Press E to reload Scan Darts";
    }

    public void OnInteracted(PlayerControls playerInteracted)
    {
        reloaded = true;
        _probe.RecallDarts(_player);
        _probeTransform.DOKill();
        _probeTransform.DOLocalMove(endPos, _tween.Duration).SetEase(_tween.Ease);

    }


    void Update()
    {
        if (reloaded && !_probe.IsLoaded)
        {
            reloaded = false;
            _probeTransform.DOKill();
            _probeTransform.DOLocalMove(StartPos, _tween.Duration).SetEase(_tween.Ease);
        }
    }
}
