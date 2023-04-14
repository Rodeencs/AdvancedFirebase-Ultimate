using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AdvancedFirebaseEditor : Editor
{
   public static string ConfigurationWarningKey = "AdvancedFirebase_ConfigurationWarningShown";
   static AdvancedFirebaseEditor()
   {
      EditorApplication.update += OnUpdate;
   }

   private static void OnUpdate()
   {
      if (EditorPrefs.HasKey(ConfigurationWarningKey))
      {
         return;
      }
      float timeSinceStartup = Time.realtimeSinceStartup;
      if (timeSinceStartup > 5f && !EditorApplication.isPlaying)
      {
         string title = "Advanced Firebase";
         string message = "Don't forget to install the SDK for Advanced Firebase, otherwise you may encounter errors! Create a project on the Firebase website and download the necessary SDKs and if you not use Newtonsoft.json please import!";
         string ok = "Ok, i will";
         bool result = EditorUtility.DisplayDialog(title, message, ok);

         if (result)
         {
            EditorPrefs.SetBool(ConfigurationWarningKey, true);
         }

         EditorApplication.update -= OnUpdate;
      }
   }

  
}
