using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorPallet", menuName = "Godspeed/ColorPallet")]
public class ColorPallet : ScriptableObject
{
    [Header("Directional Light Color")]
    public Color sceneLightColor;

    [Header("Ambient Color")]
    public Color skyColor;
    public Color equuatorColor;
    public Color groundColor;

    public Material skybox;

    [Header("Props")]
    public Texture propTexture_1;
    public Texture propTexture_2;
    public Texture book_and_cone_texture;

    [Header("Mat")]
    public Color mat_1;
    public Color mat_2;
    public Color mat_3;
    public Color mat_4;
}
