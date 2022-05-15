#if ENABLE_PHOTON
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourVRExperience.Network
{
    public class NetworkController : ScriptableObject
    {
        public const string EVENT_NETWORK_CONNECTION_WITH_ROOM = "EVENT_NETWORK_CONNECTION_WITH_ROOM";

        public const string EVENT_MIRROR_NETWORK_AVATAR_INITED = "EVENT_MIRROR_NETWORK_AVATAR_INITED";
        public const string EVENT_MIRROR_LOCAL_CONNECTION = "EVENT_MIRROR_LOCAL_CONNECTION";
        public const string EVENT_MIRROR_NEW_CLIENT_CONNECTION = "EVENT_MIRROR_NEW_CLIENT_CONNECTION";


        public const bool DEBUG = true;
        public const string MY_ROOM_NAME = "MyRoomName";

        public byte PHOTON_EVENT_CODE = 0;

        // ----------------------------------------------
        // NETWORK EVENT
        // ----------------------------------------------	
        public delegate void MultiplayerNetworkEvent(string _nameEvent, int _originNetworkID, int _targetNetworkID, params object[] _parameters);

        public event MultiplayerNetworkEvent NetworkEvent;

        public void DispatchEvent(string _nameEvent, int _originNetworkID, int _targetNetworkID, params object[] _parameters)
        {
            if (NetworkEvent != null) NetworkEvent(_nameEvent, _originNetworkID, _targetNetworkID, _parameters);
        }

        // ----------------------------------------------
        // SINGLETON
        // ----------------------------------------------	
        private static NetworkController _instance;

        public static NetworkController Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType<NetworkController>();
                    if (_instance == null)
                    {
                        _instance = ScriptableObject.CreateInstance<NetworkController>();
                    }
                }
                return _instance;
            }
        }

        private int m_totalNumberOfPlayers = -1;

        public int UniqueNetworkID
        {
            get
            {
#if ENABLE_PHOTON
            return PhotonController.Instance.UniqueNetworkID;
#elif ENABLE_MIRROR
                return MirrorController.Instance.UniqueNetworkID;
#else
            return -1;
#endif
            }
        }
        public bool IsServer
        {
            get
            {
#if ENABLE_PHOTON
            return PhotonNetwork.LocalPlayer.IsMasterClient;
#elif ENABLE_MIRROR
                return MirrorController.Instance.IsServer;
#else
            return false;
#endif
            }
        }
        public bool IsConnected
        {
            get
            {
#if ENABLE_PHOTON
            return PhotonController.Instance.IsConnected;
#elif ENABLE_MIRROR
                return MirrorController.Instance.IsConnected;
#else
            return false;
#endif
            }
        }

        public void Connect()
        {
#if ENABLE_PHOTON
        PhotonController.Instance.Connect();
#elif ENABLE_MIRROR
            MirrorController.Instance.Connect();
#endif
        }

        public void DispatchNetworkEvent(string _nameEvent, int _originNetworkID, int _targetNetworkID, params object[] _list)
        {
#if ENABLE_PHOTON
        PhotonController.Instance.DispatchNetworkEvent(_nameEvent, _originNetworkID, _targetNetworkID, _list);
#elif ENABLE_MIRROR
            MirrorController.Instance.DispatchNetworkEvent(_nameEvent, _originNetworkID, _targetNetworkID, _list);
#endif
        }
    }
}