using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMoDistSet : MonoBehaviour
{
    [SerializeField] Shader slowMoShader;
    [SerializeField] HashSet<Material> mats;
    [SerializeField] float curveWidth = 0.04f;
    [SerializeField] float thresholdEdge0 = 0.1f;
    [SerializeField] float thresholdEdge1 = 0.2f;
    [SerializeField] Color SlowMoTint;

    private void Start()
    {
        mats = new HashSet<Material>();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer r in renderers)
        {
            foreach(Material m in r.materials)
            {
                if (m.shader == slowMoShader) mats.Add(m);
            }
        }
    }
    private void Update()
    {
        foreach(Material m in mats)
        {
            m.SetFloat("Vector1_C1D16EB5", Mathf.Sin(Time.realtimeSinceStartup));
            m.SetFloat("Vector1_4EF7D7A2", curveWidth);
            m.SetFloat("Vector1_D7101408", thresholdEdge0);
            m.SetFloat("Vector1_5ED7C484", thresholdEdge1);
            m.SetColor("Color_BCF9F6A2", SlowMoTint);
        }
    }
}
