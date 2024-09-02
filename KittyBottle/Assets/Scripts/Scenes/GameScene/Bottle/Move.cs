using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public class Move
    {
        private int from;
        private int to;
        private int countOfColorToTransfer;

        public Move(int from, int to, int countOfColorToTransfer)
        {
            this.from = from;
            this.to = to;
            this.countOfColorToTransfer = countOfColorToTransfer;
        }
    }
}
