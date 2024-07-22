using UnityEngine;

namespace Scenes.GameScene.RaycastSystem
{
    public class SpriteTouch : MonoBehaviour
    {
        public void OnSpriteTouched()
        {
            var bottle = gameObject.GetComponent<Bottle>();
            if (!bottle) return;
            bottle.OnClick();
        }
    }
}
