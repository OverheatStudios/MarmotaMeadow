using DiscordRPC;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "DiscordRPCHandler", menuName = "Scriptable Objects/DiscordRPCHandler")]
public class DiscordRPCHandler : ScriptableObject
{
    [SerializeField] private string m_clientId;
    private DiscordRpcClient m_client;

    private void OnEnable()
    {
        if (m_clientId == null || m_clientId.Length == 0)
        {
            Debug.LogWarning("No discord RPC client id set");
            return;
        }

        m_client = new DiscordRpcClient(m_clientId);
        m_client.Initialize();

        UpdatePresence("");
    }

    private void OnDisable()
    {
        m_client?.Dispose();
        m_client = null;
    }

    public void UpdatePresence(string details)
    {
        if (m_client == null || m_client.IsDisposed)
        {
            return;
        }

        m_client.SetPresence(new RichPresence()
        {
            Details = details,
            State = "Playing",
            Assets = new Assets()
            {
                LargeImageKey = "large",
                LargeImageText = "Marmota Meadow",
                SmallImageKey = "small"
            }
        });
    }
}
