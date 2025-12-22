using UnityEngine;
using UnityEngine.Audio;

public abstract class AbilitySO : ScriptableObject
{
    public string abilityName;
    public float cooldown;
    public Sprite Icon;
    [HideInInspector] public float cooldownTimer = 0f;

    public AudioClip activationSound;
    public AudioMixerGroup audioFxGroup;
    public float soundVolume = 0.7f;
    [Range(0f, 0.5f)] public float pitchVariation = 0.1f;

    public bool manualSoundControl = false;

    private void OnEnable()
    {
        cooldownTimer = 0f;
    }

    public void TickCooldown(float deltaTime)
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= deltaTime;
        }
          
    }

    public bool IsReady()
    {
        return cooldownTimer <= 0f;
    }

    public void UseAbility(GameObject user)
    {
        if (!IsReady()) return;

        Vector2 aimDir = GetAimDirection(user);

        if (!manualSoundControl)
            PlaySound(user.transform.position);
            

        ExecuteAbility(user, aimDir);
    }

    protected virtual Vector2 GetAimDirection(GameObject user)
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (Vector2)mouseWorld - (Vector2)user.transform.position;
        return dir.normalized;
    }

    protected void TriggerCooldown()
    {
        cooldownTimer = cooldown;
    }

    protected abstract void ExecuteAbility(GameObject user, Vector2 aimDir);

    public void PlaySound(Vector2 position)
    {
        AudioManager.Instance.PlayClip(activationSound);
    }
}
