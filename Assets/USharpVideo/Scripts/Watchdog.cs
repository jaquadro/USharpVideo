
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonSharp.Video
{
    public class Watchdog : UdonSharpBehaviour
    {
        public GameObject faultObject;
        public UdonBehaviour actionController;
        public Text faultText;

        bool faulted = false;
        float time = 0;
        float nextTimeout = 0;

        Color prevTextColor;

        const float timeout = 5;

        private void Start()
        {
            time = Time.time;
            nextTimeout = time + timeout;

            if (faultObject)
                faultObject.SetActive(false);
        }

        public void Ping()
        {
            time = Time.time;
            nextTimeout = time + timeout;

            if (faulted)
            {
                Debug.Log("[USVWatchdog] Player resumed ping");
                faulted = false;
                if (faultObject != null)
                    faultObject.SetActive(false);
                if (faultText != null)
                {
                    faultText.text = "";
                    faultText.color = prevTextColor;
                }
            }
        }

        private void Update()
        {
            if (Time.time > nextTimeout && !faulted)
            {
                Debug.Log("[USVWatchdog] No response from player");
                faulted = true;
                if (faultObject != null)
                    faultObject.SetActive(true);
                if (faultText != null)
                {
                    prevTextColor = faultText.color;
                    faultText.text = "Video player fault: please rejoin world";
                    faultText.color = Color.red;
                }
                if (actionController != null)
                    actionController.SendCustomEvent("PlayerFault");
            }
        }
    }
}