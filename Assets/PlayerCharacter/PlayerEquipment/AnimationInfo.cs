// #AnimationInfo
using UnityEngine;

public class AnimationInfo
{
    public string Animation { get; set; }
    public float Duration { get; set; }
    public float HitBoxOnTime { get; set; }
    public float HitBoxOffTime { get; set; }
    public float SwingSoundTime { get; set; }
    public AudioClip SwingSound { get; set; }
    public AudioClip ImpactSound { get;  set; }
    public float Stamina { get; set; }

    public AnimationInfo(string animation, float duration, float hitBoxOnTime, float hitBoxOffTime, float swingSoundTime, AudioClip swingSound, AudioClip impactSound,float stamina)
    {
        Animation = animation;
        Duration = duration;
        HitBoxOnTime = hitBoxOnTime;
        HitBoxOffTime = hitBoxOffTime;
        SwingSoundTime = swingSoundTime;
        SwingSound = swingSound;
        ImpactSound = impactSound;
        Stamina = stamina;
    }

}
