using TMPro;

using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class LoadedScriptsMaster : MonoBehaviour, IGameLoadProgress
    {
        [SerializeField] private TextMeshProUGUI textControl;
        private string format;

        private void OnEnable()
        {
            IGameLoadMaster.OnInstanceEvent += () =>
            {
                IGameLoadMaster master = IGameLoadMaster.Instance;
                master.OnScriptProgressUpdateEvent += () => OnProgressUpdate(master.Progress);
                master.OnScriptsLoadedEvent += OnAllLoaded;
            };
        }

        private void Awake()
        {
            textControl = GetComponent<TextMeshProUGUI>();
            this.format = this.textControl.text;
            OnProgressUpdate(0);
        }

        private void OnProgressUpdate(float progress)
        {
            textControl.text = string.Format(format, $"{progress:0} %");
        }

        private void OnAllLoaded()
        {
            textControl.text = "";
        }
    }
}
