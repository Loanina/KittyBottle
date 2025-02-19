using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scenes.GameScene.Bottle
{
    public interface IBottlesContainer
    {
        void CreateBottles(List<List<Color>> colors);
        void DeleteBottles();
        Bottle GetBottle(int index);
        int GetIndexOfBottle(Bottle bottle);

        public List<Bottle> GetAllBottles();
        event Action OnLevelComplete;
    }
}