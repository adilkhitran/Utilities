using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace KHiTrAN
{
    public class UserPrefs // TempUserPrefs
    {
        public static bool isMusic, isSound;
    }

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
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private bool soundPreviousStatus;
        private bool musicPreviousStatus;

        private bool isAdDisplayed = false;

        public void OnAdDisplay()
        {
            isAdDisplayed = true;
            soundPreviousStatus = UserPrefs.isSound;
            musicPreviousStatus = UserPrefs.isMusic;
        }

        public void OnAdClose()
        {
            if (isAdDisplayed)
            {
                isAdDisplayed = false;
                UserPrefs.isSound = soundPreviousStatus;
                UserPrefs.isMusic = musicPreviousStatus;
            }
        }

        void Start()
        {
            audioClipsNames = new List<string>();

            foreach (var clip in clips)
            {
                audioClipsNames.Add(clip.name.ToString());
            }

            if (!UserPrefs.isMusic)
            {
                MuteMusic();
            }
            if (!UserPrefs.isSound)
            {
                MuteSound();
            }
        }

        void OnEnable()
        {
            //GameManager.onStateChange += OnStateChangeAction;
        }

        void OnDisable()
        {
            // GameManager.onStateChange -= OnStateChangeAction;
        }

        //public void OnStateChangeAction(GameState state)
        //{
        //    if (audioClipsNames == null || audioClipsNames.Count <= 0)
        //    {
        //        Start();
        //    }
        //    if (state == GameState.LOADING || state == GameState.PAUSED)
        //    {
        //        MuteSound();
        //        MuteMusic();
        //    }
        //    else
        //    {
        //        if ((state == GameState.GAMEPLAY && GameManager.Instance.previousState == GameState.PAUSED) || state != GameState.GAMEPLAY)
        //        {
        //            if (UserPrefs.isSound)
        //            {
        //                UnMuteSound();
        //            }
        //            if (UserPrefs.isMusic)
        //            {
        //                UnMuteMusic();
        //            }
        //        }
        //    }

        //    if (state == GameState.GAMEPLAY)
        //    {
        //        if (UserPrefs.isMusic)
        //        {
        //            UnMuteMusic();

        //            var index = audioClipsNames.IndexOf(SoundNames.GAMEPLAYBG.ToString());

        //            if (index >= 0 && backgroundMusic.clip != clips[index].clip)
        //                backgroundMusic.clip = clips[index].clip;
        //            if (!backgroundMusic.isPlaying)
        //            {
        //                backgroundMusic.Play();
        //            }
        //        }

        //        if (UserPrefs.isSound)
        //        {
        //            UnMuteSound();
        //        }
        //    }
        //    else if (state == GameState.LEVELCOMPLETE)
        //    {
        //        MuteSoundAndMusicOtherThanPopup();
        //        if (UserPrefs.isSound)
        //        {
        //            var index = audioClipsNames.IndexOf(SoundNames.LEVELCOMPLETE.ToString());
        //            if (index >= 0)
        //                popupSounds.PlayOneShot(clips[index].clip);
        //        }
        //    }
        //}

        #region Mute_Unmute

        public void MuteMusic()
        {
            backgroundMusic.mute = true;
        }

        public void MuteSound()
        {
            soundEffect.mute = true;
            soundEffect.Stop();

            popupSounds.mute = true;
            popupSounds.Stop();

            loopingSounds.mute = true;
            loopingSounds.Stop();
        }

        public void MuteSoundAndMusicOtherThanPopup()
        {
            backgroundMusic.mute = true;
            backgroundMusic.Stop();

            soundEffect.mute = true;
            soundEffect.Stop();

            loopingSounds.mute = true;
            loopingSounds.Stop();
        }

        public void UnmuteSoundAndMusicOtherThanPopup()
        {
            if (UserPrefs.isSound && backgroundMusic.mute)
            {
                backgroundMusic.mute = false;
                if (!backgroundMusic.isPlaying)
                    backgroundMusic.Play();
            }

            if (UserPrefs.isMusic)
            {
                soundEffect.mute = false;
                loopingSounds.mute = false;
            }
        }

        public void UnMuteMusic()
        {
            //if (backgroundMusic.mute && GameManager.Instance.currentState != GameState.PAUSED)
            //{
            backgroundMusic.mute = false;
            if (!backgroundMusic.isPlaying)
                backgroundMusic.Play();
            //}
        }

        public void UnMuteSound()
        {
            //if (GameManager.Instance.currentState != GameState.PAUSED)
            //{
            soundEffect.mute = false;
            popupSounds.mute = false;
            loopingSounds.mute = false;
            //}
        }

        public void StopLoopingSound()
        {
            loopingSounds.Stop();
        }

        #endregion

        public void PlayBGMusic(SoundNames clipName)
        {
            int index = audioClipsNames.IndexOf(clipName.ToString());
            if (index >= 0)
            {
                var clip = clips[index].clip;
                PlayBGMusic(clip);

            }
        }

        public void PlayBGMusic(AudioClip clip)
        {
            if (UserPrefs.isMusic)
            {
                backgroundMusic.clip = clip;
                backgroundMusic.Play();
            }
        }



        public void PlaySoundEffect(SoundNames clipName, bool isPopup, float delay)
        {
            StartCoroutine(PlaySoundEffectsEnum(clipName, isPopup, delay));
        }

        public void PlaySoundEffect(SoundNames clipName, float delay)
        {
            StartCoroutine(PlaySoundEffectsEnum(clipName, false, delay));
        }

        private IEnumerator PlaySoundEffectsEnum(SoundNames clipName, bool isPopup, float delay)
        {
            yield return new WaitForSeconds(delay);
            PlaySoundEffect(clipName, isPopup);
        }



        public void PlaySoundEffect(SoundNames clipName)
        {
            if (UserPrefs.isSound)
            {
                int index = audioClipsNames.IndexOf(clipName.ToString());
                if (index > -1)
                {
                    SoundClip clip = clips[index];
                    if (clip.type == SoundType.OneShot)
                    {
                        soundEffect.PlayOneShot(clip.clip);
                    }
                    else
                    {
                        if (!loopingSounds.isPlaying)
                        {
                            loopingSounds.clip = clip.clip;
                            loopingSounds.Play();
                        }
                    }
                }
            }
        }

        public void PlaySoundEffect(SoundNames clipName, bool isPopup)
        {
            if (!isPopup)
            {
                PlaySoundEffect(clipName);
            }
            else
            {
                if (UserPrefs.isSound)
                {
                    int index = audioClipsNames.IndexOf(clipName.ToString());
                    if (index > -1)
                    {
                        SoundClip clip = clips[index];
                        if (clip.type == SoundType.OneShot)
                        {
                            if (popupSounds.loop)
                            {
                                popupSounds.loop = false;
                                popupSounds.Stop();
                            }
                            popupSounds.PlayOneShot(clip.clip);
                        }
                        else
                        {
                            popupSounds.clip = clip.clip;
                            popupSounds.loop = true;
                            popupSounds.Play();
                        }
                    }
                }
            }
        }

        public AudioClip GetAudioClip(SoundNames clipName)
        {
            var index = audioClipsNames.IndexOf(name.ToString());
            if (index >= 0)
                return clips[audioClipsNames.IndexOf(name.ToString())].clip;
            else
                return null;
        }



        bool isAssigned = false;
        private bool isMusic, isSound;

        private bool isAdShowing = false;

        [ContextMenu("OnAdIsShowing")]
        public void OnAdIsShowing()
        {
            isAdShowing = true;
            if (!isAssigned)
            {
                isMusic = UserPrefs.isMusic;
                isSound = UserPrefs.isSound;
                isAssigned = true;
            }
            UserPrefs.isMusic = false;
            UserPrefs.isSound = false;
            MuteMusic();
            MuteSound();
            CancelInvoke("StopGame");
            Invoke("StopGame", 0.3f);
            Debug.Log("GT >> Shop Sound ON AD SHOW");
        }

        private void StopGame()
        {
            Time.timeScale = 0;
            Debug.Log("GT >> Set TimeScale Zero");
        }

        [ContextMenu("OnAdIsClose")]
        public void OnAdIsClose()
        {
            CancelInvoke("StopGame");
            isAdShowing = false;
            if (isAssigned)
            {
                isAssigned = false;
                UserPrefs.isMusic = isMusic;
                UserPrefs.isSound = isSound;
            }
            Time.timeScale = 1;
            UnMuteMusic();
            UnMuteSound();
            Debug.Log("GT >> Play Sound ON AD SHOW");
        }
    }

    [System.Serializable]
    public struct SoundClip
    {
        public SoundNames name;
        public AudioClip clip;
        public SoundType type;
    }
}