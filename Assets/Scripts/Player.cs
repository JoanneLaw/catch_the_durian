using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private Vector3 initialPos = Vector3.zero;
    [SerializeField] private InputAction press = null;
    [SerializeField] private InputAction screenPos = null;

    [Button("Get Initial Position", ButtonSizes.Medium)]
    private void GetCurrentPos()
    {
        initialPos = transform.position;
    }

    private Camera camera;
    private Vector3 curScreenPos;
    private bool isDragging;

    private Vector3 CurrentWorldPos
    {
        get
        {
            float zPos = camera.WorldToScreenPoint(transform.position).z;
            return camera.ScreenToWorldPoint(curScreenPos + new Vector3(0, 0, zPos));
        }
    }

    private bool IsPressOnPlayer
    {
        get
        {
            Ray ray = camera.ScreenPointToRay(curScreenPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return hit.transform == transform;
            }
            return false;
        }
    }

    private void Awake()
    {
        camera = Camera.main;
    }

    private void OnEnable()
    {
        screenPos.Enable();
        press.Enable();

        screenPos.performed += OnScreenPosChanged;
        press.started += OnPressed;
        press.canceled += OnPressedCancelled;
    }

    private void OnDisable()
    {
        screenPos.performed -= OnScreenPosChanged;
        press.started -= OnPressed;
        press.canceled -= OnPressedCancelled;

        screenPos.Disable();
        press.Disable();
    }

    private void OnScreenPosChanged(InputAction.CallbackContext context)
    {
        curScreenPos = context.ReadValue<Vector2>();
    }

    private void OnPressed(InputAction.CallbackContext context)
    {
        if (GameManager.instance.State == GameManager.GameState.Pause)
        {
            isDragging = false;
            return;
        }

        if (IsPressOnPlayer)
        {
            StartCoroutine(Drag());
        }
    }

    private void OnPressedCancelled(InputAction.CallbackContext context)
    {
        isDragging = false;
    }

    private IEnumerator Drag()
    {
        isDragging = true;
        Vector3 offset = transform.position - CurrentWorldPos;
        while(isDragging)
        {
            if (GameManager.instance.State == GameManager.GameState.Pause || GameManager.instance.State == GameManager.GameState.GameOver)
            {
                isDragging = false;
                yield return null;
            }
            
            if (GameManager.instance.State == GameManager.GameState.Play)
            {
                GameManager.instance.StartGame();
            }
            float leftBoundary = GameManager.instance.UiManager.MinScreenWorldPos.x + spriteRenderer.size.x * 0.65f;
            float rightBoundary = GameManager.instance.UiManager.MaxScreenWorldPos.x - spriteRenderer.size.x * 0.65f;
            transform.position = new Vector3(Mathf.Clamp((CurrentWorldPos + offset).x, leftBoundary, rightBoundary), transform.position.y, transform.position.z);
            yield return null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (CatchSuccess(other.gameObject))
        {
            Item item = other.GetComponent<Item>();
            int score = FormulaCalculations.GetScore(item.ItemType);

            switch(item.ItemType)
            {
                case Item.Type.Normal:
                    FloatingTextManager.instance.ShowFloatingText($"+{score}", other.transform, "#FFF800");
                    PlayerManager.instance.AddScore(score, item.Stage);
                    MissionManager.instance.TriggerMission(MissionManager.MissionType.GoodDurianCount, 1);

                    if (!PlayerManager.instance.IsFeverTime)
                        PlayerManager.instance.AddCollectedAmount(1);
                    break;
                case Item.Type.Spoiled:
                    PlayerManager.instance.AddChance(-1);
                    break;
                case Item.Type.Hp:
                    PlayerManager.instance.AddChance(1);
                    break;
                case Item.Type.Premium:
                    FloatingTextManager.instance.ShowFloatingText($"+{score}", other.transform, "#FFF800");
                    PlayerManager.instance.AddScore(score, item.Stage, true);
                    MissionManager.instance.TriggerMission(MissionManager.MissionType.PremiumDurianCount, 1);
                    break;
            }
            item.Hide();
        }
    }

    private bool CatchSuccess(GameObject item)
    {
        return (item.transform.position.y >= transform.position.y)
            && (item.transform.position.y <= transform.position.y + 0.6f)
            && (item.transform.position.x >= (transform.position.x - 0.7f))     // left boundary
            && (item.transform.position.x <= (transform.position.x + 0.7f));    // right boundary
    }

    public void Restart()
    {
        transform.position = initialPos;
    }
}
