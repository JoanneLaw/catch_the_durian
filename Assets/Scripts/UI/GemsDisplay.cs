using TMPro;
using UnityEngine;

public class GemsDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountText = null;

    void Start()
    {
        UpdateAmountText();

        PlayerManager.instance.onGemUpdated += UpdateAmountText;
    }

    private void UpdateAmountText()
    {
        amountText.SetText(PlayerManager.instance.Gem.ToString("N0"));
    }
}
