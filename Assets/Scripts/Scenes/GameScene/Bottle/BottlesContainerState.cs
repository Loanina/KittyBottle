using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace Scenes.GameScene.Bottle
{
    public class BottlesContainerState
    {
        private List<BottleState> bottles;
        private List<(int, int)> moves;

        public BottlesContainerState(List<Bottle> bottles)
        {
            this.bottles = bottles.Select(b => new BottleState(b)).ToList();
            this.moves = new List<(int, int)>();
        }

        public BottlesContainerState(List<BottleState> bottleStates)
        {
            this.bottles = bottleStates;
            this.moves = new List<(int, int)>();
        }

        public BottleState GetBottle(int index)
        {
            return bottles[index];
        }

        public void ApplyMove(int from, int to)
        {
            bottles[from].PourInto(bottles[to]);
            moves.Add((from, to));
        }

        public List<(int, int)> GetAvailableMoves()
        {
            List<(int, int)> moves = new List<(int, int)>();
            for (int i = 0; i < bottles.Count; i++)
            {
                for (int j = 0; j < bottles.Count; j++)
                {
                    if (i != j && bottles[i].CanPourInto(bottles[j]))
                    {
                        moves.Add((i, j));
                    }
                }
            }
            return moves;
        }

        public bool IsSolved()
        {
            return bottles.All(bottle => bottle.IsComplete() || bottle.IsEmpty());
        }

        public string GetStateKey()
        {
            return string.Join("|", bottles.Select(b => b.GetStateKey()));
        }

        public BottlesContainerState Clone()
        {
            BottlesContainerState clone = new BottlesContainerState(this.bottles.Select(b => b.Clone()).ToList());
            clone.moves = new List<(int, int)>(this.moves);
            return clone;
        }

        public (int, int) GetFirstMove()
        {
            Debug.Log($"Вот он - первый ход: {moves.First().Item1}  ---  {moves.First().Item2}");
            return moves.First();
        }
    }
}
