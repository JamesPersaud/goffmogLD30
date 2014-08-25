using System;
using UnityEngine;

namespace Assets
{
    public class Helpers
    {
        public const int NOISE_TEXTURE_SIZE = 128;

        public static float tooltipdelay = 0.01f;

        public static System.Random GalacticRandom = new System.Random();
        public static int perlinX = 0;
        public static int perlinY = 0;

        public static int[] fib = { 1,1,2,3,5,8,13,21,34,55,89,144,223,377,610,987};

        public static string[] alpha = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        public static string[] numer = new string[] { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX", "XXI", "XXII", "XXIII", "XXIV", "XXV", "XXVI", "XXVII" };

        public static string getAlpha(int i)
        {
            return alpha[i];
        }

        public static string getNumeral(int i)
        {
            return numer[i];
        }

        public static int GetFibonacci(int min, int max)
        {                        
            return fib[GalacticRandom.Next(min, max)];
        }        

        public static int GetSmallFibonacci()
        {
            return fib[GalacticRandom.Next(4)];
        }

        public static Vector3 PoleToCart(float radius, float angle, Vector3 offset)
        {
            Vector3 v = new Vector3();
            v.x = radius * Mathf.Cos(angle);
            v.z = radius * Mathf.Sin(angle);
            return v + offset;
        }        

        public static void AddTrails(GameObject o, Body body)
        {
            TrailRenderer tr = o.AddComponent<TrailRenderer>();
            tr.castShadows = false;
            tr.receiveShadows = false;
            tr.material = getTrailMaterial();
            if(body != null)
                tr.time = body.Period * 4;
            else
                tr.time = 4;
            tr.startWidth = 0.1f;
            tr.endWidth = 0.1f;              
        }

        public static Material getTrailMaterial()
        {            
            Shader lineshader = Shader.Find("VertexLit");
            Material m = new Material(lineshader);            
            m.mainTexture = (Texture)Resources.Load<Texture>("line");                
            if (m.mainTexture == null)
            {
                Debug.LogError("Failed to load line material");
            }

            return m;
        }

        public static Material getPlanetMaterial(Body planet)
        {
            Material mat = new Material(Shader.Find("goffmog/PlanetShader"));
            mat.mainTexture = getPlanetTexture(planet);
            return mat;
        }

        public static Texture2D getPlanetTexture(Body body)
        {
            Texture2D tex = new Texture2D(NOISE_TEXTURE_SIZE, NOISE_TEXTURE_SIZE);
            Color[] cols = new Color[NOISE_TEXTURE_SIZE * NOISE_TEXTURE_SIZE];

            int x = 0;
            int y = 0;
            while (y < tex.height)
            {
                x = 0;
                while (x < tex.width)
                {
                    float xCoord = perlinX + (float)x / tex.width * 2f * body.BodyRadius;
                    float yCoord = perlinY + (float)y / tex.height * 2f * body.BodyRadius;
                    float sample = Mathf.PerlinNoise(xCoord, yCoord);
                    cols[y * tex.width + x] = Color.Lerp(body.SkyColor, body.CloudColor, sample);                    
                    x++;
                }
                y++;
            }
            
            perlinX = x;
            perlinY = y;

            tex.SetPixels(cols);
            tex.Apply();

            return tex;
        }
    }
}

