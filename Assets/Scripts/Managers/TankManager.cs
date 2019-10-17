using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;            
    public Vector3 m_SpawnPoint;         
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;                     

    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private TankHealth m_Health;
    private TankRadar m_Radar;
    private TankAI m_TankAI;

    private GameObject m_CanvasGameObject;

    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_Health = m_Instance.GetComponent<TankHealth>();
        m_Radar = m_Instance.GetComponent<TankRadar>();
        m_TankAI = m_Instance.GetComponent<TankAI>();


        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }

    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;
        m_Health.enabled = false;
        m_Radar.enabled = false;

        if (m_TankAI) m_TankAI.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }

    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;
        m_Health.enabled = true;
        m_Radar.enabled = true;
        if (m_TankAI) m_TankAI.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }

    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint + Vector3.up;
        m_Instance.transform.rotation = Quaternion.Euler(Vector3.up * 360.0f);

        m_Instance.SetActive(false);

        m_Instance.SetActive(true);
    }
}
