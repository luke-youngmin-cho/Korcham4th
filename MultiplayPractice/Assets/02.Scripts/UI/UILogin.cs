using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Text.RegularExpressions;
using MP.Authentication;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using MP.Utilities;


namespace MP.UI
{
    public class UILogin : MonoBehaviour
    {
        private TMP_InputField _id;
        private TMP_InputField _pw;
        private Button _tryLogin;
        private Button _register;


        private async void Awake()
        {
            _id = transform.Find("Panel/InputField (TMP) - ID").GetComponent<TMP_InputField>();
            _pw = transform.Find("Panel/InputField (TMP) - PW").GetComponent<TMP_InputField>();
            _tryLogin = transform.Find("Panel/Button - TryLogin").GetComponent<Button>();
            _register = transform.Find("Panel/Button - Register").GetComponent<Button>();

            var dependecnyState = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependecnyState != DependencyStatus.Available)
                throw new Exception();

            _tryLogin.onClick.AddListener(() =>
            {
                string id = _id.text;
                string pw = _pw.text;
                FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(id, pw)
                                            .ContinueWithOnMainThread(async task =>
                                            {
                                                if (task.IsCanceled)
                                                {
                                                    UpdateDispatcher.instance.Enqueue(() =>
                                                    {
                                                        UIManager.instance.Get<UIWarningWindow>()
                                                                          .Show("Canceled login.");
                                                    });
                                                    
                                                    return;
                                                }

                                                if (task.IsFaulted)
                                                {
                                                    UpdateDispatcher.instance.Enqueue(() =>
                                                    {
                                                        UIManager.instance.Get<UIWarningWindow>()
                                                                          .Show("Faulted login.");
                                                    });
                                                    
                                                    return;
                                                }

                                                LoginInformation.Refresh(id);

                                                Debug.Log("Successfully logged in.");

                                                await FirebaseFirestore.DefaultInstance
                                                        .Collection("users")
                                                            .Document(LoginInformation.userKey)
                                                                .GetSnapshotAsync()
                                                                .ContinueWithOnMainThread(task =>
                                                                {
                                                                    Dictionary<string, object> documentDictionary = task.Result.ToDictionary();

                                                                    Debug.Log("Finished Get profile document");

                                                                    UpdateDispatcher.instance.Enqueue(() =>
                                                                    {
                                                                        if (documentDictionary?.TryGetValue("nickname", out object value) ?? false)
                                                                        {
                                                                            LoginInformation.nickname = (string)value;
                                                                            SceneManager.LoadScene("Lobby");
                                                                        }
                                                                        else
                                                                        {
                                                                            UIManager.instance.Get<UIProfileSettingWindow>().Show();
                                                                        }
                                                                    });
                                                                });
                                            });
            });

            _register.onClick.AddListener(() =>
            {
                string id = _id.text;
                string pw = _pw.text;

                if (IsValidID(id) == false)
                {
                    UIManager.instance.Get<UIWarningWindow>()
                                      .Show("Wrong email format.");
                    return;
                }
                
                if (IsValidPW(pw) == false)
                {
                    UIManager.instance.Get<UIWarningWindow>()
                                      .Show("Wrong password format.");
                    return;
                }

                FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(id, pw)
                                            .ContinueWithOnMainThread(task =>
                                            {
                                                if (task.IsCanceled)
                                                {
                                                    UpdateDispatcher.instance.Enqueue(() =>
                                                    {
                                                        UIManager.instance.Get<UIWarningWindow>()
                                                                          .Show("Canceled registration.");
                                                    });
                                                    
                                                    return;
                                                }

                                                if (task.IsFaulted)
                                                {
                                                    UpdateDispatcher.instance.Enqueue(() =>
                                                    {
                                                        UIManager.instance.Get<UIWarningWindow>()
                                                                          .Show($"Faulted registration. {task.Exception.Message}");
                                                    });
                                                    
                                                    return;
                                                }

                                                // todo -> 회원가입 성공에 대한 알림창 팝업
                                            });
            });
        }
        
        private bool IsValidID(string id)
        {
            return Regex.IsMatch(id,
                                 @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsValidPW(string pw)
        {
            return pw.Length >= 6;
        }
    }
}