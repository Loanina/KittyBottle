using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.GameScene.Reward
{
    public class RewardItemView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI amountText;
        
        public void Setup(Sprite sprite, int entryAmount)
        {
            icon.sprite = sprite;
            amountText.text = entryAmount.ToString();
        }
    }
}