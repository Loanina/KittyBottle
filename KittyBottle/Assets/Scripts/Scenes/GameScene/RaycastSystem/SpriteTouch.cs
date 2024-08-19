using UnityEngine;

namespace Scenes.GameScene.RaycastSystem
{
    public class SpriteTouch : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour clickableComponent;
        public void OnSpriteTouched()
        {
            if (clickableComponent)
            {
                var method = clickableComponent.GetType().GetMethod("OnClick");
                if (method != null)
                {
                    method.Invoke(clickableComponent, null);
                }
                else
                {
                    Debug.Log("clickable component doesnt have OnClick");
                }
            }
            else
            {
                Debug.Log("clickable component not set");
            }
        }
    }
}
