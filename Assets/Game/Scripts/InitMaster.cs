using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

// Script Execution Order: 1

namespace Game
{
    public class InitMaster : MonoBehaviour, IGameLoadMaster
    {
        public event System.Action OnScriptsLoadedEvent;
        public event System.Action OnScriptProgressUpdateEvent;
        public bool IsReady { get; private set; }
        public float Progress { get; private set; }

        private void Awake()
        {
            IGameLoadMaster.Instance = this;
            IGameLoadMaster.OnMasterInstanced();

            // Alles zurücksetzen bevor andere Scripts starten
            IsReady = false;
            Debug.Log("InitMaster Awake → Start der Initialisierung");
        }

        private void Start()
        {
            var allInitComponents = Resources.FindObjectsOfTypeAll(typeof(MonoBehaviour))
            .OfType<IGameLoadProgress>()
            .ToList();

            Debug.Log($"Gefunden: {allInitComponents.Count} Init-Scripte");

            StartCoroutine(TrackStartProgress(allInitComponents.Count));
        }

        private float progressOld;

        private System.Collections.IEnumerator TrackStartProgress(int total)
        {
            int initialized = 0;

            IEnumerable<IGameLoadProgress> allScripts = null;
            while (initialized < total)
            {
                // prüfen, welche Start bereits durch ist

                allScripts = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
                    .OfType<IGameLoadProgress>();
                initialized = allScripts
                    .Count(m => ((MonoBehaviour)m).enabled);

                Progress = Mathf.Round(initialized / total * 100);
                print("Test");
                if (Progress > progressOld)
                {
                    OnScriptProgressUpdateEvent?.Invoke();
                }
                progressOld = Progress;
                yield return null;
            }

            if (!IsReady)
            {
                IsReady = true;
                allScripts.OfType<IGameStartAfterLoadAll>().ToList().ForEach(script => script.OnGameStart());
                OnScriptsLoadedEvent?.Invoke();
            }
        }
    }

    internal interface IGameStartAfterLoadAll : IGameLoadProgress
    {
        void OnGameStart();
    }

    internal interface IGameLoadProgress
    {

    }

    public interface IGameLoadMaster
    {
        static event Action OnInstanceEvent;

        static void OnMasterInstanced() => OnInstanceEvent?.Invoke();

        static IGameLoadMaster Instance { get; internal protected set; }

        event Action OnScriptsLoadedEvent;

        event Action OnScriptProgressUpdateEvent;

        bool IsReady { get; }
        float Progress { get; }

    }

}
