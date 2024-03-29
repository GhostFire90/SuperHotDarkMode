﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Harmony;
using UnityEngine;
using SuperhotModList;
using UnityEngine.SceneManagement;


using HarmonyLib;
namespace SuperHotDarkMode
{
    [BepInDependency("SHModList", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("ghostfire042.SHDarkmode", "SuperHot Darkmode", "1.0")]
    public class MainMod : BaseUnityPlugin, IToggleableMod
    {




        static bool mod_enabled = true;
        static Texture2D MainTex = new Texture2D(2, 2);
        Color DarkModeColor;

        static Texture2D LighterMainTex = new Texture2D(2, 2);
        Color LighterDarkModeColor;

        Harmony hw = new Harmony("mainOne");

        
        void Start()
        {
          

            ColorUtility.TryParseHtmlString("#222222", out DarkModeColor);
            ColorUtility.TryParseHtmlString("#666666", out LighterDarkModeColor);
            var tarr = MainTex.GetPixels();
            var tarr2 = LighterMainTex.GetPixels();
            for (int i = 0; i < tarr.Length; i++)
            {
                tarr[i] = DarkModeColor;
                tarr2[i] = LighterDarkModeColor;
            }
            MainTex.SetPixels(tarr);
            MainTex.Apply();
            LighterMainTex.SetPixels(tarr2);
            LighterMainTex.Apply();

            SceneManager.sceneLoaded += (a, b) => levelModifier();

            Logger.LogDebug(Settings.Instance.ToString());
            
            //sHGUIappbase.interactable = true;
            

        }
        void Awake()
        {
            hw.PatchAll();
        }
        [HarmonyPatch(typeof(PejAiFactory), "SpawnForReal", MethodType.Normal)]
        private class matOverrider
        {
            static BepInEx.Logging.ManualLogSource logSource = new BepInEx.Logging.ManualLogSource("matOverriderSource");
            private static void Prefix(ref GameObject ___Prefab)
            {
                
            }
            private static void Postfix(ref GameObject ___SpawnedGameObject)
            {
                GameObject body = ___SpawnedGameObject.transform.GetChild(7).gameObject;


                /*GameObject _TorsoTemp = body.transform.GetChild(8).gameObject;

                Material m = _TorsoTemp.GetComponent<SkinnedMeshRenderer>().sharedMaterial;
                m.SetVector("_Color", new Vector4(0, .7f, 0, 1));
                _TorsoTemp.GetComponent<SkinnedMeshRenderer>().material = m;*/
                if (mod_enabled)
                {
                    foreach (Renderer r in body.GetComponentsInChildren<Renderer>(true))
                    {
                        Vector4 color = new Vector4(.3f, .5f, .4f, 1);
                        var mat = r.sharedMaterial;
                        mat.SetVector("_EmissionColor", color);
                        mat.SetVector("_Color", color);
                        mat.SetVector("_SpecColor", new Vector4(.1f, .1f, .1f));
                        mat.SetVector("_StripeFocusColor", Color.white);
                        mat.SetVector("_SuperSpecColor", new Vector4(.1f, .1f, .1f));
                        mat.SetVector("_PrimaryRimColor", new Vector4(.1f, .1f, .1f));


                        mat.SetTexture("_MainTex", LighterMainTex);
                        mat.SetFloat("_SuperSpecularPower", .5f);
                        mat.SetFloat("_SpecularPower", .5f);
                    }
                }
                
                /*foreach(RendererWrapper r in body.GetComponentsInChildren<RendererWrapper>(true))
                {
                    r.enabled = false;
                }*/
            }
        }


        void levelModifier()
        {
            //sHGUIappbase.Redraw(0, 0);


            //r.material = new Material(Shader.Find("Standard"));

            if (mod_enabled)
            {
                foreach (Renderer r in GameObject.FindObjectsOfType<Renderer>())
                {
                    Vector4 color = new Vector4(0, 0, 0, 1);
                    if (!r.material.shader.name.Contains("Crystal") && !r.gameObject.TryGetComponent<Crosshair>(out _))
                    {


                        r.material.SetTexture("_MainTex", MainTex);

                    }
                }
            }
            
        }

        public void SetEnabled(bool _enabled)
        {
            mod_enabled = _enabled;
        }

        public bool GetEnabled()
        {
            return mod_enabled;
        }
    }
}
