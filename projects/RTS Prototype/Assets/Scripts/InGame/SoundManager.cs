using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;


/*
TODO:
- I have to care about the numbers of voices
- Also it must be easy to change the volumes of independable sounds. (Do this with Submixers in the MainMixer!)
Code needs to be updated for that (like if you find a group with the same filename in the SFX group (e.g.)
than use that value if not use the SFX Mixer group
- Stoping sounds should just "pause" them
and there should be an option to "delete" all sounds
*/


public class SoundManager : MonoBehaviour
{
    [ReadOnly]
    public AudioMixer mixer;

    private static SoundManager _instance;
    public static SoundManager instance
    {
        get
        {
            if (!_instance)
            {
                SoundManager foundManager = FindObjectOfType<SoundManager>();
                // We never set the instance so create a new SoundManager!
                if (foundManager)
                {
                    // Use the one you just found.
                    _instance = foundManager;

                }
                else
                {
                    // Create a new one!

                    // Is there a _SCRIPTS_ Object in the Scene?
                    // This is only neded for organization.
                    GameObject scripts = GameObject.Find("_SCRIPTS_");
                    if (!scripts)
                    {
                        Debug.LogError("Error, there is no GameObject in this Scene named: '_SCRIPTS_'");
                        Debug.Break();
                    }
                    GameObject instanceGO = new GameObject();
                    instanceGO.transform.parent = scripts.transform;
                    instanceGO.transform.name = "Sound Manager";
                    _instance = instanceGO.AddComponent<SoundManager>();
                }

                _instance.mixer = (AudioMixer)Resources.Load("AudioMixer/Main Mixer");
                if (!_instance.mixer)
                {
                    Debug.LogError("Error, there is no Audio Mixer called 'Main Mixer', that is located at: "
                        + "Ressources/AudioMixer/");
                    Debug.Break();
                }
            }

            return _instance;
        }
    }

    public enum SoundType
    {
        SFX,
        Music,
        Background
    }

    public void Play(string clipname, SoundType type, bool loop)
    {
        // These are all the clips that unity has already loaded
        AudioClip[] loadedClips = Resources.FindObjectsOfTypeAll<AudioClip>();
        // Does this clip already exist?
        foreach (AudioClip currClip in loadedClips)
        {
            if (currClip.name == clipname)
            {
                // Yes, so I just need to play it!
                Play(currClip, type, loop);
                return;
            }
        }

        // If you're still here you need to load that Sound first!
        // load the clip
        AudioClip newClip = (AudioClip)Resources.Load("Audio/" + clipname);
        // Now Play it!
        Play(newClip, type, loop);
    }

    public void Play(AudioClip clip, SoundType type, bool loop)
    {
        // Instantiate one SoundObject!
        GameObject soundObjectChild = new GameObject();
        soundObjectChild.AddComponent<AudioSource>();
        soundObjectChild.AddComponent<SoundObject>();
        soundObjectChild.transform.name = "SoundObject";
        // set it as a child of the soundmanager
        soundObjectChild.transform.parent = transform;

        // Set the Soundtype on the SoundObject
        soundObjectChild.GetComponent<SoundObject>().type = type;

        // Set the Audiosources audiogroup
        soundObjectChild.GetComponent<AudioSource>().outputAudioMixerGroup = getMixerFromType(type);
        soundObjectChild.GetComponent<AudioSource>().playOnAwake = false;

        // get the AudioSource of this child and set the clip aswell as the looping state
        soundObjectChild.GetComponent<AudioSource>().loop = loop;
        soundObjectChild.GetComponent<AudioSource>().clip = clip;
        soundObjectChild.GetComponent<AudioSource>().Play();
    }

    public void StopAllSoundsOfType(SoundType type)
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<SoundObject>().type == type)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void StopAllSounds()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public bool areSoundsPlaying()
    {
        for (int i = 0; i < transform.childCount;)
        {
            return true;
        }


        return false;
    }

    private AudioMixerGroup getMixerFromType(SoundType type)
    {
        foreach (AudioMixerGroup currGroup in mixer.FindMatchingGroups(""))
        {
            if (type.ToString() == currGroup.name)
            {
                return currGroup;
            }
        }

        // if you're still here then we couldn't find the right AudioMixerGroup!
        Debug.LogAssertion("You specified a Soundtype that has not an Group assigned in the "
        + "getMixerFromType function of SoundManager.", this);
        Debug.Break();
        return null;
    }

}
