using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

using Color = UnityEngine.Color;

namespace Game.Interaction
{
    public class GameLightControl : MonoBehaviour, IInteractiveLight, IGameLoadProgress
    {
        [SerializeField] private HDAdditionalLightData beleuchtung;
        [SerializeField] private bool brokenLight = false;
        [SerializeField] private bool canBeRepaired = true;

        private Color light_color;
        private float light_intense;
        private float light_range;
        private Coroutine flickerRoutine;
        private bool savedSettingsDefault;

        public bool IsEnabled => beleuchtung.GetComponent<Light>().enabled;

        private void Start()
        {
            ReadDefaultSettings();
        }

        private void ReadDefaultSettings()
        {
            if (savedSettingsDefault)
            {
                return;
            }

            this.savedSettingsDefault = true;
            Color color = beleuchtung.color;
            float intensity = beleuchtung.GetComponent<Light>().intensity;
            float range = beleuchtung.range;

            SaveSettings(color, intensity, range);
        }

        [ContextMenu("Default Light")]
        public void SwitchDefaultLight()
        {
            SetSettings(this.light_color, this.light_intense, this.light_range);
        }

        public void Flicker()
        {
            if (!IsEnabled)
            {
                return;
            }

            // Vorbereitung:
            DisableDanger();
            ReadDefaultSettings();

            // Wirkung:
            const int anzahlFlickerProSek = 4;
            List<float> flickerIntensities = CreateRandomLightIntensities(anzahlFlickerProSek);

            // Starten:
            flickerRoutine = StartCoroutine(FlickerEffect());

            // Ausführen:
            IEnumerator FlickerEffect()
            {
                while (true)
                {
                    // alle Lichtstärken anwenden, aber
                    // mit gleichm. Abständen, damit man es sehen kann
                    for (int i = 0; i < anzahlFlickerProSek; i++)
                    {
                        float intensity = flickerIntensities[i];
                        Color colorFromLightReuse = beleuchtung.color;
                        SetSettings(colorFromLightReuse, intensity, beleuchtung.range);
                        yield return new WaitForSecondsRealtime(1f / anzahlFlickerProSek);
                    }

                    yield return new WaitForEndOfFrame();

                    if (flickerRoutine == null)
                    {
                        yield break;
                    }
                }
            }
        }

        private List<float> CreateRandomLightIntensities(int anzahlFlickerProSek)
        {
            List<float> flickerIntensities = new List<float>();
            for (int i = 0; i < anzahlFlickerProSek; i++)
            {
                float newValue = UnityEngine.Random.Range(light_intense, light_intense * 4);
                flickerIntensities.Add(newValue);
            }

            return flickerIntensities;
        }

        public void TurnOff()
        {
            beleuchtung.GetComponent<Light>().enabled = false;
        }

        public void TurnOn()
        {
            if (brokenLight)
            {
                return;
            }

            beleuchtung.GetComponent<Light>().enabled = true;
        }

        public bool IsBroken => this.brokenLight;
        public bool CanBeRepaired => canBeRepaired;

        public void Repair()
        {
            brokenLight = false;
        }

        public void EnableDanger()
        {
            Flicker();
        }

        [ContextMenu("Red Light")]
        public void SwitchRed()
        {
            if (!Application.isPlaying)
            {
                ReadDefaultSettings();
            }

            SetSettings(new Color(1, 0, 0), light_intense * 2, light_range * 2);
        }

        private void SaveSettings(Color color, float intensity, float range)
        {
            this.light_color = color;
            this.light_intense = intensity;
            this.light_range = range;
        }

        private void SetSettings(Color color, float intensity, float range)
        {
            beleuchtung.color = color;
            beleuchtung.GetComponent<Light>().intensity = intensity;
            beleuchtung.range = range;
        }

        public void DisableDanger()
        {
            if (flickerRoutine != null)
            {
                StopCoroutine(flickerRoutine);
                flickerRoutine = null;
            }
        }
    }
}
