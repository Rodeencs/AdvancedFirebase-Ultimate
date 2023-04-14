using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using TMPro;
using UnityEngine;

public class ADVDemo : MonoBehaviour
{
    [Header("Authentication")] 
    
    [Header("Login")]
    [SerializeField] private TMP_InputField log_email;
    [SerializeField] private TMP_InputField log_password;
    
    [Header("Register")]
    [SerializeField] private TMP_InputField reg_email;
    [SerializeField] private TMP_InputField reg_reEmail;
    [SerializeField] private TMP_InputField reg_password;
    
    [Header("Reset")]
    [SerializeField] private TMP_InputField reset_email;
    
    [Header("Settings")]
    [SerializeField] private int emailLength, passwordLength;

    [Header("Debug")] 
    [SerializeField] private TMP_Text debug;

    [Header("Panels")] 
    [SerializeField] private GameObject RegisterPanel;
    [SerializeField] private GameObject LoginPanel;
    [SerializeField] private GameObject ActiveUser;

    private FirebaseAuth _auth;
    
    #region Unity Methods

    private void Start()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _auth.StateChanged += StateChange;
    }

    private void StateChange(object sender, EventArgs e)
    {
        if (_auth.CurrentUser == null)
        {
            debug.text = "";
            RegisterPanel.SetActive(true);
            LoginPanel.SetActive(false);
            ActiveUser.SetActive(false);
        }
        else
        {
            Debug.Log("You have successfully logged in to your account");
            debug.text = "Logged in successfully";
            RegisterPanel.SetActive(false);
            LoginPanel.SetActive(false);
            ActiveUser.SetActive(true);
        }
    }

    

    #endregion

    #region Panels

    public void OpenLoginOrRegister(bool type)
    {
        if (type)
        {
            RegisterPanel.SetActive(true);
            LoginPanel.SetActive(false);
        }
        else
        {
            LoginPanel.SetActive(true);
            RegisterPanel.SetActive(false);
        }
        
    }

    #endregion
    
    #region Auth

    public void Register()
    {
        if (CheckFields(true))
        {
            if (AdvancedDatabase.Instance.LoginAndRegister(reg_email.text, reg_password.text,true))
            {
               Debug.Log("A new account has been created");
               debug.text = "Success!";
            }
            else
            {
                Debug.LogError("error occurred while creating the account");
                debug.text = "Fail!";
            }
        }
        else
        {
            debug.text = "Fail!"; 
        }
        
    }

    public void Login()
    {
        if (CheckFields(false))
        {
            if (AdvancedDatabase.Instance.LoginAndRegister(reg_email.text, reg_password.text,false))
            {
                Debug.Log("You have successfully logged in to your account");
                debug.text = "Success!";
            }
            else
            {
                Debug.LogError("Failed to log in to this account");
                debug.text = "Fail!";
            }
        }
    }

    public void SignOut()
    {
        AdvancedDatabase.Instance.SignOut();
    }

    public void PasswordReset()
    {
        if (reset_email.text != String.Empty)
        {
            if (AdvancedDatabase.Instance.SendPasswordResetMail(reset_email.text))
            {
                //Do something...
                debug.text = "Success!";
            }
            else
            {
                //Do something...
                debug.text = "Fail!";
            }
        }
    }
    
    private bool CheckFields(bool register)
    {
        if (register)
        {
            if (reg_email.text != String.Empty && reg_email.text.Length >= emailLength&& reg_email.text == reg_reEmail.text && reg_password.text.Length >= passwordLength)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (log_email.text != String.Empty && log_password.text != String.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    #endregion 
    
}
