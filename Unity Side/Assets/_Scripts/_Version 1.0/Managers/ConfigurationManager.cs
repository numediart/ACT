using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationManager : MonoBehaviour
{
    #region Singleton

    public static ConfigurationManager Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #endregion

    [Header("Action configuration controllers")] 
    [SerializeField] private ActionConfigurationController _actionConfigurationController;

    [Header("Avatar configuration controllers")]
    [SerializeField] private AvatarConfigurationController _avatarConfigurationController;

    [Header("Instances")] 
    [SerializeField] private GameObject _mainConfiguration;
    [SerializeField] private GameObject _actionConfiguration;
    [SerializeField] private GameObject _avatarConfiguration;
    [SerializeField] private JsonReader _jsonReader;
    
    #region Getters
    
    public ActionConfigurationController ActionConfigurationController
    {
        get => _actionConfigurationController;
        private set => _actionConfigurationController = value;
    }

    public AvatarConfigurationController AvatarConfigurationController
    {
        get => _avatarConfigurationController;
        private set => _avatarConfigurationController = value;
    }
    
    public JsonReader JsonReader
    {
        get => _jsonReader;
        private set => _jsonReader = value;
    }

    #endregion
    
    //intern var
    private GameObject _currentConfiguration;

    private void Start()
    {
        _currentConfiguration = null;
        
        _actionConfiguration.SetActive(false);
        _avatarConfiguration.SetActive(false);
        
        LoadNewConfig(_mainConfiguration);
    }

    #region Loaders

    public void LoadMainConfiguration()
    {
        UnloadCurrentConfig();
        
        LoadNewConfig(_mainConfiguration);
    }

    public void LoadActionConfiguration()
    {
        UnloadCurrentConfig();
        
        LoadNewConfig(_actionConfiguration);
    }

    public void LoadAvatarConfiguration()
    {
        UnloadCurrentConfig();
        
        LoadNewConfig(_avatarConfiguration);
    }

    private void LoadNewConfig(GameObject config)
    {
        _currentConfiguration = config;
        config.SetActive(true);
    }
    
    private void UnloadCurrentConfig()
    {
        if (_currentConfiguration != null)
            _currentConfiguration.SetActive(false);
    }
    
    #endregion

}
