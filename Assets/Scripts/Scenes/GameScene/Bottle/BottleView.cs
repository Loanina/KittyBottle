using UnityEngine;
using UnityEngine.Rendering;

namespace Scenes.GameScene.Bottle
{
    public class BottleView : MonoBehaviour
    {
        [SerializeField] public Transform glassTransform;
        [SerializeField] public SpriteRenderer rightColorFlow;
        [SerializeField] public SpriteRenderer leftColorFlow;
        [SerializeField] public SpriteRenderer bottleMaskSR;
        
        [SerializeField] public SortingGroup sortingGroup;
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
    }
}