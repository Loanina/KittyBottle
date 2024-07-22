using UnityEngine;

namespace Scenes.GameScene.RaycastSystem
{
    public class TouchHandler : MonoBehaviour
    {
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main camera not found");
            }
        }

        void Update()
        {
             // Для тестирования на компьютере
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log("Mouse position: " + mousePosition);

                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
                if (hit.collider != null)
                {
                    Debug.Log("Hit: " + hit.collider.name);
                    SpriteTouch spriteTouch = hit.collider.GetComponent<SpriteTouch>();
                    if (spriteTouch != null)
                    {
                        spriteTouch.OnSpriteTouched();
                    }
                    else
                    {
                        Debug.LogWarning("SpriteTouch component not found on " + hit.collider.name);
                    }
                }
                else
                {
                    Debug.Log("No hit detected");
                }
            }

            // Для касаний на мобильных устройствах
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.phase == TouchPhase.Began)
                    {
                        Vector2 touchPosition = mainCamera.ScreenToWorldPoint(touch.position);

                        RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
                        if (hit.collider != null)
                        {
                            SpriteTouch spriteTouch = hit.collider.GetComponent<SpriteTouch>();
                            if (spriteTouch != null)
                            {
                                spriteTouch.OnSpriteTouched();
                            }
                        }
                    }
                }
            }
        }
    }
}
