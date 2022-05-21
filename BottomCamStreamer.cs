using UnityEngine;

namespace CBC
{
    public class BottomCamStreamer : MonoBehaviour
    {
        public static BottomCamStreamer instance { get; private set; }

        public bool renderOnScreen = false;
        public Camera cameraToStream;

        RenderTexture textureFromCamera;
        MeshRenderer screenToRenderOn;

        void Start() 
        {
            if (instance != null)
            {
                Destroy(this);
                return;
            }
            instance = this;

            screenToRenderOn = GetComponent<MeshRenderer>();
            textureFromCamera = new RenderTexture(512, 512, 16);
            textureFromCamera.Create();

            GlobalMessenger.AddListener("ExitFlightConsole", new Callback(TurnOffScreen));
            GlobalMessenger.AddListener("ExitLandingView", new Callback(TurnOffScreen));
            GlobalMessenger.AddListener("EnterLandingView", new Callback(TurnOnScreen));
        }
        void TurnOffScreen()
        {
            cameraToStream.targetTexture = null;
            screenToRenderOn.material.mainTexture = null;
            screenToRenderOn.material.color = Color.black;
        }
        void TurnOnScreen()
        {
            screenToRenderOn.material.color = Color.white;
        }
        public static bool IsShipCameraOn()
        {
            return instance.renderOnScreen;
        }

        public static void SetTrueRenderOnScreen()
        {
            instance.renderOnScreen = true;
        }
        void Update()
        {
            ShowNextFrame();
        }
        void ShowNextFrame() 
        {
            if (cameraToStream.enabled == false && renderOnScreen)
            {
                textureFromCamera.Release();
                textureFromCamera.width = 512;
                textureFromCamera.height = 512;
                cameraToStream.targetTexture = textureFromCamera;
                cameraToStream.Render();

                screenToRenderOn.material.mainTexture = textureFromCamera;
            }
        }
    }
}
