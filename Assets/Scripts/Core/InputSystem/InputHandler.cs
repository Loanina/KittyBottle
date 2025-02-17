using UnityEngine;
using Zenject;

namespace Core.InputSystem
{
    public class InputHandler : ITickable
    {
        private SignalBus signalBus;

        [Inject]
        public InputHandler(SignalBus signalBus)
        {
            this.signalBus = signalBus;
        }

        public void Tick()
        {
#if UNITY_EDITOR
            HandleMouseInput();
#else
            HandleTouchInput();
#endif
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ProcessInput(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        ProcessInput(Camera.main.ScreenToWorldPoint(touch.position));
                    }
                }
            }
        }

        private void ProcessInput(Vector2 position)
        {
            if (Camera.main != null)
            {
                var hit = Physics2D.OverlapPoint(position);
                if (hit != null && hit.TryGetComponent(out IClickable clickable))
                {
                    signalBus.Fire(new InputSignal { Target = clickable });
                }
            }
        }
    }
}