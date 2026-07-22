using UnityEngine;
using UnityEngine.UI;

public enum HudStat { Health, Mana, Experience }

public class HudPanel : MonoBehaviour
{
    public Slider healthBar;
    public Slider manaBar;
    public Slider xpBar;

    public Text levelValue;

    public Text roomCounterText;
    public Text metaXpText;

    private float lastUpdateTime;

    void LateUpdate()
    {
        if (Time.unscaledTime - lastUpdateTime < 0.1f) return;
        lastUpdateTime = Time.unscaledTime;
        Refresh();
    }

    public void Refresh()
    {
        if (GameManager.Instance == null) return;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health hp = player.GetComponent<Health>();
            if (hp != null)
            {
                if (healthBar != null)
                {
                    healthBar.maxValue = hp.MaxHealth;
                    healthBar.value = hp.CurrentHealth;
                }
            }
        }

        CharacterData character = GameManager.Instance.GetCharacterSnapshot();
        if (character == null) return;

        if (levelValue != null) levelValue.text = $"Lv {character.characterLevel}";

        if (GameManager.Instance.CurrentRun != null && roomCounterText != null)
        {
            roomCounterText.text = $"Room {GameManager.Instance.CurrentRun.roomIndex + 1}";
        }
        else if (roomCounterText != null) roomCounterText.text = "";

        if (manaBar != null)
        {
            manaBar.maxValue = Mathf.Max(1f, character.characterMana);
            manaBar.value = character.characterMana;
        }

        if (xpBar != null && GameManager.Instance.GetLevelCurve() != null)
        {
            LevelCurve curve = GameManager.Instance.GetLevelCurve();
            int next = curve.XpForNextLevel(character.characterLevel);
            xpBar.maxValue = Mathf.Max(1, next);
            xpBar.value = character.characterExperience;
        }

        if (metaXpText != null) metaXpText.text = $"MetaXP {character.metaXp}";
    }
}
