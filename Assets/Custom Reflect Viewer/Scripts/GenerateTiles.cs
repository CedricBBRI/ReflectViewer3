﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateTiles : MonoBehaviour
{
    public Material compMat = null;
    public Slider sliderWidth1;
    public Slider sliderHeight1;
    public Slider sliderWidth2;
    public Slider sliderHeight2;
    public int resolutionTexture;
    public Color tileColor;
    public Color mortarColor;
    public Texture2D tileTexture;
    Texture2D tileTextureNormal;
    public Text textW1;
    public Text textH1;
    public Text textW2;
    public Dropdown matDropdown;

    public void generateTiles()
    {
        float tileWidth = sliderWidth1.value;
        float tileHeight = sliderHeight1.value;
        float mortarWidth = sliderWidth2.value;
        float mortarHeight = sliderHeight2.value;
        float totalWidth = tileWidth + mortarWidth;
        float totalHeight = tileHeight + mortarHeight;
        int totalWidthInt = resolutionTexture;
        int totalHeightInt = (int)Mathf.Floor(totalHeight / totalWidth * resolutionTexture);
        int mortarWidthInt = (int)Mathf.Floor(mortarWidth / totalWidth * resolutionTexture);
        int mortarHeightInt = (int)Mathf.Floor(mortarHeight / totalHeight * resolutionTexture);
        tileTexture = new Texture2D(totalWidthInt, totalHeightInt, TextureFormat.ARGB32, false);
        tileTextureNormal = new Texture2D(totalWidthInt, totalHeightInt, TextureFormat.ARGB32, false);
        float metallicness = 0;

        Debug.Log(totalWidthInt.ToString() + ", " + totalHeightInt.ToString());

        if (matDropdown.options[matDropdown.value].text.Equals("Tiles"))
        {
            for (int i = 0; i < totalWidthInt; i++)
            {
                for (int j = 0; j < totalHeightInt; j++)
                {
                    if (i < (int)Mathf.Round(mortarWidthInt / 2f) || j < (int)Mathf.Round(mortarHeightInt / 2f) || i >= totalWidthInt - (int)Mathf.Round(mortarWidthInt / 2f) || j >= totalHeightInt - (int)Mathf.Round(mortarHeightInt / 2f))
                    //if (i < mortarWidthInt || j < mortarHeightInt)
                    {
                        tileTexture.SetPixel(i, j, mortarColor);
                        //tileTextureNormal.SetPixel(i, j, Color.black);
                    }
                    else
                    {
                        tileTexture.SetPixel(i, j, tileColor);
                        //tileTextureNormal.SetPixel(i, j, Color.white);
                    }
                }
            }
            metallicness = 0.85f;
            tileTextureNormal = Resources.Load<Texture2D>("Materials/normalMap_Tiles");
        }
        else if (matDropdown.options[matDropdown.value].text.Equals("Bricks"))
        {
            for (int i = 0; i < totalWidthInt; i++)
            {
                for (int j = 0; j < totalHeightInt; j++)
                {
                    //Outer borders
                    if ((i < (int)Mathf.Round(mortarWidthInt / 2f) && j <= totalHeightInt/2f) || j < (int)Mathf.Round(mortarHeightInt / 2f) || (i >= totalWidthInt - (int)Mathf.Round(mortarWidthInt / 2f) && j <= totalHeightInt / 2f) || j >= totalHeightInt - (int)Mathf.Round(mortarHeightInt / 2f))
                    {
                        tileTexture.SetPixel(i, j, mortarColor);
                        //tileTextureNormal.SetPixel(i, j, Color.black);
                    }
                    else if(j < totalHeightInt / 2f + (int)Mathf.Round(mortarHeightInt / 2f) && j > totalHeightInt / 2f - (int)Mathf.Round(mortarHeightInt / 2f)) //middle horizontal
                    {
                        tileTexture.SetPixel(i, j, mortarColor);
                    }
                    else if(j > totalHeightInt / 2f && i < totalWidthInt / 2f + (int)Mathf.Round(mortarWidthInt / 2f) && i > totalWidthInt / 2f - (int)Mathf.Round(mortarWidthInt / 2f))
                    {
                        tileTexture.SetPixel(i, j, mortarColor);
                    }
                    else
                    {
                        tileTexture.SetPixel(i, j, tileColor);
                        //tileTextureNormal.SetPixel(i, j, Color.white);
                    }
                }
            }
            metallicness = 0.0f;
            tileTextureNormal = Resources.Load<Texture2D>("Materials/normalMap_Bricks");
        }
        tileTexture.Apply();
        //tileTextureNormal.Apply();

        textW1.text = "Tile width: " + string.Format("{0:N3}", tileWidth) + "m";
        textH1.text = "Tile height: " + string.Format("{0:N3}", tileHeight) + "m";
        textW2.text = "Mortar width: " + string.Format("{0:N3}", mortarWidth) + "m";

        compMat.mainTexture = tileTexture;
        compMat.EnableKeyword("_NORMALMAP");
        compMat.SetTexture("_BumpMap", tileTextureNormal);
        compMat.mainTextureScale = new Vector2(1/totalWidth, 1/totalHeight);
        compMat.SetFloat("_Metallic", metallicness);
        //compMat.shader = Shader.Find("Diffuse");
    }

    // Start is called before the first frame update
    void Start()
    {
        generateTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
