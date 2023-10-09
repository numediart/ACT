using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    #region Singleton
    public static FeedbackManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region public fields
    [Header("Instances")] [SerializeField] private Feedback[] _feedbackPrefabs;
    [SerializeField] private Transform _feedbackParent;

    [Header("Parameters")] [SerializeField] private float _feedbackDuration;

    [Header("Do Tween Parameters")] [SerializeField] private float _moveUpDuration;
    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float _fadeOffDuration;
    #endregion
    
    #region private fields
    private List<Feedback> _feedbacks;
    private Dictionary<FeedbackType, GameObject> _feedbackPrefabVariants;
    #endregion
    
    private void Start()
    {
        _feedbacks = new List<Feedback>();
        GenerateFeedbackDict();
    }

    private void GenerateFeedbackDict()
    {
        _feedbackPrefabVariants = new Dictionary<FeedbackType, GameObject>();

        foreach (var feedback in _feedbackPrefabs)
        {
            _feedbackPrefabVariants.Add(feedback.FeedbackType, feedback.transform.gameObject);
        }
    }

    /// <summary>
    /// Function called by every script that needs to pop up a feedback on top of his screen
    /// </summary>
    /// <param name="message"></param>
    /// <param name="feedbackType"></param>
    public void CreateFeedBack(string message, FeedbackType feedbackType)
    {
        GameObject feedbackGo = Instantiate(_feedbackPrefabVariants[feedbackType], _feedbackParent);
        
        // Feedback Script
        feedbackGo.GetComponent<Feedback>().ModifyMessage(message);
        
        RectTransform goRect = feedbackGo.GetComponent<RectTransform>();
        CanvasGroup goCanvasGroup = feedbackGo.GetComponent<CanvasGroup>();
        
        goRect.anchoredPosition = new Vector2(goRect.anchoredPosition.x, -Screen.height);
        goCanvasGroup.alpha = 0f;

        // DOTween
        goRect.DOKill();
        goCanvasGroup.DOKill();

        goRect.DOAnchorPosY(0f, _moveUpDuration).SetEase(Ease.OutQuad);
        goCanvasGroup.DOFade(1f, _fadeInDuration).OnComplete(() =>
            {
                goCanvasGroup.DOFade(0f, _fadeOffDuration).SetDelay(_feedbackDuration).OnComplete(() =>
                {
                    Destroy(feedbackGo);
                });
            });
    }
}
