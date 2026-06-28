using TMPro;
using UnityEngine;

public class UICategory : MonoBehaviour
{
    [SerializeField] private TMP_Text categoryName;

    public void Init(string categoryName) {
        this.categoryName.text = categoryName;
    }
}
