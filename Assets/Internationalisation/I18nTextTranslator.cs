using TMPro;
using UnityEngine;

public class I18nTextTranslator : MonoBehaviour
{
	public string TextId;

	private TextMeshProUGUI text;

	private void Awake()
	{
		MenuOptionsController.Instance.onLanguageChanged += SetText;
	}

	protected void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
		SetText();
	}

	private void SetText()
	{
		if (text != null)
			text.text = I18n.Fields[TextId];
	}

	private void OnDisable()
	{
		MenuOptionsController.Instance.onLanguageChanged -= SetText;
	}
}