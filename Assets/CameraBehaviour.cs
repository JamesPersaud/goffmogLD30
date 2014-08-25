using UnityEngine;
using System.Collections;
using Assets;

namespace Assets
{

    public class CameraBehaviour : MonoBehaviour
    {
        public static float movespeed = 10;
        public static float minHeight = 25;
        public static float maxHeight = 500;

        public Body target;
        public bool panningToBody;

        // Use this for initialization
        void Start()
        {            
            transform.position = new Vector3(0.0f, 25.0f, -25.0f);            
        }

        // Update is called once per frame
        void Update()
        {
            float height = Mathf.Clamp(transform.position.y + Input.GetAxis("Mouse ScrollWheel") * 100,minHeight,maxHeight);

            float xaxis = Input.GetAxis("Horizontal");
            float zaxis = Input.GetAxis("Vertical");

            Vector3 newpos;

            if(panningToBody)
            {
                Vector3 targetVector = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z - 25);
                float distance = Vector3.Distance(transform.position, targetVector);                

                if(distance <= 20)
                {
                    panningToBody = false;
                }
                else
                {                    
                    float dx = transform.position.x - targetVector.x;
                    float dz = transform.position.z - targetVector.z;
                    float dd = Mathf.Abs(dx + dz);
                    xaxis = dx / dd * -1;
                    zaxis = dz / dd * -1;    
                }

                newpos = (new Vector3(
                (xaxis * Time.deltaTime * movespeed * 15) + transform.position.x,
                height,
                (zaxis * Time.deltaTime * movespeed * 15) + transform.position.z));
            }            
            else
            {
                newpos = (new Vector3(
                (xaxis * Time.deltaTime * movespeed * transform.position.y / 10f) + transform.position.x,
                height,
                (zaxis * Time.deltaTime * movespeed * transform.position.y / 10f) + transform.position.z));
            }                        

            transform.position = newpos;

            transform.LookAt(new Vector3(transform.position.x, 0.0f, transform.position.z + 25));            
        }

        public void StartPanningToBody(Body b)
        {
            target = b;
            panningToBody = true;
        }
    }
}
