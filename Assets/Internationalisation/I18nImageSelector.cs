
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
struct I18nImageData
{
	public Language language;
	public Sprite sprite;
	public Sprite buttonActiveSprite;
}

public class I18nImageSelector : MonoBehaviour
{
	[SerializeField] private List<I18nImageData> options = new List<I18nImageData>();

	private SpriteRenderer spriteRenderer;
	private Image image;
	private Button button;

	private void Awake()
	{
		MenuOptionsController.Instance.onLanguageChanged += SetSprite;
	}

	protected void Start()
	{
		image = GetComponent<Image>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		button = GetComponent<Button>();
		SetSprite();
	}

	private void SetSprite()
	{
		I18nImageData op = options.Where(x => x.language == (Language)Enum.Parse(typeof(Language), GameData.Language)).FirstOrDefault();
		if (image != null)
			image.sprite = op.sprite;
		else if (spriteRenderer != null)
			spriteRenderer.sprite = op.sprite;

		if (button != null && op.buttonActiveSprite != null)
		{
			SpriteState states;
			states.highlightedSprite = op.buttonActiveSprite;
			states.pressedSprite = op.buttonActiveSprite;
			states.selectedSprite = op.buttonActiveSprite;

			button.spriteState = states;
		}
	}

	private void OnDestroy()
	{
		MenuOptionsController.Instance.onLanguageChanged -= SetSprite;
	}
}
