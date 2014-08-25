using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Messaging
    {
        public static float MESSAGE_GRAVITY = 200.0f;
        public float glowtime;
        public float maxglow;

        public List<GUITexture> messageHUDObjects;
        public List<string> messages;        

        public void DoGravity(float delta)
        {
            float expectedLeft = Screen.width - 64;
            float expectedBottom = 0;

            for(int i = 0; i < messageHUDObjects.Count; i++)
            {
                expectedBottom = 64 * i;
                GUITexture tex = messageHUDObjects[i];

                if (tex.pixelInset.top > expectedBottom)
                    tex.pixelInset = new Rect(
                        tex.pixelInset.xMin,
                        Mathf.Max(tex.pixelInset.yMin - MESSAGE_GRAVITY * delta, 0),
                        tex.pixelInset.width,tex.pixelInset.height);
            }
        }

        public string PopMessage(GUITexture t)
        {
            int i = messageHUDObjects.IndexOf(t);

            string s = messages[i];
            messages.RemoveAt(i);
            GameObject.Destroy(messageHUDObjects[i].gameObject);
            messageHUDObjects.RemoveAt(i);

            return s;
        }

        public void PushMessage(string s)
        {
            messages.Add(s);
            GameObject o = new GameObject();
            GUITexture t = o.AddComponent<GUITexture>();             
            messageHUDObjects.Add(t);
            t.gameObject.name = "Message_" + (messageHUDObjects.Count).ToString();
            GuiMessage mess = o.AddComponent<GuiMessage>();
            mess.GuiTexture = t;

            t.texture = Resources.Load<Texture>("messagehud");
            t.transform.position = Vector3.zero;
            t.transform.localScale = Vector3.zero;
            t.pixelInset = new Rect(Screen.width - 64, Screen.height, 64, 64);
        }

        private Messaging()
        {
            messageHUDObjects = new List<GUITexture>();
            messages = new List<string>();
        }

        private static Messaging instance;
        public static Messaging Instance
        {
            get
            {
                if (instance == null)
                    instance = new Messaging();

                return instance;
            }
        }

    }
}
