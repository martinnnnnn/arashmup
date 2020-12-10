using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using DG.Tweening;
using System;
using UnityEngine.UI;


namespace Arashmup
{
    public class GameUIManager : MonoBehaviour
    {
        public TMP_Text countDown;
        public TMP_Text youDied;
        public TMP_Text youWon;
        public TMP_Text aliveCount;
        public TMP_Text ammoLeft;
        public ProgressBar dashProgressBar;
        public ProgressBar fireProgressBar;
        [HideInInspector] public Action OnCountDownOver;


        private void Awake()
        {
            countDown.gameObject.SetActive(false);
            youDied.gameObject.SetActive(false);
            youWon.gameObject.SetActive(false);
            aliveCount.gameObject.SetActive(false);
        }

        public void StartCountDown()
        {
            StartCoroutine(StartCountDownRoutine());
        }

        public IEnumerator StartCountDownRoutine()
        {
            countDown.gameObject.SetActive(true);

            countDown.text = "5";
            countDown.transform.localScale = Vector3.zero;

            countDown.transform.DOScale(Vector3.one, 0.5f)
                .OnComplete(() =>
                {
                    countDown.transform.DOScale(Vector3.zero, 0.1f)
                    .OnComplete(() =>
                    {
                        countDown.text = "4";
                        countDown.transform.DOScale(Vector3.one, 0.5f)
                        .OnComplete(() =>
                        {
                            countDown.transform.DOScale(Vector3.zero, 0.1f)
                            .OnComplete(() =>
                            {
                                countDown.text = "3";
                                countDown.transform.DOScale(Vector3.one, 0.5f)
                                .OnComplete(() =>
                                {
                                    countDown.transform.DOScale(Vector3.zero, 0.1f)
                                    .OnComplete(() =>
                                    {
                                        countDown.text = "2";
                                        countDown.transform.DOScale(Vector3.one, 0.5f)
                                        .OnComplete(() =>
                                        {
                                            countDown.transform.DOScale(Vector3.zero, 0.1f)
                                            .OnComplete(() =>
                                            {
                                                countDown.text = "1";
                                                countDown.transform.DOScale(Vector3.one, 0.5f)
                                                .OnComplete(() =>
                                                {
                                                    countDown.transform.DOScale(Vector3.zero, 0.1f).
                                                    OnComplete(() =>
                                                    {
                                                        countDown.gameObject.SetActive(false);
                                                        aliveCount.gameObject.SetActive(true);
                                                        aliveCount.GetComponent<TMP_Text>().text = PhotonNetwork.PlayerList.Count().ToString();

                                                        OnCountDownOver();
                                                    });
                                                });
                                            });
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            yield return null;
        }
    }
}