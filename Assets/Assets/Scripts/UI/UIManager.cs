using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    private UIInputActions actions;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject photoMenu;
    [SerializeField] private GameObject HUD;
    [SerializeField] SetCameraSpeed setCameraSpeedScript;
    [SerializeField] private UISettings UISettingsScript;
    [SerializeField] private GameObject[] photoPanel;
    private int panelUIIndex;
    public int panelAssignmentIndex;
    public int imageAssignmentIndex;
    [SerializeField] private GameObject photoMenuSelectedImage;
    [SerializeField] private GameObject dialogueBoxFlag;

    void OnEnable()
    {
        actions = new UIInputActions();
        actions.UI.Enable();
        actions.UI.Menu.performed += Menu;

        LockAndHideCursor();
    }

    void OnDisable()
    {
        actions.UI.Menu.performed -= Menu;
        actions.UI.Disable();
    }

    public void Menu(InputAction.CallbackContext context)
    {
        if (context.performed && !dialogueBoxFlag.activeInHierarchy)
        {
            if (photoMenuSelectedImage.activeInHierarchy)
            {
                HideImage();
                return;
            }
            else if (photoMenu.activeInHierarchy)
            {
                ClosePhotoMenu();
                OpenMainMenu();
                return;
            }

            if (settingsMenu.activeInHierarchy)
            {
                UISettingsScript.SaveSettings();
                CloseSettingsMenu();
                OpenMainMenu();
                return;
            }

            if (mainMenu.activeInHierarchy)
            {
                CloseMainMenu();
            }
            else
            {
                OpenMainMenu();
            }
        }
    }

    public void LockAndHideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void UnlockAndUnhideCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowHUD()
    {
        HUD.SetActive(true);
    }
    public void HideHUD()
    {
        HUD.SetActive(false);
    }

    public void OpenMainMenu()
    {
        UnlockAndUnhideCursor();
        Time.timeScale = 0;
        HideHUD();
        mainMenu.SetActive(true);
        setCameraSpeedScript.Pause();
    }
    public void CloseMainMenu()
    {
        LockAndHideCursor();
        Time.timeScale = 1;
        ShowHUD();
        mainMenu.SetActive(false);
        setCameraSpeedScript.SetDefaultSpeed();
    }

    public void HideMainMenu()
    {
        mainMenu.SetActive(false);
    }

    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true);
        HideMainMenu();
    }
    public void CloseSettingsMenu()
    {
        settingsMenu.SetActive(false);
        OpenMainMenu();
    }
    public void OpenPhotoMenu()
    {
        photoMenu.SetActive(true);
        HideMainMenu();
    }
    public void ClosePhotoMenu()
    {
        photoMenu.SetActive(false);
        OpenMainMenu();
    }

    public void NextPhotoPanel()
    {
        if (panelUIIndex < photoPanel.Length - 1)
        {
            photoPanel[panelUIIndex].SetActive(false);
            panelUIIndex++;
            photoPanel[panelUIIndex].SetActive(true);
        }
    }
    public void PreviousPhotoPanel()
    {
        if (panelUIIndex > 0)
        {
            photoPanel[panelUIIndex].SetActive(false);
            panelUIIndex--;
            photoPanel[panelUIIndex].SetActive(true);
        }
    }

    public void ShowImage()
    {
        photoMenuSelectedImage.SetActive(true);
        photoMenuSelectedImage.GetComponent<RawImage>().texture = EventSystem.current.currentSelectedGameObject.GetComponent<RawImage>().texture;
    }

    public void HideImage()
    {
        photoMenuSelectedImage.SetActive(false);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
