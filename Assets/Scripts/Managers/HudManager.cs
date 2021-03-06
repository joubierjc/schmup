﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class HudManager : UnityFakeSingleton<HudManager> {

	[Header("Settings")]
	public float transitionsDuration;

	[Header("UI Elements")]
	public TextMeshProUGUI scoreText;
	public Slider healthBarSlider;
	public TextMeshProUGUI healthText;
	public Slider energyBarSlider;
	public TextMeshProUGUI energyText;
	public TextMeshProUGUI specialText;
	public GameObject PlayGroup;
	public GameObject EndGameGroup;
	public TextMeshProUGUI EndScoreText;

	private Tween scoreTween;
	private Tween healthBarTween;
	private Tween energyBarTween;

	public void ChangeHealthDisplay(float newValue) {
		healthBarTween.Kill();
		healthBarTween = DOTween.To(() => healthBarSlider.value, x => healthBarSlider.value = x, newValue, transitionsDuration);
		RefreshHealthText(newValue);
	}

	public void ChangeEnergyDisplay(float newValue) {
		energyBarTween.Kill();
		energyBarTween = DOTween.To(() => energyBarSlider.value, x => energyBarSlider.value = x, newValue, transitionsDuration);
		RefreshEnergyText(newValue);
	}

	public void RefreshScoreText() {
		scoreText.SetText("{0:0}", GameManager.Instance.score);
	}

	public void RefreshEndScoreText() {
		EndScoreText.SetText("{0:0}", GameManager.Instance.score);
	}

	public void RefreshHealthText(float newValue) {
		healthText.SetText("{0:0}", newValue);
	}

	public void RefreshEnergyText(float newValue) {
		energyText.SetText("{0:0}", newValue);
	}

	public void RefreshSpecialText(string newValue) {
		specialText.text = newValue;
	}

	public void DisplayEndGame() {
		if (PlayGroup) {
			PlayGroup.SetActive(false);
		}
		if (EndGameGroup) {
			EndGameGroup.SetActive(true);
		}
		if (EndScoreText) {
			RefreshEndScoreText();
		}
	}
}
