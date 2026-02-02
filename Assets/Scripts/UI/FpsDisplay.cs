using TMPro;
using UnityEngine;

public class FpsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _fpsText;
    private float updateInterval = 1.0f;
    private float countdown = 0;

    private float _currentFPS;

    private void Start()
    {
        _fpsText.text = "FPS: 0";
    }

    private void Update()
    {
        if (countdown <= 0)
        {
            _currentFPS = 1f / Time.deltaTime;
            UpdateFPS();

            countdown = updateInterval;
        }

        countdown -= Time.deltaTime;
    }

    private void UpdateFPS()
    {
        if (_currentFPS > 50)
            _fpsText.color = Color.green;
        else if (_currentFPS > 30)
            _fpsText.color = Color.yellow;
        else
            _fpsText.color = Color.red;
        _fpsText.text = "FPS: " + Mathf.RoundToInt(_currentFPS);
    }
}
