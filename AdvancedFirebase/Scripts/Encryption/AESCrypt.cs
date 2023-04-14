using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace AdvancedFirebase.Encryption
{
    public class AESCrypt
    {
        private static string password = "AdvancedFirebase";
        

        #region User manuel

        // This method will do the encryption and return a string
        
        public static string Encrypt(string value, string _password)
        {
            password = _password;
            var cryptedStr = "";
            
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.GenerateKey();
                aes.GenerateIV();
                byte[] key = Encoding.UTF8.GetBytes(password);
                byte[] iv = aes.IV;

                byte[] originalAsBytes = Encoding.UTF8.GetBytes(value);
       
                byte[] encrypted = ReturnEncryptedValue(originalAsBytes, key, iv);

                
                cryptedStr = Convert.ToBase64String(encrypted);
                

               
             
                
            }


            return cryptedStr;
        }
        

        public static string Decrypt(string value, string _password)
        {
            password = _password;
            var decryptedStr = "";
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 128;
                aes.BlockSize = 128;
                aes.GenerateKey();
                aes.GenerateIV();
                byte[] key = Encoding.UTF8.GetBytes(password);
                byte[] iv = aes.IV;

                byte[] originalAsBytes = Encoding.UTF8.GetBytes(value);
       
       

                
              
                
                byte[] decrypted = ReturnDecryptedValue(originalAsBytes, key, iv);

               
                decryptedStr = Encoding.UTF8.GetString(decrypted);
               
             
                
            }

            return decryptedStr;

        }


        #endregion

        #region System Encryption

        static byte[] ReturnEncryptedValue(byte[] original, byte[] key, byte[] iv)
        {
            //Create aes crypt object
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(original, 0, original.Length);
                    cs.Close();

                    return ms.ToArray();
                }
            }
        }
        static byte[] ReturnDecryptedValue(byte[] encrypted, byte[] key, byte[] iv)
        {
            //Create aes crypt object
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

             
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(encrypted, 0, encrypted.Length);
                    cs.Close();

                    return ms.ToArray();
                }
            }
        }

        #endregion
    
    

    }
}

