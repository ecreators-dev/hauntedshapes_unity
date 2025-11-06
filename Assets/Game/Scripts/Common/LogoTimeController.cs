using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Game.SceneControl
{
    [RequireComponent(typeof(PlayableDirector))]
    public sealed class LogoTimeController : MonoBehaviour
    {
        [SerializeField] private PlayableDirector timeline;
        [SerializeField] private List<string> openAfterEnd;

        private async void Awake()
        {
            if (timeline == null)
            {
                timeline = GetComponent<PlayableDirector>();
            }

            List<string> allSceneNames = new List<string>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                allSceneNames.Add(sceneName);
            }

            string logoSceneName = SceneManager.GetActiveScene().name;
            allSceneNames.Sort((a, b) => Comparer<int>.Default.Compare(SceneManager.GetSceneByName(a).buildIndex, SceneManager.GetSceneByName(b).buildIndex));
            foreach (string name in allSceneNames)
            {
                if (name != logoSceneName)
                {
                    if (SceneManager.GetSceneByName(name).isLoaded)
                    {
                        await SceneManager.UnloadSceneAsync(name);
                    }
                }
            }
        }

        private void Start()
        {
            timeline.stopped -= OnEndInternal;
            timeline.stopped += OnEndInternal;
        }

        private void OnEndInternal(PlayableDirector director)
        {
            //Task.Run(OnEnd);
        }

        public async void OnEnd()
        {
            //await SceneManager.UnloadSceneAsync(0);
            for (int i = 0; i < openAfterEnd.Count; i++)
            {
                if (!SceneManager.GetSceneByName(openAfterEnd[i]).isLoaded)
                {
                    await SceneManager.LoadSceneAsync(openAfterEnd[i], i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);

                    if (i == 0)
                        await Task.Delay(2000);
                }
            }
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(openAfterEnd[0]));

            GameController.Instance.PlayGame();
        }
    }
}
