using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace BurgerGame
{
    public enum BGMTrack
    {
        None,
        Menu,
        Masak
    }

    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Object.FindAnyObjectByType<AudioManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("AudioManager");
                        go.transform.position = new Vector3(0, 50f, 0);
                        instance = go.AddComponent<AudioManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                if (instance != null)
                {
                    instance.EnsureAudioSources();
                }
                return instance;
            }
        }

        [Header("Audio Clips - BGM")]
        public AudioClip clipMainMenu;
        public AudioClip clipMasak;

        [Header("Audio Clips - SFX")]
        public AudioClip clipDimasak;
        public AudioClip clipMatang;
        public AudioClip clipOrderanBaru;
        public AudioClip clipOrderServed;
        public AudioClip clipPotong;
        public AudioClip clipButtonClick;

        [Header("Audio Settings")]
        [Range(0f, 1f)]
        public float bgmVolume = 0.5f;
        [Range(0f, 1f)]
        public float sfxVolume = 0.85f;
        public bool isMuted = false;

        [Header("Runtime State")]
        public BGMTrack currentTrack = BGMTrack.None;

        private AudioSource sourceA;
        private AudioSource sourceB;
        private AudioSource activeSource;
        private AudioSource sfxSource;
        private Coroutine fadeCoroutine;
        private bool isLoadingClips = false;

        private const string PREF_KEY_VOLUME = "Burger_BGM_Volume";
        private const string PREF_KEY_MUTED = "Burger_BGM_Muted";

        private void Awake()
        {
            transform.position = new Vector3(0, 50f, 0);

            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            bgmVolume = PlayerPrefs.GetFloat(PREF_KEY_VOLUME, 0.5f);
            if (bgmVolume < 0.1f) bgmVolume = 0.5f;

            // "saya mau bgm menu langsung on saja ketika awal play"
            isMuted = false;
            PlayerPrefs.SetInt(PREF_KEY_MUTED, 0);

            InitAudioSources();
            LoadAudioClips();

            // Langsung putar BGM Menu pada awal play!
            PlayMenuBGM(false);
        }

        private void Start()
        {
            // Initial sync with OrderManager state & ensure BGM is playing
            SyncWithGameState();

            if (activeSource == null || !activeSource.isPlaying)
            {
                PlayMenuBGM(false);
            }
        }

        private void Update()
        {
            // Mute toggle shortcut: [M]
            if (Input.GetKeyDown(KeyCode.M))
            {
                ToggleMute();
            }

            // Sync BGM with active GameState
            SyncWithGameState();
        }

        public void EnsureAudioSources()
        {
            if (sourceA == null)
            {
                sourceA = gameObject.AddComponent<AudioSource>();
                ConfigureSource(sourceA);
            }

            if (sourceB == null)
            {
                sourceB = gameObject.AddComponent<AudioSource>();
                ConfigureSource(sourceB);
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
                sfxSource.loop = false;
                sfxSource.spatialBlend = 0f;
                sfxSource.bypassEffects = true;
                sfxSource.bypassListenerEffects = true;
                sfxSource.bypassReverbZones = true;
                sfxSource.ignoreListenerPause = true;
                sfxSource.volume = isMuted ? 0f : sfxVolume;
            }

            if (activeSource == null)
            {
                activeSource = sourceA;
            }
        }

        private void InitAudioSources()
        {
            EnsureAudioSources();
        }

        private void ConfigureSource(AudioSource src)
        {
            src.playOnAwake = false;
            src.loop = true;
            src.spatialBlend = 0f; // Pure 2D stereo background music
            src.bypassEffects = true;
            src.bypassListenerEffects = true;
            src.bypassReverbZones = true;
            src.ignoreListenerPause = true;
            src.volume = 0f;
        }

        private void LoadAudioClips()
        {
            // 1. Try Resources.Load
            if (clipMainMenu == null) clipMainMenu = Resources.Load<AudioClip>("Audio/mainmenu");
            if (clipMasak == null) clipMasak = Resources.Load<AudioClip>("Audio/masak");
            if (clipDimasak == null) clipDimasak = Resources.Load<AudioClip>("Audio/dimasak");
            if (clipMatang == null) clipMatang = Resources.Load<AudioClip>("Audio/matang");
            if (clipOrderanBaru == null) clipOrderanBaru = Resources.Load<AudioClip>("Audio/orderan_baru") ?? Resources.Load<AudioClip>("Audio/orderan baru");
            if (clipOrderServed == null) clipOrderServed = Resources.Load<AudioClip>("Audio/pesanan_benar_selesai") ?? Resources.Load<AudioClip>("Audio/pesanan benar_selesai");
            if (clipPotong == null) clipPotong = Resources.Load<AudioClip>("Audio/potong") ?? Resources.Load<AudioClip>("Audio/soundreality-folding-knife-424580");
            if (clipButtonClick == null) clipButtonClick = Resources.Load<AudioClip>("Audio/button_click") ?? Resources.Load<AudioClip>("Audio/vadim_makes_sound-soft-app-button-tap-sound-5-547873");

            // 2. If any clip not loaded via Resources, fallback to direct disk loading
            bool needLoad = clipMainMenu == null || clipMasak == null || clipDimasak == null || clipMatang == null || clipOrderanBaru == null || clipOrderServed == null || clipPotong == null || clipButtonClick == null;
            if (needLoad && !isLoadingClips)
            {
                StartCoroutine(LoadClipsFromDiskRoutine());
            }
        }

        private IEnumerator LoadClipsFromDiskRoutine()
        {
            isLoadingClips = true;

            string resourcesAudioDir = Path.Combine(Application.dataPath, "Resources/Audio");
            string downloadDir = @"C:\Users\HYPE7\Downloads";

            // Load mainmenu.mp3
            if (clipMainMenu == null)
            {
                string path1 = Path.Combine(resourcesAudioDir, "mainmenu.mp3");
                string path2 = Path.Combine(downloadDir, "mainmenu.mp3");
                string targetPath = File.Exists(path1) ? path1 : (File.Exists(path2) ? path2 : null);

                if (!string.IsNullOrEmpty(targetPath))
                {
                    yield return LoadClipFromFile(targetPath, clip =>
                    {
                        clipMainMenu = clip;
                        Debug.Log($"<color=#2ECC71><b>[AudioManager]</b> Berhasil memuat BGM Menu: {targetPath}</color>");
                        if (currentTrack == BGMTrack.Menu && (activeSource == null || !activeSource.isPlaying))
                        {
                            PlayTrack(BGMTrack.Menu, false);
                        }
                    });
                }
            }

            // Load masak.mp3
            if (clipMasak == null)
            {
                string path1 = Path.Combine(resourcesAudioDir, "masak.mp3");
                string path2 = Path.Combine(downloadDir, "masak.mp3");
                string targetPath = File.Exists(path1) ? path1 : (File.Exists(path2) ? path2 : null);

                if (!string.IsNullOrEmpty(targetPath))
                {
                    yield return LoadClipFromFile(targetPath, clip =>
                    {
                        clipMasak = clip;
                        Debug.Log($"<color=#2ECC71><b>[AudioManager]</b> Berhasil memuat BGM Masak: {targetPath}</color>");
                        if (currentTrack == BGMTrack.Masak && (activeSource == null || !activeSource.isPlaying))
                        {
                            PlayTrack(BGMTrack.Masak, false);
                        }
                    });
                }
            }

            // Load dimasak.mp3 (suara patty dimasak)
            if (clipDimasak == null)
            {
                string path1 = Path.Combine(resourcesAudioDir, "dimasak.mp3");
                string path2 = Path.Combine(downloadDir, "dimasak.mp3");
                string targetPath = File.Exists(path1) ? path1 : (File.Exists(path2) ? path2 : null);

                if (!string.IsNullOrEmpty(targetPath))
                {
                    yield return LoadClipFromFile(targetPath, clip =>
                    {
                        clipDimasak = clip;
                        Debug.Log($"<color=#2ECC71><b>[AudioManager]</b> Berhasil memuat SFX Dimasak: {targetPath}</color>");
                    });
                }
            }

            // Load matang.mp3 (suara patty matang)
            if (clipMatang == null)
            {
                string path1 = Path.Combine(resourcesAudioDir, "matang.mp3");
                string path2 = Path.Combine(downloadDir, "matang.mp3");
                string targetPath = File.Exists(path1) ? path1 : (File.Exists(path2) ? path2 : null);

                if (!string.IsNullOrEmpty(targetPath))
                {
                    yield return LoadClipFromFile(targetPath, clip =>
                    {
                        clipMatang = clip;
                        Debug.Log($"<color=#2ECC71><b>[AudioManager]</b> Berhasil memuat SFX Matang: {targetPath}</color>");
                    });
                }
            }

            // Load orderan baru.mp3 (suara orderan baru masuk)
            if (clipOrderanBaru == null)
            {
                string path1 = Path.Combine(resourcesAudioDir, "orderan_baru.mp3");
                string path2 = Path.Combine(resourcesAudioDir, "orderan baru.mp3");
                string path3 = Path.Combine(downloadDir, "orderan baru.mp3");
                string targetPath = File.Exists(path1) ? path1 : (File.Exists(path2) ? path2 : (File.Exists(path3) ? path3 : null));

                if (!string.IsNullOrEmpty(targetPath))
                {
                    yield return LoadClipFromFile(targetPath, clip =>
                    {
                        clipOrderanBaru = clip;
                        Debug.Log($"<color=#2ECC71><b>[AudioManager]</b> Berhasil memuat SFX Orderan Baru: {targetPath}</color>");
                    });
                }
            }

            // Load pesanan benar_selesai.mp3 (suara ketika serve pesanan benar)
            if (clipOrderServed == null)
            {
                string path1 = Path.Combine(resourcesAudioDir, "pesanan_benar_selesai.mp3");
                string path2 = Path.Combine(resourcesAudioDir, "pesanan benar_selesai.mp3");
                string path3 = Path.Combine(downloadDir, "pesanan benar_selesai.mp3");
                string targetPath = File.Exists(path1) ? path1 : (File.Exists(path2) ? path2 : (File.Exists(path3) ? path3 : null));

                if (!string.IsNullOrEmpty(targetPath))
                {
                    yield return LoadClipFromFile(targetPath, clip =>
                    {
                        clipOrderServed = clip;
                        Debug.Log($"<color=#2ECC71><b>[AudioManager]</b> Berhasil memuat SFX Pesanan Selesai (Serve): {targetPath}</color>");
                    });
                }
            }

            // Load potong.mp3 / soundreality-folding-knife-424580.mp3 (suara pisau memotong)
            if (clipPotong == null)
            {
                string path1 = Path.Combine(resourcesAudioDir, "potong.mp3");
                string path2 = Path.Combine(resourcesAudioDir, "soundreality-folding-knife-424580.mp3");
                string path3 = Path.Combine(downloadDir, "soundreality-folding-knife-424580.mp3");
                string targetPath = File.Exists(path1) ? path1 : (File.Exists(path2) ? path2 : (File.Exists(path3) ? path3 : null));

                if (!string.IsNullOrEmpty(targetPath))
                {
                    yield return LoadClipFromFile(targetPath, clip =>
                    {
                        clipPotong = clip;
                        Debug.Log($"<color=#2ECC71><b>[AudioManager]</b> Berhasil memuat SFX Potong (Chop): {targetPath}</color>");
                    });
                }
            }

            // Load button_click.mp3 / vadim_makes_sound-soft-app-button-tap-sound-5-547873.mp3
            if (clipButtonClick == null)
            {
                string path1 = Path.Combine(resourcesAudioDir, "button_click.mp3");
                string path2 = Path.Combine(resourcesAudioDir, "vadim_makes_sound-soft-app-button-tap-sound-5-547873.mp3");
                string path3 = Path.Combine(downloadDir, "vadim_makes_sound-soft-app-button-tap-sound-5-547873.mp3");
                string targetPath = File.Exists(path1) ? path1 : (File.Exists(path2) ? path2 : (File.Exists(path3) ? path3 : null));

                if (!string.IsNullOrEmpty(targetPath))
                {
                    yield return LoadClipFromFile(targetPath, clip =>
                    {
                        clipButtonClick = clip;
                        Debug.Log($"<color=#2ECC71><b>[AudioManager]</b> Berhasil memuat SFX Button Click: {targetPath}</color>");
                    });
                }
            }

            isLoadingClips = false;
        }

        private IEnumerator LoadClipFromFile(string fullPath, System.Action<AudioClip> onDone)
        {
            string url = "file:///" + fullPath.Replace("\\", "/");
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                    if (clip != null)
                    {
                        clip.name = Path.GetFileNameWithoutExtension(fullPath);
                        onDone?.Invoke(clip);
                    }
                }
                else
                {
                    Debug.LogWarning($"[AudioManager] Gagal memuat audio dari {url}: {uwr.error}");
                }
            }
        }

        public void SyncWithGameState()
        {
            OrderManager om = OrderManager.Instance;
            if (om == null) return;

            BGMTrack desiredTrack = BGMTrack.None;

            switch (om.currentState)
            {
                case GameState.MainMenu:
                case GameState.LevelSelect:
                case GameState.RecipeBook:
                    desiredTrack = BGMTrack.Menu;
                    break;

                case GameState.Playing:
                case GameState.Paused:
                case GameState.LevelComplete:
                case GameState.GameOver:
                    desiredTrack = BGMTrack.Masak;
                    break;
            }

            bool isCurrentlyPlaying = (activeSource != null && activeSource.isPlaying && currentTrack == desiredTrack && activeSource.clip != null);
            if (desiredTrack != BGMTrack.None && !isCurrentlyPlaying)
            {
                PlayTrack(desiredTrack, true, 0.7f);
            }
        }

        public void PlayMenuBGM(bool crossfade = true)
        {
            PlayTrack(BGMTrack.Menu, crossfade);
        }

        public void PlayCookingBGM(bool crossfade = true)
        {
            PlayTrack(BGMTrack.Masak, crossfade);
        }

        public void PlayTrack(BGMTrack track, bool crossfade = true, float duration = 0.6f)
        {
            EnsureAudioSources();

            AudioClip targetClip = null;
            if (track == BGMTrack.Menu) targetClip = clipMainMenu;
            else if (track == BGMTrack.Masak) targetClip = clipMasak;

            if (targetClip == null)
            {
                // Clip not loaded yet; try loading, but do NOT lock currentTrack yet
                LoadAudioClips();
                if (track == BGMTrack.Menu) targetClip = clipMainMenu;
                else if (track == BGMTrack.Masak) targetClip = clipMasak;

                if (targetClip == null)
                {
                    currentTrack = track;
                    return;
                }
            }

            currentTrack = track;

            if (activeSource != null && activeSource.clip == targetClip && activeSource.isPlaying)
            {
                // Already playing this clip
                return;
            }

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(CrossfadeRoutine(targetClip, crossfade ? duration : 0.05f));
        }

        private IEnumerator CrossfadeRoutine(AudioClip newClip, float duration)
        {
            EnsureAudioSources();

            AudioSource outgoing = activeSource;
            AudioSource incoming = (outgoing == sourceA) ? sourceB : sourceA;
            if (incoming == null) incoming = (sourceA != null) ? sourceA : sourceB;
            activeSource = incoming;

            if (incoming == null)
            {
                fadeCoroutine = null;
                yield break;
            }

            incoming.clip = newClip;
            incoming.time = 0f;
            incoming.volume = 0f;

            if (newClip != null)
            {
                incoming.Play();
            }

            float startOutVol = (outgoing != null && outgoing != incoming) ? outgoing.volume : 0f;
            float targetInVol = isMuted ? 0f : bgmVolume;

            if (duration <= 0.05f)
            {
                incoming.volume = targetInVol;
                if (outgoing != null && outgoing != incoming)
                {
                    outgoing.volume = 0f;
                    outgoing.Stop();
                    outgoing.clip = null;
                }
                fadeCoroutine = null;
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime; // Use unscaled time so pause menu doesn't freeze fade
                float t = Mathf.Clamp01(elapsed / duration);

                if (outgoing != null && outgoing != incoming)
                {
                    outgoing.volume = Mathf.Lerp(startOutVol, 0f, t);
                }

                if (incoming != null)
                {
                    incoming.volume = Mathf.Lerp(0f, targetInVol, t);
                }

                yield return null;
            }

            if (outgoing != null && outgoing != incoming)
            {
                outgoing.volume = 0f;
                outgoing.Stop();
                outgoing.clip = null;
            }

            if (incoming != null)
            {
                incoming.volume = targetInVol;
            }

            fadeCoroutine = null;
        }

        public void StopBGM(bool fade = true)
        {
            currentTrack = BGMTrack.None;
            if (activeSource != null)
            {
                if (fade)
                {
                    if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                    fadeCoroutine = StartCoroutine(CrossfadeRoutine(null, 0.5f));
                }
                else
                {
                    sourceA.Stop();
                    sourceB.Stop();
                    sourceA.volume = 0f;
                    sourceB.volume = 0f;
                }
            }
        }

        public void SetVolume(float newVolume)
        {
            bgmVolume = Mathf.Clamp01(newVolume);
            PlayerPrefs.SetFloat(PREF_KEY_VOLUME, bgmVolume);

            // Ketika 0 maka otomatis terdeteksi BGM mati, dan ketika > 0 otomatis aktif
            if (bgmVolume <= 0.001f)
            {
                if (!isMuted)
                {
                    isMuted = true;
                    PlayerPrefs.SetInt(PREF_KEY_MUTED, 1);
                }
            }
            else
            {
                if (isMuted)
                {
                    isMuted = false;
                    PlayerPrefs.SetInt(PREF_KEY_MUTED, 0);
                }
            }
            PlayerPrefs.Save();

            if (activeSource != null)
            {
                activeSource.volume = isMuted ? 0f : bgmVolume;
            }
        }

        public void ToggleMute()
        {
            isMuted = !isMuted;
            if (!isMuted && bgmVolume <= 0.05f)
            {
                bgmVolume = 0.5f;
                PlayerPrefs.SetFloat(PREF_KEY_VOLUME, bgmVolume);
            }
            PlayerPrefs.SetInt(PREF_KEY_MUTED, isMuted ? 1 : 0);
            PlayerPrefs.Save();

            if (activeSource != null)
            {
                activeSource.volume = isMuted ? 0f : bgmVolume;
            }

            if (sfxSource != null)
            {
                sfxSource.volume = isMuted ? 0f : sfxVolume;
            }

            string status = isMuted ? "DIMATIKAN (Muted)" : "DIAKTIFKAN";
            if (OrderManager.Instance != null)
            {
                OrderManager.Instance.AddToast(isMuted ? "Audio Dimatikan" : "Audio Diaktifkan", Color.white);
            }
        }

        public void PlaySFX(AudioClip clip, float volumeScale = 1.0f)
        {
            if (isMuted || clip == null) return;
            if (sfxSource == null) InitAudioSources();
            if (sfxSource != null)
            {
                sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale * sfxVolume));
            }
        }

        public void PlayPattyCookedSFX()
        {
            if (clipMatang == null) LoadAudioClips();
            if (clipMatang != null)
            {
                PlaySFX(clipMatang, 0.95f);
            }
        }

        public void PlayNewOrderSFX()
        {
            if (clipOrderanBaru == null) LoadAudioClips();
            if (clipOrderanBaru != null)
            {
                PlaySFX(clipOrderanBaru, 0.95f);
            }
        }

        public void PlayOrderServedSFX()
        {
            if (clipOrderServed == null) LoadAudioClips();
            if (clipOrderServed != null)
            {
                PlaySFX(clipOrderServed, 1.0f);
            }
        }

        public void PlayChopSFX()
        {
            if (clipPotong == null) LoadAudioClips();
            if (clipPotong != null)
            {
                PlaySFX(clipPotong, 0.95f);
            }
        }

        public void PlayButtonClickSFX()
        {
            if (clipButtonClick == null) LoadAudioClips();
            if (clipButtonClick != null)
            {
                PlaySFX(clipButtonClick, 1.0f);
            }
        }
    }
}
