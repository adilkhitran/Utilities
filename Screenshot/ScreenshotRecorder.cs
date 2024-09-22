#if UNITY_EDITOR

using System.IO;
using UnityEngine;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using System.Collections;
using UnityEditor;

namespace KHiTrAN.Screenshot
{
    public class ScreenshotRecorder : MonoBehaviour
    {
        public Vector2Int[] resolutions = { new Vector2Int(1242,2208), new Vector2Int(1242, 2688), new Vector2Int(2048, 2732) };
        public GameViewSizeGroupType sizeGroupType = GameViewSizeGroupType.Android;

        RecorderController m_RecorderController;

        private bool isPointerDown = false;


        private int step = 0;
        private int resIndex = 0;
        private float currentDelay = 0;

        private float originalTimeScale = 1;
        private int screenshotCount = 0;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
#if UNITY_EDITOR
            Application.targetFrameRate = 30;
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!isPointerDown)
                {
                    step = 0;
                    resIndex = 0;
                    originalTimeScale = Time.timeScale;
                }
                Time.timeScale = 0;
                isPointerDown = true;
            }


            if (isPointerDown)
            {
                if (currentDelay > 0)
                {
                    currentDelay -= 0.01f;
                    return;
                }

                if (step == 0)
                    SelectResolution();
                else
                    TakeSS();
            }
        }


        private void SelectResolution()
        {
            Vector2Int size = resolutions[resIndex];
            var sizeIndex = ResolutionChanger.FindSize(sizeGroupType, size.x, size.y);
            if (sizeIndex >= 0)
            {
                ResolutionChanger.SetSize(sizeIndex);
            }

            currentDelay = 0.05f;
            step = 1;
        }

        private void TakeSS()
        {
            SetResolution();
            m_RecorderController.PrepareRecording();
            m_RecorderController.StartRecording();

            currentDelay = 0.04f;

            step = 0;
            resIndex++;
            if (resIndex >= resolutions.Length)
            {
                Time.timeScale = originalTimeScale;
                isPointerDown = false;
            }
        }

        private void SetResolution()
        {
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            m_RecorderController = new RecorderController(controllerSettings);

            var mediaOutputFolder = Path.Combine(Application.dataPath, "..", "SampleRecordings/" + Screen.width + "_" + Screen.height);

            // Image
            var imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            imageRecorder.name = "My Image Recorder";
            imageRecorder.Enabled = true;
            imageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
            imageRecorder.CaptureAlpha = false;



            imageRecorder.OutputFile = Path.Combine(mediaOutputFolder, "image_" + screenshotCount) + DefaultWildcard.Take;

            imageRecorder.imageInputSettings = new GameViewInputSettings
            {
                OutputWidth = Screen.width,
                OutputHeight = Screen.height,
            };

            screenshotCount++;
            // Setup Recording

            controllerSettings.AddRecorderSettings(imageRecorder);
            controllerSettings.SetRecordModeToSingleFrame(0);
        }
    }
}

#endif
