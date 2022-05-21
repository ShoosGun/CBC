using BepInEx;
using UnityEngine;
using CAMOWA;
using HarmonyLib;

namespace CBC
{
    [BepInPlugin("locochoco.OWA.CoolerBottomCams", "Cooler Bottom Cams","1.0.0")]
    public class CoolerBottomCams : BaseUnityPlugin
    {
        void Awake() 
        {
            var harmonyInstance = new Harmony("com.locochoco.CoolerBottomCams");
            harmonyInstance.PatchAll();
            SceneLoading.OnSceneLoad += SceneLoading_OnSceneLoad;
        }

        private void SceneLoading_OnSceneLoad(int sceneId)
        {
            if (sceneId == 1)
                CreateShipCam();
        }

        void CreateShipCam()
        {
            Transform shipChair = GameObject.Find("ship_chair").transform;

            GameObject rootOfScreen = new GameObject("CameraScreen");
            rootOfScreen.transform.parent = shipChair;
            rootOfScreen.transform.localPosition = new Vector3(0.006f, 0.178f, 4.473f); 
            rootOfScreen.transform.localRotation = Quaternion.Euler(-37.185f, 0f, 0f);

            GameObject cameraScreen = GameObject.CreatePrimitive(PrimitiveType.Plane);
            cameraScreen.GetComponent<Collider>().enabled = false;

            cameraScreen.transform.rotation = rootOfScreen.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
            cameraScreen.transform.localScale = new Vector3(0.038916f, 0.01f, 0.0414f);
            cameraScreen.transform.position = rootOfScreen.transform.position; 
            cameraScreen.transform.parent = rootOfScreen.transform;

            MeshRenderer screenRenderer = cameraScreen.GetComponent<MeshRenderer>();
            screenRenderer.renderer.material.color = Color.black;

            BottomCamStreamer camStreamer = cameraScreen.AddComponent<BottomCamStreamer>();
            

            GameObject landingCam = GameObject.Find("LandingCam");
            camStreamer.cameraToStream = landingCam.camera;
        }
    }
}

