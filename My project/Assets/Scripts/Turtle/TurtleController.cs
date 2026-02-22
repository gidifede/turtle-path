using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TurtlePath.Core;
using TurtlePath.Grid;
using TurtlePath.Collectibles;

namespace TurtlePath.Turtle
{
    public class TurtleController : MonoBehaviour
    {
        public event Action OnPathCompleted;
        public event Action<CollectibleType, Vector2Int> OnCollectibleReached;

        private GridManager gridManager;
        private Sequence moveSequence;
        private List<Vector3> breadcrumbs = new List<Vector3>();
        private List<GameObject> followers = new List<GameObject>();
        private Sprite followerSprite;

        private static readonly Color BabyColor = new Color(1f, 0.714f, 0.757f); // Baby Pink #FFB6C1

        public void Initialize(GridManager gridManager)
        {
            this.gridManager = gridManager;
            gameObject.SetActive(false);
        }

        public void SetFollowerSprite(Sprite sprite)
        {
            followerSprite = sprite;
        }

        public void MoveAlongPath(List<Vector2Int> path)
        {
            if (path == null || path.Count < 2) return;

            gameObject.SetActive(true);
            breadcrumbs.Clear();

            Vector3 startPos = gridManager.GridToWorld(path[0].x, path[0].y);
            transform.position = startPos;
            breadcrumbs.Add(startPos);

            int pathLength = path.Count;
            float totalDuration = Mathf.Clamp(pathLength * 0.4f, 2.0f, 4.0f);
            float durationPerCell = totalDuration / pathLength;

            moveSequence = DOTween.Sequence();

            for (int i = 1; i < path.Count; i++)
            {
                Vector3 targetPos = gridManager.GridToWorld(path[i].x, path[i].y);
                Vector2Int gridPos = path[i];

                moveSequence.Append(
                    transform.DOMove(targetPos, durationPerCell)
                        .SetEase(Ease.InOutSine)
                );
                moveSequence.AppendCallback(() =>
                {
                    breadcrumbs.Add(targetPos);
                    CheckCollectible(gridPos);
                    UpdateFollowers(durationPerCell);
                });
            }

            moveSequence.OnComplete(() =>
            {
                OnPathCompleted?.Invoke();
            });

            moveSequence.Play();
        }

        private void CheckCollectible(Vector2Int gridPos)
        {
            CollectibleView collectible = gridManager.GetCollectible(gridPos);
            if (collectible == null) return;

            CollectibleType type = collectible.Type;

            if (type == CollectibleType.BabyTurtle)
            {
                SpawnFollower(collectible.transform.position);
            }

            collectible.Collect();
            OnCollectibleReached?.Invoke(type, gridPos);
        }

        private void SpawnFollower(Vector3 position)
        {
            GameObject followerGO = new GameObject($"BabyFollower_{followers.Count}");
            followerGO.transform.position = position;
            followerGO.transform.localScale = transform.localScale * 0.6f;

            SpriteRenderer sr = followerGO.AddComponent<SpriteRenderer>();
            sr.sprite = followerSprite;
            sr.color = BabyColor;
            sr.sortingOrder = 9;

            followers.Add(followerGO);
        }

        private void UpdateFollowers(float duration)
        {
            for (int i = 0; i < followers.Count; i++)
            {
                int targetBreadcrumbIndex = breadcrumbs.Count - 1 - (i + 1);
                if (targetBreadcrumbIndex < 0) targetBreadcrumbIndex = 0;

                Vector3 targetPos = breadcrumbs[targetBreadcrumbIndex];
                float delay = 0.3f * (i + 1);

                followers[i].transform.DOMove(targetPos, duration)
                    .SetEase(Ease.InOutSine)
                    .SetDelay(delay);
            }
        }

        public void ResetTurtle()
        {
            if (moveSequence != null)
            {
                moveSequence.Kill();
                moveSequence = null;
            }

            for (int i = followers.Count - 1; i >= 0; i--)
            {
                if (followers[i] != null)
                    Destroy(followers[i]);
            }
            followers.Clear();
            breadcrumbs.Clear();

            gameObject.SetActive(false);
        }
    }
}
