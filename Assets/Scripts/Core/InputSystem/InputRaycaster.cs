using System;
using UnityEngine;
using Zenject;

namespace Core.InputSystem
{
    public class InputRaycaster : IInitializable, IDisposable
    {
        private readonly SignalBus signalBus;
        private readonly Camera camera;

        [Inject]
        public InputRaycaster(SignalBus signalBus)
        {
            this.signalBus = signalBus;
            camera = Camera.main;
        }

        public void Initialize() => signalBus.Subscribe<InputSignal>(OnInput);

        public void Dispose() => signalBus.Unsubscribe<InputSignal>(OnInput);

        private void OnInput(InputSignal signal)
        {
            var worldPos = camera.ScreenToWorldPoint(signal.ScreenPosition);
            var hit = Physics2D.OverlapPoint(worldPos);
            if (hit && hit.TryGetComponent(out IClickable clickable))
            {
                clickable.OnClick();
            }
        }
    }
}