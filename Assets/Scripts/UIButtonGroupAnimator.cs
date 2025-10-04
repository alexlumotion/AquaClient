using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonGroupAnimator : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button[] buttons = default;
    [SerializeField] private RectTransform[] buttonRects = default;

    [Header("Visuals")]
    [SerializeField] private RectTransform indicator = default;

    [Header("Sizes")]
    [SerializeField] private Vector2 normalSize = new Vector2(224f, 224f);
    [SerializeField] private Vector2 activeSize = new Vector2(548f, 548f);

    [Header("Timing")]
    [SerializeField] private float sizeTweenDuration = 0.35f;
    [SerializeField] private float indicatorTweenDuration = 0.35f;
    [SerializeField] private Ease tweenEase = Ease.OutBack;

    [Header("State")]
    [SerializeField] private int initialActiveIndex = 0;

    private int currentIndex = -1;
    private Tween indicatorTween;
    private Tween[] sizeTweens;

    private void Awake()
    {
        if (buttonRects == null || buttonRects.Length == 0)
        {
            Debug.LogError("UIButtonGroupAnimator: buttonRects are not assigned.", this);
            return;
        }

        sizeTweens = new Tween[buttonRects.Length];

        if (buttons != null && buttons.Length == buttonRects.Length)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int index = i;
                buttons[i].onClick.AddListener(() => SetActiveButton(index));
            }
        }
    }

    private void Start()
    {
        for (int i = 0; i < buttonRects.Length; i++)
        {
            buttonRects[i].sizeDelta = normalSize;
        }

        SetActiveButton(Mathf.Clamp(initialActiveIndex, 0, buttonRects.Length - 1), true);
    }

    public void SetActiveButton(int index)
    {
        SetActiveButton(index, false);
    }

    private void SetActiveButton(int index, bool instant)
    {
        if (buttonRects == null || buttonRects.Length == 0)
        {
            return;
        }

        if (index < 0 || index >= buttonRects.Length)
        {
            Debug.LogWarning($"UIButtonGroupAnimator: index {index} is out of range.", this);
            return;
        }

        if (index == currentIndex)
        {
            return;
        }

        RectTransform newRect = buttonRects[index];

        if (currentIndex >= 0)
        {
            RectTransform previousRect = buttonRects[currentIndex];
            KillSizeTween(currentIndex);

            if (instant)
            {
                previousRect.sizeDelta = normalSize;
            }
            else
            {
                sizeTweens[currentIndex] = previousRect
                    .DOSizeDelta(normalSize, sizeTweenDuration)
                    .SetEase(tweenEase);
            }
        }

        KillSizeTween(index);

        if (instant)
        {
            newRect.sizeDelta = activeSize;
        }
        else
        {
            sizeTweens[index] = newRect
                .DOSizeDelta(activeSize, sizeTweenDuration)
                .SetEase(tweenEase);
        }

        if (indicator)
        {
            indicatorTween?.Kill();
            if (instant)
            {
                indicator.anchoredPosition = newRect.anchoredPosition;
            }
            else
            {
                indicatorTween = indicator
                    .DOAnchorPos(newRect.anchoredPosition, indicatorTweenDuration)
                    .SetEase(tweenEase);
            }
        }

        currentIndex = index;
    }

    private void KillSizeTween(int index)
    {
        if (sizeTweens == null)
        {
            return;
        }

        if (index < 0 || index >= sizeTweens.Length)
        {
            return;
        }

        sizeTweens[index]?.Kill();
        sizeTweens[index] = null;
    }
}
