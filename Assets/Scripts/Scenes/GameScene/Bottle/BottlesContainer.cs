using System;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Scenes.GameScene.Bottle
{
    public class BottlesContainer : SerializedMonoBehaviour
    {
        [SerializeField] private Bottle bottlePrefab;
        [SerializeField] private BottlesController bottlesController;
        [SerializeField] private LayoutSettings layoutSettings;
        private List<Bottle> bottles;
        public event Action OnLevelComplete;

        public void CreateBottles(List<List<Color>> levelColors)
        {
            bottles = new List<Bottle>();
            var layout = layoutSettings.GetLayout(levelColors.Count);
            for (var i = 0; i < levelColors.Count; i++)
            {
                var position = transform.TransformPoint(layout[i]);
                var bottle = Instantiate(bottlePrefab, position, new Quaternion(), transform);
                bottle.Initialize(levelColors[i]);
                bottles.Add(bottle);
                bottle.OnClickEvent += bottlesController.OnClickBottle;
                bottle.OnEndPouring += CheckLevelCompletion;
            }
        }

        public void DeleteBottles()
        {
            if (bottles == null) return;
            foreach (var bottle in bottles)
            {
                DOTween.Kill(bottle.gameObject);
                bottle.OnClickEvent -= bottlesController.OnClickBottle;
                bottle.OnEndPouring -= CheckLevelCompletion;
                Destroy(bottle.gameObject);
            }
            bottles.Clear();
            Debug.Log("Bottles was deleted");
        }

        private void CheckLevelCompletion()
        {
            foreach (var bottle in bottles)
            {
                var numberOfTopColorLayers = bottle.GetNumberOfTopColorLayers();
                if (numberOfTopColorLayers !=4 & numberOfTopColorLayers !=0) return;
            }
            Debug.Log("LEVEL COMPLETE!");
            OnLevelComplete?.Invoke();
        }

        public void CalculateBestMove()
        {
            var bestMove = GetBestMove();
            Debug.Log($"Best move is from bottle №{bestMove.Item1} to bottle №{bestMove.Item2}");
        }
        
        private (int, int) GetBestMove()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Queue<BottlesContainerState> queue = new Queue<BottlesContainerState>();
            HashSet<string> visitedStates = new HashSet<string>();
            BottlesContainerState initialState = new BottlesContainerState(bottles);

            queue.Enqueue(initialState);
            visitedStates.Add(initialState.GetStateKey());
            Debug.Log("Initial State Key: "+initialState.GetStateKey());

            while (queue.Count > 0)
            {
                BottlesContainerState currentState = queue.Dequeue();

                if (currentState.IsSolved())
                {
                    stopwatch.Stop();
                    Debug.Log($"Time to getting best move: {stopwatch.ElapsedMilliseconds} ms");
                    return currentState.GetFirstMove();
                }

                var availableMoves = currentState.GetAvailableMoves();
                
                foreach (var move in availableMoves)
                {
                    BottlesContainerState nextState = currentState.Clone();
                    nextState.ApplyMove(move.Item1, move.Item2);

                    string stateKey = nextState.GetStateKey();
                    if (!visitedStates.Contains(stateKey))
                    {
                        visitedStates.Add(stateKey);
                        queue.Enqueue(nextState);
                    }
                }
            }
            Debug.Log("Best move doesn't find");
            stopwatch.Stop();
            Debug.Log($"Time to getting best move: {stopwatch.ElapsedMilliseconds} ms IT DOESN'T FIND");
            return (-1, -1); // Если решения не найдено
        }

        // Функция эвристики: даем приоритет ходам, которые приближают бутылку к завершенному состоянию
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

        public Bottle GetBottle(int index)
        {
            return bottles[index];
        }

        public int GetIndexOfBottle(Bottle bottle)
        {
            return bottles.IndexOf(bottle);
        }
    }
}
