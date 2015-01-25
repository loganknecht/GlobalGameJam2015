using UnityEngine;
using System.Collections;

/// <summary>
/// You're going to forget why you created this in the first place.
/// The unity audio source class is just terrible in providing information that lets you knw when it finished. Because of this
/// you created a wrapper that allows you to track the logic of the audio source as it plays through.
/// Typically the audio source's isPlaying variable could be relied on to know if it's playing, but on focus changes and 
/// when paused the audio source isPlaying variable is set to false. This means it's unreliable for determining state.
/// On top of this to determine if it finished you could try waiting for isPlaying to return false, but again its state is unreliable.
/// You could also wait to determine if the time has finished, but it resets back to 0 when it has finished. This would be fine if you 
/// could determine if the audio source had started, but you can't. SO in order to make tracking the logic easier you created this 
/// wrapper for tracking when the audio object is started, when it finishes, and so forth, so that outside logic can infer when to
/// use the audio object itself.
/// </summary>
public class AudioSourceManager : MonoBehaviour {
    public AudioSource audioSourceReference = null;
    public bool hasStartedPlaying = false;
    public bool hasFinished = false;

	// Use this for initialization
	void Start () {
        audioSourceReference = this.gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        // Debug.Log("-----------------------");
        // Debug.Log("Sample: " + audioSourceReference.clip.samples);
        // Debug.Log("Frequency: " + audioSourceReference.clip.frequency);
        // Debug.Log("Seconds: " + audioSourceReference.clip.samples/audioSourceReference.clip.frequency);
        // Debug.Log("Current Time: " + audioSourceReference.time);
        if(Input.GetKeyDown(KeyCode.P)) {
            Pause();
        }

        PerformStateLogic();
	}

    public void PerformStateLogic() {
        if(hasStartedPlaying == false
            && audioSourceReference.isPlaying) {
            hasStartedPlaying = true;
        }

        if(hasStartedPlaying
            && audioSourceReference.time == 0
            && audioSourceReference.isPlaying == false) {
            hasFinished = true;
        }
    }

    public void SetClip(AudioClip newAudioClip) {
        audioSourceReference.clip = newAudioClip;
    }

    public void SetLoop(bool loop) {
        audioSourceReference.loop = loop;
    }

    public void Play(ulong delay = 0) {
        audioSourceReference.Play(delay);
    }

    public void PlayDelayed(float delay) {
        audioSourceReference.PlayDelayed(delay);
    }

    public void Pause() {
        audioSourceReference.Pause();
    }

    public void Stop() {
        audioSourceReference.Stop();
    }
}
