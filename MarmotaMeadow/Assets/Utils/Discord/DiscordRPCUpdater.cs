using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscordRPCUpdater : MonoBehaviour
{
    [SerializeField] private string m_details = "In A Menu";
    [SerializeField] private DiscordRPCHandler m_rpcHandler;

    void Start()
    {
        StartCoroutine(UpdateRPC());
    }

    IEnumerator UpdateRPC()
    {
        yield return new WaitForSeconds(0.1f);
        m_rpcHandler.UpdatePresence(m_details);
    }
}
