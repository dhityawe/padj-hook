using System.Collections;
using UnityEngine;
using TMPro;

namespace Assets.Scripts.UI
{
    public class TextAnimation : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float animationInterval = 0.05f;
        [SerializeField] private string loadingText = "Loading...";
        [SerializeField] private bool loopAnimation = true;
        
        private Coroutine currentAnimation;
        private TextMeshProUGUI targetText;
        
        public void StartAnimation(TextMeshProUGUI textComponent, string content = null, float interval = -1f, bool isLoop = true)
        {
            if (textComponent == null) return;
            
            // Stop any existing animation
            StopAnimation();
            
            targetText = textComponent;
            string textToAnimate = content ?? loadingText;
            float animInterval = interval > 0 ? interval : animationInterval;
            
            currentAnimation = StartCoroutine(AnimateText(textToAnimate, animInterval, isLoop));
        }
        
        public void StopAnimation()
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
                currentAnimation = null;
            }
        }
        
        public void StartLoadingAnimation(TextMeshProUGUI textComponent)
        {
            StartAnimation(textComponent, loadingText, animationInterval, loopAnimation);
        }
        
        private IEnumerator AnimateText(string content, float interval, bool isLoop = false)
        {
            string value = string.Empty;
            while (true)
            {
                foreach (char c in content.ToCharArray())
                {
                    value += c;
                    if (targetText != null)
                    {
                        targetText.text = value;
                    }
                    yield return new WaitForSeconds(interval);
                }
                if (string.Equals(value, content) && isLoop)
                {
                    value = string.Empty;
                }
                else
                {
                    break;
                }
            }
        }
        
        private void OnDestroy()
        {
            StopAnimation();
        }
    }
}
