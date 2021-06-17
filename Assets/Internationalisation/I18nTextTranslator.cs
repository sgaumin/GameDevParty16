using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class I18nTextTranslator : MonoBehaviour
{
	public string TextId;

	private TextMeshProUGUI tmpText;
	private Text plainText;

	private void Awake()
	{
		MenuOptionsController.Instance.onLanguageChanged += SetText;
	}

	protected void Start()
	{
		tmpText = GetComponent<TextMeshProUGUI>();
		plainText = GetComponent<Text>();
		SetText();
	}

	private void SetText()
	{
		if (tmpText != null)
			tmpText.text = I18n.Fields[TextId];
		else if (plainText != null)
			plainText.text = I18n.Fields[TextId];
	}

	private void OnDestroy()
	{
		MenuOptionsController.Instance.onLanguageChanged -= SetText;
	}
}