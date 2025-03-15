using UnityEngine;
using TMPro;
using DG.Tweening;

using System;

namespace Dennis.Tools.Dialogue
{
    /// <summary>
    /// TMP DoTween Animation Type
    /// </summary>
    public enum TextAnimationType
    {
        FadeAndScale,
        RotateAndColor,
        WaveEffect,
        TypewriterEffect,
        PixelShake,
        TypewriterSmooth,
        TypewriterScramble
    }

    /// <summary>
    /// TMPro Text Animation
    /// </summary>
    public class TextAnimation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private TextAnimationType _animationType = TextAnimationType.FadeAndScale;

        /// <summary>
        /// Starts the animation based on the specified TextAnimationType.
        /// </summary>
        public void StartAnimation(Action OnAnimationCompleteCallback = null)
        {
            switch (_animationType)
            {
                case TextAnimationType.FadeAndScale:
                    FadeAndScaleAnimation(OnAnimationCompleteCallback);
                    break; ;

                case TextAnimationType.RotateAndColor:
                    RotateAndColorAnimation(OnAnimationCompleteCallback);
                    break;

                case TextAnimationType.WaveEffect:
                    WaveEffectAnimation(OnAnimationCompleteCallback);
                    break;

                case TextAnimationType.TypewriterEffect:
                    TypewriterEffectAnimation(OnAnimationCompleteCallback);
                    break;

                case TextAnimationType.PixelShake:
                    PixelShakeAnimation(OnAnimationCompleteCallback);
                    break;

                case TextAnimationType.TypewriterSmooth:
                    TypewriterSmoothAnimation(OnAnimationCompleteCallback);
                    break;

                case TextAnimationType.TypewriterScramble:
                    TypewriterScrambleAnimation(OnAnimationCompleteCallback);
                    break;

                default:
#if UNITY_EDITOR
                    Debug.LogError($"Unknown animation type: {_animationType}. Cannot start animation.");
#endif
                    break;
            }
        }

        #region Animation

        private void FadeAndScaleAnimation(Action OnAnimationCompleteCallback)
        {
            DOTweenTMPAnimator tmproAnimator = new DOTweenTMPAnimator(_text);

            for (int i = 0; i < tmproAnimator.textInfo.characterCount; ++i)
            {
                tmproAnimator.DOScaleChar(i, 0.7f, 0);
                Vector3 currCharOffset = tmproAnimator.GetCharOffset(i);
                DOTween.Sequence()
                    .Append(tmproAnimator.DOOffsetChar(i, currCharOffset + new Vector3(0, 30, 0), 0.4f).SetEase(Ease.OutFlash, 2))
                    .Join(tmproAnimator.DOFadeChar(i, 1, 0.4f))
                    .Join(tmproAnimator.DOScaleChar(i, 1, 0.4f).SetEase(Ease.OutBack))
                    .SetDelay(0.07f * i);
            }

            DOTween.Sequence()
                .AppendInterval(0.07f * tmproAnimator.textInfo.characterCount + 0.5f)
                .OnComplete(() => OnAnimationCompleteCallback?.Invoke());
        }

        private void RotateAndColorAnimation(Action OnAnimationCompleteCallback)
        {
            _text.color = Color.black;

            DOTweenTMPAnimator tmproAnimator = new DOTweenTMPAnimator(_text);

            for (int i = 0; i < tmproAnimator.textInfo.characterCount; ++i)
            {
                tmproAnimator.DORotateChar(i, Vector3.up * 90, 0);
                DOTween.Sequence()
                    .Append(tmproAnimator.DORotateChar(i, Vector3.zero, 0.4f))
                    .AppendInterval(1f)
                    .Append(tmproAnimator.DOColorChar(i, new Color(1f, 1f, 0.8f), 0.2f).SetLoops(2, LoopType.Yoyo))
                    .SetDelay(0.07f * i);
            }

            DOTween.Sequence()
                .AppendInterval(0.07f * tmproAnimator.textInfo.characterCount + 1.5f)
                .OnComplete(() => OnAnimationCompleteCallback?.Invoke());
        }

        private void WaveEffectAnimation(Action OnAnimationCompleteCallback)
        {
            DOTweenTMPAnimator tmproAnimator = new DOTweenTMPAnimator(_text);
            float duration = 0.5f;

            for (int i = 0; i < tmproAnimator.textInfo.characterCount; ++i)
            {
                Vector3 initialOffset = tmproAnimator.GetCharOffset(i);
                DOTween.Sequence()
                    .Append(tmproAnimator.DOOffsetChar(i, initialOffset + new Vector3(0, 10, 0), duration).SetEase(Ease.InOutSine))
                    .Append(tmproAnimator.DOOffsetChar(i, initialOffset, duration).SetEase(Ease.InOutSine))
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetDelay(0.05f * i);
            }

            DOTween.Sequence()
                .AppendInterval(duration * 2 + 1.0f)
                .OnComplete(() => OnAnimationCompleteCallback?.Invoke());
        }

        private void TypewriterEffectAnimation(Action OnAnimationCompleteCallback)
        {
            _text.maxVisibleCharacters = 0;
            int totalCharacters = _text.textInfo.characterCount;

            DOTween.To(() => _text.maxVisibleCharacters, x => _text.maxVisibleCharacters = x, totalCharacters, totalCharacters * 0.07f)
                .SetEase(Ease.Linear)
                .OnComplete(() => OnAnimationCompleteCallback?.Invoke());
        }

        private void PixelShakeAnimation(Action OnAnimationCompleteCallback)
        {
            DOTweenTMPAnimator tmproAnimator = new DOTweenTMPAnimator(_text);
            float duration = 0.1f;
            float shakeAmount = 2f;

            for (int i = 0; i < tmproAnimator.textInfo.characterCount; ++i)
            {
                Vector3 initialOffset = tmproAnimator.GetCharOffset(i);
                DOTween.Sequence()
                    .Append(tmproAnimator.DOOffsetChar(i, initialOffset + new Vector3(UnityEngine.Random.Range(-shakeAmount, shakeAmount), UnityEngine.Random.Range(-shakeAmount, shakeAmount), 0), duration).SetEase(Ease.InOutQuad))
                    .Append(tmproAnimator.DOOffsetChar(i, initialOffset, duration).SetEase(Ease.InOutQuad))
                    .SetLoops(5, LoopType.Yoyo)
                    .SetDelay(0.02f * i);
            }

            DOTween.Sequence()
                .AppendInterval(duration * 10 + 0.5f)
                .OnComplete(() => OnAnimationCompleteCallback?.Invoke());
        }

        private void TypewriterSmoothAnimation(Action OnAnimationCompleteCallback)
        {
            string targetText = _text.text;
            float animationDuration = 2.0f;

            _text.text = "";
            _text.DOText(targetText, animationDuration).SetEase(Ease.Linear)
                .OnComplete(() => OnAnimationCompleteCallback?.Invoke());
        }

        private void TypewriterScrambleAnimation(Action OnAnimationCompleteCallback)
        {
            string targetText = _text.text;
            float animationDuration = 2.0f;

            _text.text = "";
            _text.DOText(targetText, animationDuration, false, ScrambleMode.Uppercase).SetEase(Ease.Linear)
                .OnComplete(() => OnAnimationCompleteCallback?.Invoke());
        }

        #endregion Animation
    }
}
