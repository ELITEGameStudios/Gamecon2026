using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] GameObject settingsDisplay;
    [SerializeField] PageManager pageManager;
    [SerializeField] InputActionReference pauseInput;
    [SerializeField] Button saveButton;
    [SerializeField] GameObject fpsDisplay;
    [SerializeField] CrosshairManager crosshairManager;
    [Header("Video")]
    [SerializeField] TMP_Dropdown resolutionOptions;
    [SerializeField] Toggle fullscreenToggle;
    [SerializeField] Toggle FPSToggle;
    [SerializeField] Toggle crosshairToggle;

    [Header("Volume")]
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SFXSlider;

    [Header("Sensitivity")]
    [SerializeField] Slider horizSensSlider;
    [SerializeField] Slider vertSensSlider;

    [SerializeField] TMP_InputField horizSensInputField;
    [SerializeField] TMP_InputField vertSensInputField;

    bool paused = false;
    bool pausable = true;
    bool changesMade = false;

    Vector2Int requestedResolution = Vector2Int.zero;

    public PlayerSettings currentSettings = new();
    public PlayerSettings newSettings = new();
    public SaveSystem saveSystem;

    private void Awake()
    {
        pauseInput.action.performed += OnPausePressed;
        paused = false;
        saveButton.gameObject.SetActive(false);
        InitSettings();

       CultureInfo info = CultureInfo.GetCultureInfo("en-US"); //this allows decimals to be typed into unity input fields
       System.Globalization.CultureInfo.CurrentCulture = info;
       CultureInfo.CurrentUICulture = info;
    }
    public void OnGameOver(float time)
    {
        pausable = false;
    }

    void InitSettings()
    {
        if (saveSystem == null) saveSystem = new SaveSystem();
        var settingsAsString = saveSystem.Read( PlayerSettings.GetPlayerSettingsFilePath() );
        if (settingsAsString != string.Empty )
        {
            currentSettings = saveSystem.ParseFromJson<PlayerSettings>(settingsAsString);
        }
        if (currentSettings == null || settingsAsString == string.Empty)
        {
            Debug.LogWarning("Could not find settings files at location " + PlayerSettings.GetPlayerSettingsDirectory());
            saveSystem.EnsureSave(PlayerSettings.GetPlayerSettingsDirectory(), "playerSettings", currentSettings);
        }
        InitVideoSettings();
        InitAudioSettings();
        InitInputSettings();
        settingsDisplay.SetActive(false);
        ApplyNewSettings();
    }


    void InitVideoSettings()
    {
        requestedResolution = currentSettings.resolution;

        fullscreenToggle.isOn = currentSettings.fullscreen;

        FPSToggle.isOn = currentSettings.showFPS;

        fpsDisplay.SetActive(currentSettings.showFPS);

        for (int i = 0; i < resolutionOptions.options.Count; i++) 
        {
            var index = resolutionOptions.options[i];
            var resolutionText = index.text;
            int xPosition = resolutionText.IndexOf('x');
            int x = int.Parse(resolutionText.Substring(0, xPosition));
            int y = int.Parse(resolutionText.Substring(xPosition + 1));
            Vector2Int res = new (x, y);

            if (res == currentSettings.resolution)
            {
                resolutionOptions.value = i;
            }
        }

        crosshairManager.ToggleCrosshair(currentSettings.crosshairEnabled);

    }

    void InitAudioSettings()
    {
        SFXSlider.value = currentSettings.sfxVolume;
        BGMSlider.value = currentSettings.bgmVolume;

    }
    void InitInputSettings()
    {
        vertSensSlider.value = currentSettings.verticalSensitivity;
        horizSensSlider.value = currentSettings.horizontalSensitivity;

        vertSensInputField.text = currentSettings.verticalSensitivity.ToString();
        horizSensInputField.text = currentSettings.horizontalSensitivity.ToString();
    }


    void ApplyNewSettings()
    {
        ApplyVideoSettings();
        StartCoroutine(ApplyInputSettings());
        ApplyAudioSettings();
    }

    void ApplyVideoSettings()
    {
        Screen.SetResolution(requestedResolution.x, requestedResolution.y, currentSettings.fullscreen);
        fpsDisplay.SetActive(currentSettings.showFPS);
        if (crosshairManager != null) crosshairManager.ToggleCrosshair(currentSettings.crosshairEnabled);
    }

    IEnumerator ApplyInputSettings()
    {
        yield return new WaitUntil(() => Player.instance != null);
        if (!Player.instance.TryGetComponent(out PlayerMovementStateMachine machine)) Debug.LogWarning("Could not find state machine in player");

        machine.rotSensitivity.x = currentSettings.horizontalSensitivity;
        machine.rotSensitivity.y = currentSettings.verticalSensitivity;
    }

    void ApplyAudioSettings()
    {
        //need to sit up audio mixers
    }


    public void OnPausePressed(InputAction.CallbackContext ctx)
    {
        if (!pausable) return;
        paused = !paused;
        settingsDisplay.SetActive(paused);
        if (paused)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = RespawnManager.PlayerDead ? 0.0f : 1.0f;
        }
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void OnFullscreenToggled(bool isOn)
    {
        newSettings.fullscreen = isOn;
        OnChangeMade();
    }

    public void OnFPSToggled(bool isOn)
    {
        newSettings.showFPS = isOn;
        OnChangeMade();
    }

    public void OnCrosshairToggled(bool isOn)
    {
        newSettings.crosshairEnabled = isOn;
        OnChangeMade();
    }

    public void OnResolutionChanged(int index)
    {
        string resolution = resolutionOptions.options[index].text;
        int xPosition = resolution.IndexOf('x');
        int x = int.Parse(resolution.Substring(0, xPosition));
        int y = int.Parse(resolution.Substring(xPosition + 1));

        newSettings.resolution = new Vector2Int(x, y);

        OnChangeMade();
    }

    public void OnBGMVolumeChanged(float volume)
    {
        newSettings.bgmVolume = (float) Math.Round(volume, 2);
        OnChangeMade();
    }

    public void OnSFXVolumeChanged(float volume)
    {
        newSettings.sfxVolume = (float)Math.Round(volume, 2);
        OnChangeMade();
    }

    public void OnHorizontalSensitivitySliderChanged(float value)
    {
        newSettings.horizontalSensitivity = value;
        horizSensInputField.text = ((float)Math.Round(value, 2)).ToString() ;
        OnChangeMade();
    }

    public void OnVerticalSensitivitySliderChanged(float value)
    {
        newSettings.verticalSensitivity = value;
        vertSensInputField.text = ((float)Math.Round(value, 2)).ToString();
        OnChangeMade();
    }

    public void OnHorizontalSensitivityInputFieldValueChanged(string value)
    {
        float sens = float.Parse(value);
        sens = Mathf.Clamp(sens, horizSensSlider.minValue, horizSensSlider.maxValue);
        newSettings.horizontalSensitivity = sens;
        horizSensSlider.value = sens;
        horizSensInputField.text = sens.ToString();
        OnChangeMade();
    }

    public void OnVerticalSensitivityInputFieldValueChanged(string value)
    {
        float sens = float.Parse(value);
        sens = Mathf.Clamp(sens, vertSensSlider.minValue, vertSensSlider.maxValue);
        newSettings.verticalSensitivity = sens;
        vertSensSlider.value = sens;
        vertSensInputField.text = sens.ToString() ;
        OnChangeMade();
    }

    void OnChangeMade()
    {
        changesMade = !currentSettings.IsEqual(newSettings);
        saveButton.gameObject.SetActive(changesMade);
    }
    public void OnChangeSaved()
    {
        saveSystem.EnsureSave(PlayerSettings.GetPlayerSettingsDirectory(), "playerSettings", newSettings);
        currentSettings.Copy(newSettings);
        saveButton.gameObject.SetActive(false);
        changesMade = false;
        ApplyNewSettings();
    }
}
