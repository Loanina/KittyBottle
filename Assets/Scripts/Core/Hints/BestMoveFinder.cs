using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common.Logging;
using Scenes.GameScene.Bottle;
using Zenject;

namespace Core.Hints
{
    public class BestMoveFinder
    {
        private readonly IGameLogger logger;
        private readonly IBottlesContainer bottlesContainer;

        [Inject]
        public BestMoveFinder(IGameLogger logger, IBottlesContainer bottlesContainer)
        {
            this.logger = logger;
            this.bottlesContainer = bottlesContainer;
        }
        
        public (int, int) FindBestMove()
        {
            var bottles = bottlesContainer.GetAllBottles();
            var stopwatch = Stopwatch.StartNew();
            var queue = new Queue<BottlesContainerState>();
            var visitedStates = new HashSet<string>();
            var initialState = new BottlesContainerState(bottles.ToList());

            queue.Enqueue(initialState);
            visitedStates.Add(initialState.GetStateKey());

            while (queue.Count > 0)
            {
                var currentState = queue.Dequeue();

                if (currentState.IsSolved())
                {
                    stopwatch.Stop();
                    logger.Log($"Best move found in {stopwatch.ElapsedMilliseconds} ms");
                    return currentState.GetFirstMove();
                }

                foreach (var move in currentState.GetAvailableMoves())
                {
                    var nextState = currentState.Clone();
                    nextState.ApplyMove(move.Item1, move.Item2);

                    var stateKey = nextState.GetStateKey();
                    if (!visitedStates.Contains(stateKey))
                    {
                        visitedStates.Add(stateKey);
                        queue.Enqueue(nextState);
                    }
                }
            }
            stopwatch.Stop();
            logger.Log($"No best move found in {stopwatch.ElapsedMilliseconds} ms");
            return (-1, -1);
        }
        
        private int CalculateHeuristic(BottlesContainerState state, int fromIndex, int toIndex)
        {
            BottleState fromBottle = state.GetBottle(fromIndex);
            BottleState toBottle = state.GetBottle(toIndex);

            if (toBottle.IsEmpty())
            {
                return 1; // Меньший приоритет, если переливаем в пустую бутылку
            }

            if (fromBottle.GetTopColor().Equals(toBottle.GetTopColor()))
            {
                int filledLayers = toBottle.GetFilledLayersCount();
                return 10 + filledLayers; // Чем больше слоев одного цвета уже в бутылке, тем выше приоритет
            }

            return 0; // Низкий приоритет для неэффективных ходов
        }
    }
}