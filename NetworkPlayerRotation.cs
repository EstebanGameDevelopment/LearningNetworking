#if ENABLE_PHOTON
using Photon.Pun;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YourVRExperience.Utils;

namespace YourVRExperience.Network
{
    public class NetworkPlayerRotation : MonoBehaviour
    {
        public const string EVENT_MIRROR_ROTATION_AVATAR_INITED = "EVENT_MIRROR_ROTATION_AVATAR_INITED";

        public int NetworkViewPlayerID = -1;
        private bool m_hasBeenInited = false;

#if ENABLE_PHOTON || ENABLE_MIRROR
        private NetworkAvatar m_networkView;

        public NetworkAvatar NetworkView
        {
            get
            {
                if (m_networkView == null)
                {
                    m_networkView = GetComponent<NetworkAvatar>();
                }
                return m_networkView;
            }
        }

        void Start()
        {
            if (NetworkController.Instance.IsConnected)
            {
                InitializeNetworkData();
            }
            SystemEventController.Instance.Event += OnSystemEvent;
            SystemEventController.Instance.DispatchSystemEvent(EVENT_MIRROR_ROTATION_AVATAR_INITED, this);
        }

        void OnDestroy()
        {
            SystemEventController.Instance.Event -= OnSystemEvent;
        }

        private void InitializeNetworkData()
        {
            if ((NetworkView != null) && NetworkView.HasBeenInited && !m_hasBeenInited)
            {
                if (NetworkView.GetInstantiationData().Length > 0)
                {
                    NetworkViewPlayerID = (int)NetworkView.GetInstantiationData()[0];
                }
            }
        }

        private void OnSystemEvent(string _nameEvent, object[] _parameters)
        {
            if (_nameEvent == NetworkController.EVENT_MIRROR_NETWORK_AVATAR_INITED)
            {
                if (this.gameObject == (GameObject)_parameters[0])
                {
                    InitializeNetworkData();
                }
            }
        }
#endif
    }
}