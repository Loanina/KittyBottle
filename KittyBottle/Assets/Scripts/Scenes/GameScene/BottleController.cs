using ModestTree;
using UnityEngine;

namespace Scenes.GameScene
{
    public class BottleController : MonoBehaviour
    {
        [SerializeField] private Color[] bottleColors;
        [SerializeField] private SpriteRenderer bottleMaskSR;

        void UpdateColorsOnShader()
        {
            Debug.Log("<color=red>Вызвана функция смены цветов</color>");
            foreach (var bottleColor in bottleColors)
            {
                try
                {
                    bottleMaskSR.material.SetColor("_Color" + (bottleColors.IndexOf(bottleColor)+1), bottleColor);
                }
                catch (UnityException)
                {
                    Debug.Log("Color" + (bottleColors.IndexOf(bottleColor)+1) +" wasn't set :P");
                }
            }
        }
        
        // Убрать всю хуйню как выстроишь архитектуру
        void Start()
        {
            UpdateColorsOnShader();
        }
    }
}
