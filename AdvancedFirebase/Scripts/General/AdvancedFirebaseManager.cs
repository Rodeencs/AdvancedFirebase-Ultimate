using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedFirebaseManager : MonoBehaviour
{
    [Header("Settings")] 
    [Tooltip("Please enter a custom password for it to be able to perform unique and secure encryption.")]
    [SerializeField] private string AES_Password;


    #region Get

    public string GetPassword()
    {
        return AES_Password;
    }

    #endregion
}
