
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonSharp.Video
{
    [AddComponentMenu("Udon Sharp/Video/Volume Controller")]
    public class VolumeController : UdonSharpBehaviour
    {
        public VolumePanelController[] volumePanels;
        public float volume;
        public bool muted;

        public AudioSource controlledAudioSource;
        public AudioSource avProAudioR;
        public AudioSource avProAudioL;

        private void Start()
        {
            foreach (var panel in volumePanels)
            {
                panel.UpdateSliderPosition(volume);
                panel.UpdateMuteButton(muted);
            }

            ApplyVolumeFromSlider(volume);
        }

        public void ApplyVolumeSlider(float sliderValue)
        {
            if (volume == sliderValue)
                return;

            volume = sliderValue;
            ApplyVolumeFromSlider(volume);
            foreach (var panel in volumePanels)
            {
                panel.UpdateSliderPosition(volume);
            }
        }

        public void ToggleMuteButton()
        {
            muted = !muted;

            ApplyVolumeFromSlider(volume);
            foreach (var panel in volumePanels)
            {
                panel.UpdateMuteButton(muted);
            }
        }

        private void ApplyVolumeFromSlider(float position)
        {
            float applyVolume = position;
            if (muted)
                applyVolume = 0;

            // https://www.dr-lex.be/info-stuff/volumecontrols.html#ideal thanks TCL for help with finding and understanding this
            // Using the 50dB dynamic range constants
            float audioVolume = Mathf.Clamp01(3.1623e-3f * Mathf.Exp(applyVolume * 5.757f) - 3.1623e-3f);

            controlledAudioSource.volume = audioVolume;
            avProAudioR.volume = audioVolume;
            avProAudioL.volume = audioVolume;
        }
    }
}
