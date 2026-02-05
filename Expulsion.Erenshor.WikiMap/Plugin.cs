using System.Collections;
using System.IO;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Expulsion.Erenshor.WikiMap
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "Expulsion.Erenshor.WikiMap";
        private const string PluginName = "WikiMap";
        private const string PluginVersion = "1.0.3";

        private Harmony? _harmonyInstance;

        private GameObject? _mapObject;
        private GameObject? _mapBorderObject;

        private void Awake()
        {
            _harmonyInstance = new Harmony(PluginGuid);
            _harmonyInstance.PatchAll();

            Logger.LogInfo($"Plugin {PluginName} is loaded!");
        }

        private void Start()
        {
            StopAllCoroutines();

            SceneManager.sceneLoaded += (scene, mode) => { StartCoroutine(WaitForMapObject()); };
        }

        private IEnumerator WaitForMapObject()
        {
            float startTime = Time.time;
            while (_mapObject == null && Time.time - startTime < 5f)
            {
                _mapObject = GameObject.Find("UI/UIElements/Map");
                _mapBorderObject = GameObject.Find("UI/UIElements/Map/Map (1)");
                yield return null;
            }

            if (_mapObject == null)
            {
                yield break;
            }

            var mapRect = _mapObject?.GetComponent<RectTransform>();
            var mapBorderRect = _mapBorderObject?.GetComponent<RectTransform>();

            var mapImage = _mapObject?.GetComponent<Image>();

            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.LoadImage(File.ReadAllBytes(Path.Combine(Paths.PluginPath, PluginGuid, "Assets", "ErenshorMapRoutes.png")));

            if (mapImage != null && mapRect != null && mapBorderRect != null)
            {
                mapRect.sizeDelta = new Vector2(texture.width, texture.height) * 0.55f;
                mapRect.anchoredPosition = new Vector2(0, 0);
                mapRect.anchoredPosition3D = new Vector3(0, 0, 0);
                mapBorderRect.anchorMin = new Vector2(0, 0);
                mapBorderRect.anchorMax = new Vector2(1, 1);
                mapBorderRect.offsetMin = new Vector2(-24, -24);
                mapBorderRect.offsetMax = new Vector2(24, 24);
                mapImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }

        private void OnDestroy()
        {
            _harmonyInstance?.UnpatchSelf();
        }
    }
}