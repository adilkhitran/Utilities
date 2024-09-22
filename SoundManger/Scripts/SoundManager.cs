using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace KHiTrAN
{
    public class SoundManager : MonoBehaviour
    {

        public static SoundManager Instance;
        public AudioSource backgroundMusic, soundEffect, popupSounds, loopingSounds;

        public List<SoundClip> clips = new List<SoundClip>();

        private List<string> audioClipsNames;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                this.transform.SetParent(null);
                DontDestroyOnLoad(this.gameObject);

                audioClipsNames = new List<string>();

                foreach (var clip in clips)
                {
                    audioClipsNames.Add(clip.name.ToString());
                }
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        void Start()
        {
            CancelInvoke();
            Invoke("Init", 1);
        }

        private void Init()
        {
            ToggleMusic(GlobalVariables.GameData.Settings.Music);
            ToggleSound(GlobalVariables.GameData.Settings.Sounds);
        }

        #region Mute_Unmute
        public void ToggleMusic(bool isOn)
        {
            if (isOn)
            {
                backgroundMusic.mute = false;
                if (!backgroundMusic.isPlaying)
                    backgroundMusic.Play();
            }
            else
            {
                backgroundMusic.mute = true;
            }
        }

        public void ToggleSound(bool isOn)
        {
            soundEffect.mute = !isOn;
            popupSounds.mute = !isOn;
            loopingSounds.mute = !isOn;

            if (!isOn)
            {
                soundEffect.Stop();
                popupSounds.Stop();
                loopingSounds.Stop();
            }
        }
        #endregion

        public AudioClip GetAudioClip(SoundNames clipName)
        {
            var index = audioClipsNames.IndexOf(name.ToString());
            if (index >= 0 && clips[index].clip.Length > 0)
            {
                if (clips[index].clip.Length > 1)
                {
                    return clips[index].clip[Random.Range(0, clips[index].clip.Length)];
                }
                else
                    return clips[index].clip[0];
            }
            else
                return null;
        }

        public void PlayBGMusic(SoundNames clipName)
        {
            int index = audioClipsNames.IndexOf(clipName.ToString());
            if (index >= 0)
            {
                PlayAudioSound(backgroundMusic, index);
            }
            backgroundMusic.mute = !GlobalVariables.GameData.Settings.Music;
        }

        public void PlaySoundEffect(SoundNames clipName, float delay = 0)
        {
            if (delay <= 0)
            {
                PlaySoundEffect(clipName, false);
            }
            else
                StartCoroutine(PlaySoundEffectsEnum(clipName, false, delay));
        }

        private IEnumerator PlaySoundEffectsEnum(SoundNames clipName, bool isPopup, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlaySoundEffect(clipName, isPopup);
        }

        private void PlaySoundEffect(SoundNames clipName, bool isPopup)
        {
            if (!GlobalVariables.GameData.Settings.Sounds)
                return;

            int index = audioClipsNames.IndexOf(clipName.ToString());
            if (index < 0)
                return;

            if (isPopup)
            {
                PlayAudioSound(popupSounds, index);
            }
            else
            {
                if (clips[index].type == SoundType.OneShot)
                {
                    PlayAudioSound(soundEffect, index);
                }
                else
                {
                    if (!loopingSounds.isPlaying)
                    {
                        PlayAudioSound(loopingSounds, index);
                    }
                }

            }
        }

        private void PlayAudioSound(AudioSource source, int index)
        {
            if (clips[index].audioTime > 0)
            {
                float timePassed = Time.time - clips[index].lastPlayTime;

                if (timePassed > clips[index].audioTime)
                {
                    SoundClip clip = clips[index];
                    clip.lastPlayTime = Time.time + Random.Range(-clip.audioTime * 0.2f, clip.audioTime * 0.2f);

                    clips[index] = clip;
                }
                else
                    return;

            }

            if (clips[index].type == SoundType.OneShot)
            {
                if (source.loop)
                {
                    source.loop = false;
                    source.Stop();
                }

                if (clips[index].clip.Length > 0)
                {
                    AudioClip clip;
                    if (clips[index].clip.Length > 1)
                    {
                        clip = clips[index].clip[Random.Range(0, clips[index].clip.Length)];
                    }
                    else
                        clip = clips[index].clip[0];

                    source.PlayOneShot(clip);
                }
            }
            else
            {
                if (clips[index].clip.Length > 0)
                {
                    source.loop = true;

                    if (clips[index].clip.Length > 1)
                    {
                        source.clip = clips[index].clip[Random.Range(0, clips[index].clip.Length)];
                    }
                    else
                        source.clip = clips[index].clip[0];
                    source.Play();
                }
            }
        }
    }

    [System.Serializable]
    public struct SoundClip
    {
        public SoundNames name;
        public float audioTime;
        public AudioClip[] clip;
        public SoundType type;

        public float lastPlayTime;
    }
}