using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AdhawkApi
{

    public class ScreenDPIAdjustment : MonoBehaviour
    {

        public const string PLAYER_PREF_USER_DPI = "user_dpi";

        public RectTransform box;
        public InputField dpiText;
        public InputField boxSizeText;

        public GameObject window;

        float dpi = 0;
        float boxWidth_mm = 0;

        float boxWidth_pixels = 0;

        public static ScreenDPIAdjustment Instance { get; private set; } = null;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }

        public Coroutine ShowAndWaitForClose()
        {
            return StartCoroutine(ShowAndWaitForCloseCoroutine());
        }

        private IEnumerator ShowAndWaitForCloseCoroutine()
        {
            window.SetActive(true);
            yield return new WaitUntil(() => !window.activeSelf);
        }

        public void dpi_input_changed(string screen_dpi)
        {
            float.TryParse(screen_dpi, out dpi);
            boxWidth_mm = boxWidth_pixels / (dpi / 25.4f);
            boxSizeText.SetTextWithoutNotify(boxWidth_mm.ToString());

        }

        public void box_width_input_changed(string box_width_mm)
        {
            float.TryParse(box_width_mm, out boxWidth_mm);
            dpi = (boxWidth_pixels / boxWidth_mm) * 25.4f;
            dpiText.SetTextWithoutNotify(dpi.ToString());
        }

        public void submit_pressed()
        {
            EyeTrackerAPI.Instance.ScreenDPI = dpi;
            PlayerPrefs.SetFloat(PLAYER_PREF_USER_DPI, dpi);
            window.SetActive(false);
        }

        public void cancel_pressed()
        {
            window.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            resetWindow();
        }

        void resetWindow()
        {
            float stored_dpi = PlayerPrefs.GetFloat(PLAYER_PREF_USER_DPI, 0);

            dpi = stored_dpi == 0 ? Screen.dpi : stored_dpi;

            Vector3[] corners = new Vector3[4];
            box.GetWorldCorners(corners); // bl,tl,tr,br

            boxWidth_pixels = corners[3].x - corners[0].x;
            boxWidth_mm = boxWidth_pixels / (dpi / 25.4f);
            dpiText.SetTextWithoutNotify(dpi.ToString());

            boxSizeText.SetTextWithoutNotify(boxWidth_mm.ToString());
        }

        private void OnEnable()
        {
            resetWindow();
        }

    }

}