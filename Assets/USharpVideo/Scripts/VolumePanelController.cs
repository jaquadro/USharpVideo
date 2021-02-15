
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonSharp.Video
{
    [AddComponentMenu("Udon Sharp/Video/Volume Panel Controller")]
    public class VolumePanelController : UdonSharpBehaviour
    {
        public VolumeController controller;
        public Slider slider;

        public GameObject muteIcon;
        public GameObject zeroVolumeIcon;
        public GameObject lowVolumeIcon;
        public GameObject HighVolumeIcon;

        bool _muted = false;

        public void UpdateSliderPosition(float position)
        {
            if (slider.value != position)
                slider.value = position;

            UpdateVolumeIcon();
        }

        public void UpdateMuteButton(bool muted)
        {
            if (muted != _muted)
            {
                _muted = muted;
                UpdateVolumeIcon();
            }
        }

        public void SliderValueChanged()
        {
            controller.ApplyVolumeSlider(slider.value);
        }

        public void PressMuteButton()
        {
            controller.ToggleMuteButton();
        }

        void UpdateVolumeIcon()
        {
            if (_muted)
            {
                muteIcon.SetActive(true);
                zeroVolumeIcon.SetActive(false);
                lowVolumeIcon.SetActive(false);
                HighVolumeIcon.SetActive(false);
            }
            else if (slider.value > 0.6f)
            {
                muteIcon.SetActive(false);
                zeroVolumeIcon.SetActive(false);
                lowVolumeIcon.SetActive(false);
                HighVolumeIcon.SetActive(true);
            }
            else if (slider.value > 0f)
            {
                muteIcon.SetActive(false);
                zeroVolumeIcon.SetActive(false);
                lowVolumeIcon.SetActive(true);
                HighVolumeIcon.SetActive(false);
            }
            else
            {
                muteIcon.SetActive(false);
                zeroVolumeIcon.SetActive(true);
                lowVolumeIcon.SetActive(false);
                HighVolumeIcon.SetActive(false);
            }
        }
    }
}
