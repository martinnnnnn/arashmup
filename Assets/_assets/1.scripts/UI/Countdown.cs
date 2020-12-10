using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;


namespace Arashmup
{
    public class Countdown : MonoBehaviour
    {
        TMP_Text text;

        public GameEvent CountdownOver;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            text.enabled = false;
        }

        public void OnGameInitialized()
        {
            StartCoroutine(StartCountDownRoutine());
        }

        public IEnumerator StartCountDownRoutine()
        {
            text.enabled = true;
            text.text = "5";
            text.transform.localScale = Vector3.zero;

            text.transform.DOScale(Vector3.one, 0.5f)
                .OnComplete(() =>
                {
                    text.transform.DOScale(Vector3.zero, 0.1f)
                    .OnComplete(() =>
                    {
                        text.text = "4";
                        text.transform.DOScale(Vector3.one, 0.5f)
                        .OnComplete(() =>
                        {
                            text.transform.DOScale(Vector3.zero, 0.1f)
                            .OnComplete(() =>
                            {
                                text.text = "3";
                                text.transform.DOScale(Vector3.one, 0.5f)
                                .OnComplete(() =>
                                {
                                    text.transform.DOScale(Vector3.zero, 0.1f)
                                    .OnComplete(() =>
                                    {
                                        text.text = "2";
                                        text.transform.DOScale(Vector3.one, 0.5f)
                                        .OnComplete(() =>
                                        {
                                            text.transform.DOScale(Vector3.zero, 0.1f)
                                            .OnComplete(() =>
                                            {
                                                text.text = "1";
                                                text.transform.DOScale(Vector3.one, 0.5f)
                                                .OnComplete(() =>
                                                {
                                                    text.transform.DOScale(Vector3.zero, 0.1f).
                                                    OnComplete(() =>
                                                    {
                                                        text.enabled = false;
                                                        CountdownOver.Raise();
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

//aliveCount.gameObject.SetActive(true);
//aliveCount.GetComponent<TMP_Text>().text = PhotonNetwork.PlayerList.Count().ToString();