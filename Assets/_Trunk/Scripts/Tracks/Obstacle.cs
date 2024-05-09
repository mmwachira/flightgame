using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    public AudioClip _impactedSound;

    public virtual void Setup()
    {
    }

    public abstract void Spawn(TrackSegment segment, float t);

    public virtual void Impacted()
    {
        //Animation anim = GetComponentInChildren<Animation>();For when we add animations to obstacles hit
        //if (anim != null)
        //{
        //    anim.Play();
        //}

        if (_impactedSound != null)
        {
            ManagerSounds.Instance.PlaySingle(_impactedSound, true);
        }
    }
}