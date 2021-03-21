using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
   
    [SerializeField] GameObject _settingsPanel;
    [SerializeField] GameObject _startGamePanel;
    [SerializeField] GameObject finishGamePanel;
    [SerializeField] GameObject[] _cameras;
    [SerializeField] Slider _speedSlider;
    [SerializeField] Slider _sizeSlider;
    [SerializeField] Slider sideSpeedSlider;

    [SerializeField] Text forwardSpeedText;
    [SerializeField] Text sizeText;
    [SerializeField] Text sideSpeedText;



    private YuzukHareket _ring;


    void Awake()
    {
        UIManagerSingleton();
        _ring = FindObjectOfType<YuzukHareket>();
    }

    void UIManagerSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SettingsButtonClicked()
    {
        if (!_settingsPanel.activeSelf)

        {
            _settingsPanel.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            _settingsPanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void SetCameraAngle(int index)
    {
        foreach (var camera in _cameras)
        {
            camera.SetActive(false);
        }
        _cameras[index].SetActive(true);

    }

    public void SetMovement()
    {
        _ring.ForwardSpeed = _speedSlider.value;
        forwardSpeedText.text = _speedSlider.value.ToString();
    }

    public void SetDiamondSize()
    {
        _ring.DiamondSizeChangeAmount = _sizeSlider.value;
        sizeText.text = _sizeSlider.value.ToString();
    }

    public void SetSwipeSpeed()
    {
        _ring.SwipeAmount = sideSpeedSlider.value;
        sideSpeedText.text = sideSpeedSlider.value.ToString();

    }

    public void StartGame()
    {
        _startGamePanel.SetActive(false);
        _ring.IsStartGame = true;
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        finishGamePanel.SetActive(false);

    }

    public void OpenFinishPanel()
    {
        finishGamePanel.SetActive(true);
    }

}
