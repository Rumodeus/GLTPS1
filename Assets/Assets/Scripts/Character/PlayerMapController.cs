using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerMapController : MonoBehaviour
{
    [SerializeField] private PlayerInputInitialize playerInputScript;
    [SerializeField] private Camera mapCamera;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private RawImage[] mapImage;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip mapSFX;
    [SerializeField] private float duration = 0.2f;
    private bool isMapEnabled;
    private float elapsed;

    void OnEnable()
    {
        playerInputScript.actions.Player.Map.performed += Map;
    }

    void OnDisable()
    {
        playerInputScript.actions.Player.Map.performed -= Map;
    }

    private void Map(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isMapEnabled)
            {
                Time.timeScale = 0;
                
                // capture bird's eye view
                mapCamera.Render();

                RenderTexture.active = renderTexture;
                Texture2D screenshotTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
                screenshotTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                screenshotTexture.Apply();
                RenderTexture.active = null;

                mapImage[0].texture = screenshotTexture;

                audioSource.Stop();
                audioSource.PlayOneShot(mapSFX);

                isMapEnabled = true;
                elapsed = 0;
            }
            else
            {
                Time.timeScale = 1;
                isMapEnabled = false;
                elapsed = 0;
            }
        }
    }

    void Update()
    {
        float mapImageAlpha = mapImage[0].color.a;

        // lerp map UI alpha
        if (isMapEnabled)
        {
            elapsed += Time.deltaTime;

            foreach (RawImage UIComponent in mapImage)
            {
                Color color = UIComponent.color;
                color.a = Mathf.Lerp(mapImageAlpha, 1, elapsed / duration);
                UIComponent.color = color;
            }
        }
        else
        {
            elapsed += Time.deltaTime;

            foreach (RawImage UIComponent in mapImage)
            {
                Color color = UIComponent.color;
                color.a = Mathf.Lerp(mapImageAlpha, 0, elapsed / duration);
                UIComponent.color = color;
            }
        }
    }
}
