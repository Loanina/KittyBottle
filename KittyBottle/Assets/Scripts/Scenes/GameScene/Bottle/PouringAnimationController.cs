using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public class PouringAnimationController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer rightColorFlow;
        [SerializeField] private SpriteRenderer leftColorFlow;

        public void RemoveFlow(bool isRightPouringDirection)
        {
            if (isRightPouringDirection) rightColorFlow.enabled = false;
            else leftColorFlow.enabled = false;
        }

        public void DoColorFlow(bool isRightPouringDirection, Color color)
        {
            if (isRightPouringDirection)
            {
                rightColorFlow.enabled = true;
                rightColorFlow.color = color;
            }
            else
            {
                leftColorFlow.enabled = true;
                leftColorFlow.color = color;
            }
        }
    }
}
