using System;
using Zenject;

namespace Core.InputSystem
{
    public class ClickableObject : IInitializable, IDisposable
    {
        private readonly SignalBus signalBus;

        [Inject]
        public ClickableObject(SignalBus signalBus)
        {
            this.signalBus = signalBus;
        }

        public void Initialize()
        {
            signalBus.Subscribe<InputSignal>(OnInputReceived);
        }

        public void Dispose()
        {
            signalBus.Unsubscribe<InputSignal>(OnInputReceived);
        }

        private void OnInputReceived(InputSignal signal)
        {
            signal.Target.OnClick();
        }
    }
}