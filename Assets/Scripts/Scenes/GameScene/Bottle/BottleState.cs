using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public class BottleState
    {
        private Stack<Color> colors;

        public BottleState(Bottle bottle)
        {
            // Инициализация стека цветов из бутылки
            colors = new Stack<Color>(bottle.GetColors());
        }
        
        public BottleState(List<Color> colors)
        {
            this.colors = new Stack<Color>(colors);
        }

        public bool CanPourInto(BottleState other)
        {
            // Исходная бутылка не должна быть пустой
            if (this.IsEmpty()) return false;

            // Если целевая бутылка пуста, можно перелить хотя бы один слой
            if (other.IsEmpty()) return true;

            // Получаем верхний цвет из текущей бутылки
            var topColor = GetTopColor();

            // Проверка, совпадает ли верхний цвет с целевой бутылкой и есть ли достаточно места
            if (topColor.Equals(other.GetTopColor()) && other.GetFilledLayersCount() < 4)
            {
                return true;
            }
            return false;
        }

        public void PourInto(BottleState other)
        {
            // Если перелить нельзя, ничего не делаем
            if (!CanPourInto(other)) return;

            // Получаем верхний цвет из текущей бутылки
            var topColor = GetTopColor();

            // Пока можем переливать и верхний цвет совпадает, продолжаем перелив
            while (other.GetFilledLayersCount() < 4 && !IsEmpty() && topColor.Equals(GetTopColor()))
            {
                Color color = colors.Pop();
                other.colors.Push(color);
            }
        }

        public Color? GetTopColor()
        {
            return colors.Count > 0 ? colors.Peek() : null;
        }

        public bool IsComplete()
        {
            if (colors.Count != 4) return false;
            var topColor = GetTopColor();
            foreach (var color in colors)
            {
                if (!color.Equals(topColor)) return false;
            }
            return true;
        }

        public bool IsEmpty()
        {
            return colors.Count == 0;
        }

        public int GetFilledLayersCount()
        {
            return colors.Count;
        }

        public string GetStateKey()
        {
            return string.Join("-", colors.Select(c => $"{c.r},{c.g},{c.b}"));
        }

        public BottleState Clone()
        {
            return new BottleState(new List<Color>(colors));
        }
    }
}
