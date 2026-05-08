
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [SerializeField] private Transform _iconContainer;
    [SerializeField] private Transform _iconTemplate;


    private void Awake()
    {
        _iconTemplate.gameObject.SetActive(false);

    }

    public void SetRecipeSO(RecipeSO recipeSO)
    {
        _textMeshProUGUI.text = recipeSO.recipeName;

        foreach (Transform child in _iconContainer)
        {
            if (child != _iconTemplate)
            {
                Destroy(child.gameObject);
            }

        }
        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
        {
            Transform iconTransform = Instantiate(_iconTemplate, _iconContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }
    }
}
