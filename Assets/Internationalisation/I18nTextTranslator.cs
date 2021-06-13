using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class I18nTextTranslator : MonoBehaviour
{
    public string TextId;
    //private Text text;
    private TextMeshProUGUI text;

    // Use this for initialization
    private void Awake()
    {
        Debug.Log($"I18nTextTranslator awake: {MenuOptionsController.Instance}");
        MenuOptionsController.Instance.onLanguageChanged += SetText;
        text = GetComponent<TextMeshProUGUI>();
        SetText();
    }

    private void SetText()
    {
        Debug.Log($"ici: {text == null}");
        if (text != null)
            if(TextId == "ISOCode")
                text.text = I18n.GetLanguage();
            else
                text.text = I18n.Fields[TextId];
    }

    private void OnDisable()
    {
        MenuOptionsController.Instance.onLanguageChanged -= SetText;
    }

}