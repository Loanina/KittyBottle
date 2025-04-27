using UnityEngine;
using UnityEngine.Rendering;

namespace Scenes.GameScene.Bottle
{
    public class BottleView : MonoBehaviour
    {
        [SerializeField] public Transform glassTransform;
        [SerializeField] private SpriteRenderer rightColorFlow;
        [SerializeField] private SpriteRenderer leftColorFlow;
        [SerializeField] public SpriteRenderer bottleMaskSR;
        
        [SerializeField] private SortingGroup sortingGroup;
        [SerializeField] public Transform rightPouringPoint;
        [SerializeField] public Transform leftPouringPoint;
        [SerializeField] public Transform rightRotationPoint;
        [SerializeField] public Transform leftRotationPoint;

        public void SetColorFlow(bool isRightDirection, bool isActive, Color color)
        {
            if (isRightDirection)
            {
                rightColorFlow.enabled = isActive;
                rightColorFlow.color = color;
            }
            else
            {
                leftColorFlow.enabled = isActive;
                leftColorFlow.color = color;
            }
        }
        
        public void SetColorFlow(bool isRightDirection, bool isActive)
        {
            if (isRightDirection) rightColorFlow.enabled = isActive;
            else leftColorFlow.enabled = isActive;
        }

        public void SetSortingOrder(bool toUp)
        {
            sortingGroup.sortingOrder = toUp ? 2 : 1;
        }

        public void StopColorFlow()
        {
            rightColorFlow.enabled = false;
            leftColorFlow.enabled = false;
        }
    }
}