using Assets.GracesScripts.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#nullable enable

/// <summary>
/// Unit class, Player and NPCs both have abilities an equipped weapon and item and health.
/// </summary>
[RequireComponent(typeof(UseAnimatedLayers))]
public abstract class Unit : MonoBehaviour
{
    public float currentHealth = 100;
    public float maxHealth = 100;
    protected Weapon DefaultWeaponHands => GetDefaultHands();
    public List<Ability> Abilities => this.equippedWeapon == null ? DefaultWeaponHands.Abilities : this.equippedWeapon.Abilities;
    protected List<DungeonItem?> EquippedItems => new() { this.equippedWeapon, this.equippedSpecialItem };
    public Weapon equippedWeapon;
    public SpecialItem? equippedSpecialItem;
    public List<DungeonItem> Inventory = new();
    // this to go in Unit in a battle
    [HideInInspector] public GameObject HealthBarObject;
    [HideInInspector] public Image healthBarFill;

    [SerializeField] public int basePower = 0;
    public int powerModifier;

    [SerializeField] public int baseDefence = 0;

    /// <summary>
    /// Will be changed from one off items to buffing abilities
    /// </summary>
    public int defenceModifier = 0;

    protected UseAnimatedLayers? animatedLayers;
    [SerializeField] private Material? whiteFlashMaterial;
    private List<SpriteRenderer> unitsSpriteLayers = new();
    private MaterialPropertyBlock _block;
    private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");

    private void Awake()
    {
        animatedLayers = this.gameObject.GetComponent<UseAnimatedLayers>();
        MyGuard.IsNotNull(animatedLayers, $"null animated layers {this.name}");
        MyGuard.IsNotNull(whiteFlashMaterial, $"White flash material not on {this.name}");
        foreach (var sr in this.GetComponentsInChildren<SpriteRenderer>())
        {
            MyGuard.IsNotNull(sr, $"sprite renderer null on {this.name}");
            sr.material = this.whiteFlashMaterial;
            unitsSpriteLayers.Add(sr);
        }

        _block = new MaterialPropertyBlock();
    }

    IEnumerator FlashRoutineAndHurtAnimation(float duration)
    {
        yield return new WaitForSeconds(0.3f);
        var timeStart = Time.time;
        MyGuard.IsNotNull(this.animatedLayers);
        this.animatedLayers.SetTriggers("Hurt");

        while (Time.time < timeStart + duration)
        {
            ChangeSpriteFlash(1f);
            yield return new WaitForSeconds(0.05f);
            ChangeSpriteFlash(0f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>
    /// flash value of 0 for regular colour and 1 for white.
    /// </summary>
    /// <param name="flashValue"></param>
    private void ChangeSpriteFlash(float flashValue)
    {
        foreach (var layer in this.unitsSpriteLayers)
        {
            _block.Clear();
            layer.GetPropertyBlock(_block);
            _block.SetFloat(FlashAmount, flashValue);
            layer.SetPropertyBlock(_block);
        }
    }

    private Weapon GetDefaultHands()
    {
        var hands = Resources.Load<Weapon>("Items/Weapon/Hands");
        MyGuard.IsNotNull(hands);
        return hands;
    }

    /// <summary>
    /// Shake health bar and animate health bar fill.
    /// </summary>
    public void TakeDamage(float value)
    {
        StartCoroutine(FlashRoutineAndHurtAnimation(2f));
        Debug.Log("Check how the timing of StartShake and damage bar reducing works i dont remember but rewrite it so its clearly aligned.");
        var objectToShake = this.HealthBarObject.GetComponent<ShakeObject>();
        objectToShake.StartShake(1f, 5f);
        this.currentHealth -= value;
        // if current damage will kill Unit make it go down faster
        if (this.currentHealth <= 0)
        {
            StartCoroutine(AnimateHealthBarFill(0.1f, -value));
            this.Die();
        }
        else
        {
            StartCoroutine(AnimateHealthBarFill(0.5f, -value));
        }
    }

    /// <summary>
    /// Add health to this unit and animate health bar fill increasing.
    /// </summary>
    /// <param name="healAmount"></param>
    public void Heal(float healAmount)
    {
        Debug.Log("Heal play heal animation");
        this.currentHealth += healAmount;
        StartCoroutine(AnimateHealthBarFill(0.5f, healAmount));
    }

    protected abstract void Die();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animationLength">seconds time for health bar to go to where after damage puts it</param>
    /// <param name="healthChange">positive value for healing negative for damaging.</param>
    /// <returns></returns>
    private IEnumerator AnimateHealthBarFill(float animationLength, float healthChange)
    {
        float startFillAmount = this.healthBarFill.fillAmount;
        float targetFillAmount = Mathf.Clamp((this.currentHealth / this.maxHealth), 0, 1);

        float timeIncrement = 0.1f;
        float totalSteps = animationLength / timeIncrement;
        float fillChangePerStep = (targetFillAmount - startFillAmount) / totalSteps;

        while (Mathf.Abs(this.healthBarFill.fillAmount - targetFillAmount) > 0.01f)
        {
            this.healthBarFill.fillAmount += fillChangePerStep;
            yield return new WaitForSeconds(timeIncrement);
        }

        // Ensure we end exactly at target
        this.healthBarFill.fillAmount = targetFillAmount;
    }
}
