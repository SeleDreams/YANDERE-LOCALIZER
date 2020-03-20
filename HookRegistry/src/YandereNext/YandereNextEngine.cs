using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using System.Reflection;
using UnityEngine.SceneManagement;
namespace YandereNext
{
	class YandereNextEngine : MonoBehaviour
	{
		void Awake()
		{
			DontDestroyOnLoad(gameObject);
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
		

		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			_currentScene = scene.name;

			//	Debug.Log(scene.name + " loaded");
			switch (_currentScene)
			{
				case "SponsorScene":
					//Debug.Log(FindObjectOfType<DiscordController>().applicationId);
					//SceneManager.LoadScene("LoadingScene");
					break;
				case "SchoolScene":
					//StartCoroutine(ExtractingText());

					break;
				default:
					break;
			}
		}
		private static string _currentScene = "WelcomeScene";
	}
}
