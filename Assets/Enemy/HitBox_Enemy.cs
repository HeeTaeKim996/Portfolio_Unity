
using UnityEngine;

public class HitBox_Enemy : MonoBehaviour
{
    protected Enemy enemy;
    [HideInInspector]
    public AudioClip hitAudioClip;
    [HideInInspector]
    public float hitAudioVolume;
    [HideInInspector]
    public DamageInfo damageInfo;
    protected LayerMask targetLayers;

    protected virtual void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        targetLayers = CommonMethods.GetStringsToLayerMask(enemy.stringsOfAttackTarget);
    }


    protected virtual void OnEnable()
    {

    }
    protected virtual void OnDisable()
    {
        damageInfo = null;
    }

    public void SetImpactSound(AudioClip newAudioClip, float newVolume)
    {
        hitAudioClip = newAudioClip;
        hitAudioVolume = newVolume;
    }
}
