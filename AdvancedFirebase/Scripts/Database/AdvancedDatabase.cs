using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using UnityEngine;

public class AdvancedDatabase : MonoBehaviour
{
    #region Singleton

    private static AdvancedDatabase instance;

    public static AdvancedDatabase Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("Advanced Database").AddComponent<AdvancedDatabase>();
            }
            
            return instance;
        }
    }

    #endregion

    #region Firebase

    private DatabaseReference _database;
    private FirebaseAuth _auth;

    #endregion
    
    #region Enable

    private void OnEnable()
    {
        instance = this;
        _database = FirebaseDatabase.DefaultInstance.RootReference;
        _auth = FirebaseAuth.DefaultInstance;
    }

    #endregion

    #region Add Data

    /// <summary>
    /// Updates the value of the given property at the specified path in Firebase Realtime Database by adding the given value to the current value.
    /// </summary>
    /// <param name="_path">The path of the data in Firebase Realtime Database.</param>
    /// <param name="_valueName">The name of the property to update.</param>
    /// <param name="addValue">The value to add to the current value of the property.</param>

    public void AddToAndSave(string _path,string _valueName, int addValue)
    {
        _database.Child(_path).Child(_valueName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Failed!");
            }

            DataSnapshot snapshot = task.Result;

            int value = int.Parse(snapshot.Value.ToString());
            value += addValue;

            _database.Child(_path).Child(_valueName).SetValueAsync(value);
        });
    }

    /// <summary>
    /// Updates the value of the given property at the specified path in Firebase Realtime Database by subtracting the given value from the current value.
    /// </summary>
    /// <param name="_path">The path of the data in Firebase Realtime Database.</param>
    /// <param name="_valueName">The name of the property to update.</param>
    /// <param name="subtractValue">The value to subtract from the current value of the property.</param>

    public void SubtractFromValueAtPath(string _path,string _valueName, int subtractValue)
    {
        _database.Child(_path).Child(_valueName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Failed!");
            }

            DataSnapshot snapshot = task.Result;

            int value = int.Parse(snapshot.Value.ToString());
            value -= subtractValue;

            _database.Child(_path).Child(_valueName).SetValueAsync(value);
        });
    }

    #endregion

    #region Save

    public void UpdateData(string _path,string _valueName, object value)
    {
        _database.Child(_path).Child(_valueName).SetValueAsync(value);
    }

    public void SaveCustomClassToDB(string _path,string customId, bool useRandomKey, object value)
    {
        var strValue = JsonConvert.SerializeObject(value);

        if (useRandomKey)
        {
            var key = _database.Child(_path).Push().Key;
            _database.Child(_path).Child(key).SetValueAsync(strValue);
        }
        else
        {
            _database.Child(_path).Child(customId).SetValueAsync(strValue);
        }
        
    }

    #endregion

    #region Retrieve

    public string GetCustomDataWithJson(string path)
    {
        string jsonValue = "";
       var task = _database.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                //Error
            }

            DataSnapshot snapshot = task.Result;
            jsonValue = snapshot.GetRawJsonValue();
            return jsonValue;
        });
       task.Wait();
       return jsonValue;

      
    }

    #endregion

    #region Authentication

    public bool LoginAndRegister(string email, string password, bool register)
    {
        if (register)
        {
            _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                    return false;
                }
                if (task.IsFaulted) {
                    Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return false;
                }

                return true;
            });
        }
        else
        {
            _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return false;
                }
                if (task.IsFaulted) {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return false;
                }

                return true;
            });
        }

        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public void LoginWithGuest()
    {
        _auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }

    public void DeleteAccount()
    {
        Firebase.Auth.FirebaseUser user = _auth.CurrentUser;
        if (user != null) {
            user.DeleteAsync().ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User deleted successfully.");
            });
        }
    }

    public bool SendPasswordResetMail(string email)
    {
        _auth.SendPasswordResetEmailAsync(email).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    return false;
                }
                if (task.IsFaulted) {
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    return false;
                }

                Debug.Log("Password reset email sent successfully.");
                return true;
            });
        return false;
    }

    public void SignOut()
    {
        _auth.SignOut();
    }
    #endregion

    #region LeaderBoard
    /// <summary>
    /// Gets the specified number of data with the largest 'coin' value from the Firebase database.
    /// </summary>
    /// <param name="_path">The path to the Firebase database reference.</param>
    /// <param name="_valueName">The name of the value to compare.</param>
    /// <param name="limit">The maximum number of data to retrieve.</param>
    public List<DataSnapshot> GetArraysByMaxValue(string _path, string _valueName, int limit)
    {
        List<DataSnapshot> Array = new List<DataSnapshot>();
        if (limit == 0) limit = 1;
        
        _database.OrderByChild(_valueName).LimitToLast(limit).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                // Handle the error...
                Debug.LogError("Failed!");
            }
            else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot childSnapshot in snapshot.Children) {
                    Array.Add(childSnapshot);
                }
            }
        });
        
        return Array;
    }
    
    /// <summary>
    /// Gets the specified number of data with the smallest 'coin' value from the Firebase database.
    /// </summary>
    /// <param name="_path">The path to the Firebase database reference.</param>
    /// <param name="_valueName">The name of the value to compare.</param>
    /// <param name="limit">The maximum number of data to retrieve.</param>
    public List<DataSnapshot> GetArraysByMinValue(string _path, string _valueName, int limit)
    {
        List<DataSnapshot> Array = new List<DataSnapshot>();
        if (limit == 0) limit = 1;
        _database.OrderByChild(_valueName).LimitToFirst(limit).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                // Handle the error...
                Debug.LogError("Failed!");
            }
            else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot childSnapshot in snapshot.Children) {
                    Array.Add(childSnapshot);
                }
            }
        });

        return Array;
    }

    #endregion
 
}
