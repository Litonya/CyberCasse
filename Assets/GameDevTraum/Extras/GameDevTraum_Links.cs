using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTraum
{
    namespace Extras
    {
        public class GameDevTraum_Links : MonoBehaviour
        {
            public GameObject linkWindow;
            public string channelURL;
            public string moreDownloadsURL;
            public string videoURL;

            private void Start()
            {
                linkWindow.SetActive(true);
            }

            void Update()
            {

                if (Input.GetKeyDown(KeyCode.F1))
                {
                    Application.OpenURL(channelURL);
                }
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    Application.OpenURL(moreDownloadsURL);
                }
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    Application.OpenURL(videoURL);
                }
            }

        }
    }
  
}
