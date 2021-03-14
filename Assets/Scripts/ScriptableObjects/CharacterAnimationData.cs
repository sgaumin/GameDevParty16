using DG.Tweening;
using Tools.Utils;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CharacterAnimationData", order = 1)]
public class CharacterAnimationData : ScriptableObject
{
	[FloatRangeSlider(0f, 2f)] public FloatRange lookAtDuration = new FloatRange(0.8f, 1.2f);
	public Ease lookAtEase = Ease.OutSine;
	[Space]
	[FloatRangeSlider(0f, 5f)] public FloatRange raiseHeightValue = new FloatRange(1f, 1.5f);
	[FloatRangeSlider(0f, 2f)] public FloatRange raiseDuration = new FloatRange(0.8f, 1.2f);
	public Ease raiseEase = Ease.OutBounce;
	[Space]
	[FloatRangeSlider(0f, 2f)] public FloatRange moveDuration = new FloatRange(1f, 1.5f);
	public AnimationCurve moveAnimation;
	[Space]
	[FloatRangeSlider(0f, 1f)] public FloatRange cameraShakeDuration = new FloatRange(0.25f, 0.35f);
	[FloatRangeSlider(0f, 10f)] public FloatRange cameraShakeStrenght = new FloatRange(2.6f, 3.2f);
	[IntRangeSlider(5, 15)] public IntRange cameraShakeVibrato = new IntRange(9, 11);
	public float cameraShakeAdjustmentTiming = 0.8f;
	[Space]
	[FloatRangeSlider(0f, 2f)] public FloatRange strechYFactor = new FloatRange(1.02f, 1.08f);
	[FloatRangeSlider(0f, 2f)] public FloatRange strechYFrequency = new FloatRange(1f, 1.5f);
}