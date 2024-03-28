using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;


namespace MP.UI
{
    public class UILogin : MonoBehaviour
    {
        private InputField _id;
        private InputField _pw;
        private Button _tryLogin;
        private Button _register;


        private async void Awake()
        {
            _id = transform.Find("Panel/InputField (TMP) - ID").GetComponent<InputField>();
            _pw = transform.Find("Panel/InputField (TMP) - PW").GetComponent<InputField>();
            _tryLogin = transform.Find("Panel/Button - TryLogin").GetComponent<Button>();
            _register = transform.Find("Panel/Button - Register").GetComponent<Button>();

            var dependecnyState = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (dependecnyState != DependencyStatus.Available)
                throw new Exception();

            _tryLogin.onClick.AddListener(() =>
            {
                FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(_id.text, _pw.text)
                                            .ContinueWithOnMainThread(task =>
                                            {
                                                if (task.IsCanceled)
                                                {
                                                    UIManager.instance.Get<UIWarningWindow>()
                                                                      .Show("Canceled login.");
                                                    return;
                                                }

                                                if (task.IsFaulted)
                                                {
                                                    UIManager.instance.Get<UIWarningWindow>()
                                                                      .Show("Faulted login.");
                                                    return;
                                                }

                                                // todo -> 로그인 성공 후에 실행할 추가내용 (씬 전환, 리소스 로드... )
                                            });
            });

            _register.onClick.AddListener(() =>
            {
                FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(_id.text, _pw.text)
                                            .ContinueWithOnMainThread(task =>
                                            {
                                                if (task.IsCanceled)
                                                {
                                                    UIManager.instance.Get<UIWarningWindow>()
                                                                      .Show("Canceled registration.");
                                                    return;
                                                }

                                                if (task.IsFaulted)
                                                {
                                                    UIManager.instance.Get<UIWarningWindow>()
                                                                      .Show("Faulted registration.");
                                                    return;
                                                }

                                                // todo -> 회원가입 성공에 대한 알림창 팝업
                                            });
            });
        }
    }
}