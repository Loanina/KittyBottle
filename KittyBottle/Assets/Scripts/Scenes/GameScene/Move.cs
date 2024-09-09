using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public class Move
    {
        public int from { get; private set; }
        public int to { get; private set; }
        public int countOfColorToTransfer { get; private set; }

        public Move(int from, int to, int countOfColorToTransfer)
        {
            this.from = from;
            this.to = to;
            this.countOfColorToTransfer = countOfColorToTransfer;
        }
    }
}
