using UnityEngine;
using Zenject;

namespace Core.InputSystem
{
    public class InputHandler : ITickable
    {
        private readonly SignalBus signalBus;

        [Inject]
        public InputHandler(SignalBus signalBus) => this.signalBus = signalBus;

        public void Tick()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
                SendInput(Input.mousePosition);
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            SendInput(Input.GetTouch(0).position);
#endif
        }

        private void SendInput(Vector2 screenPos)
        {
            signalBus.Fire(new InputSignal { ScreenPosition = screenPos });
        }
    }
}