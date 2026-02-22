using UnityEngine;
using DG.Tweening;
using TurtlePath.Core;

namespace TurtlePath.Collectibles
{
    public class CollectibleView : MonoBehaviour
    {
        public CollectibleType Type { get; private set; }
        public Vector2Int GridPosition { get; private set; }
        public bool IsCollected { get; private set; }

        // Fallback tint colors (used only when sprite is a placeholder white square)
        private static readonly Color ShellColor = new Color(1f, 0.435f, 0.412f);   // Coral Pink #FF6F69
        private static readonly Color BabyColor = new Color(1f, 0.714f, 0.757f);    // Baby Pink #FFB6C1

        public void Initialize(CollectibleType type, Vector2Int gridPosition, Sprite sprite, bool useColorTint = false)
        {
            Type = type;
            GridPosition = gridPosition;
            IsCollected = false;

            SpriteRenderer sr = gameObject.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            // Custom sprites contain the final color → no tint needed.
            // Fallback placeholder squares use tint for visual distinction.
            if (useColorTint)
                sr.color = type == CollectibleType.Shell ? ShellColor : BabyColor;
            sr.sortingOrder = 3;

            transform.localScale = Vector3.one * 0.5f;
        }

        public void Collect()
        {
            if (IsCollected) return;
            IsCollected = true;

            transform.DOScale(Vector3.zero, 0.3f)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}
