using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GodSpeedGames.Tools;

public class ColorPalletManager : Singleton<ColorPalletManager>
{
    public ColorPallet[] pallets;

    public bool test;
    [Condition("test", true)]
    public ColorPallet testPallet;

    [Header("Props Materials")]
    public Material prop_1;
    public Material prop_2;
    public Material book_and_cone;

    [Header("Mat Materials")]
    public Material mat_1;
    public Material mat_2;
    public Material mat_3;
    public Material mat_4;

    public bool randomPattern = true;

    [Header("Mat Texture Patters")]
    public Texture[] pattern;

    List<Material> mat = new List<Material>();
    ColorPallet _pallet;

    private int patternIndex;

    void Start()
    {
        patternIndex = -1;

        _pallet = pallets[Random.Range(0, pallets.Length)];
        if (test)
            _pallet = testPallet;

        GameObject sceneLight = GameObject.FindGameObjectWithTag("MainLight");
        Light _light = sceneLight.GetComponent<Light>();
        if (_light)
            _light.color = _pallet.sceneLightColor;

        RenderSettings.skybox = _pallet.skybox;
        RenderSettings.ambientSkyColor = _pallet.skyColor;
        RenderSettings.ambientEquatorColor = _pallet.equuatorColor;
        RenderSettings.ambientGroundColor = _pallet.groundColor;

        if (prop_1)
        {
            prop_1.mainTexture = _pallet.propTexture_1;
        }

        if (prop_2)
        {
            prop_2.mainTexture = _pallet.propTexture_2;
        }

        if (book_and_cone)
        {
            book_and_cone.mainTexture = _pallet.book_and_cone_texture;
        }

        if (mat_1)
        {
            mat_1.color = _pallet.mat_1;
            mat_1.SetTexture("_MainTex", GetTexture());
        }

        if (mat_2)
        {
            mat_2.color = _pallet.mat_2;
            mat_2.SetTexture("_MainTex", GetTexture());
        }

        if (mat_3)
        {
            mat_3.color = _pallet.mat_3;
            mat_3.SetTexture("_MainTex", GetTexture());
        }

        if (mat_4)
        {
            mat_4.color = _pallet.mat_4;
            mat_4.SetTexture("_MainTex", GetTexture());
        }

        AddMaterials();
    }

    Texture GetTexture()
    {
        if (randomPattern || patternIndex < 0)
            patternIndex = Random.Range(0, pattern.Length);

        return pattern[patternIndex];
    }

    public Color SunLight
    {
        get
        {
            return _pallet.sceneLightColor;
        }
    }

    public Material GetMaterial
    {
        get
        {
            if (mat.Count == 0)
                AddMaterials();

            int random = Random.Range(0, mat.Count);
            Material _mat = mat[random];
            mat.RemoveAt(random);
            return _mat;
            //return mat[Random.Range(0, mat.Count)];
        }
    }

    void AddMaterials()
    {
        mat.Clear();
        mat.Add(mat_1);
        mat.Add(mat_2);
        mat.Add(mat_3);
        mat.Add(mat_4);
    }
}
