using System.Collections;
using System.IO;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace Expulsion.Erenshor.WikiMap
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string PluginGuid = "Expulsion.Erenshor.WikiMap";
        private const string PluginName = "WikiMap";
        private const string PluginVersion = "1.0.0";

        private Harmony? _harmonyInstance;

        private GameObject? _mapObject;

        private void Awake()
        {
            _harmonyInstance = new Harmony(PluginGuid);
            _harmonyInstance.PatchAll();

            StartCoroutine(WaitForMapObject());

            Logger.LogInfo($"Plugin {PluginName} is loaded!");
        }

        private IEnumerator WaitForMapObject()
        {
            while (!_mapObject)
            {
                _mapObject = GameObject.Find("Map");
                yield return null;
            }

            var mapImage = _mapObject?.GetComponent<Image>();

            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.LoadImage(File.ReadAllBytes($"{Paths.PluginPath}/{PluginGuid}/Assets/ErenshorMapRoutes.png"));

            if (mapImage)
                mapImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        private void OnDestroy()
        {
            _harmonyInstance?.UnpatchSelf();
        }
    }
}