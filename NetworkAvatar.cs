#if ENABLE_PHOTON
using Photon.Pun;
#elif ENABLE_MIRROR
using Mirror;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YourVRExperience.Utils;

namespace YourVRExperience.Network
{
    public class NetworkAvatar :
#if ENABLE_PHOTON
    MonoBehaviour
#elif ENABLE_MIRROR
    NetworkBehaviour
#else
    MonoBehaviour
#endif
    {
        public const string EVENT_NETWORK_AVATAR_APPLY_ROTATION = "EVENT_NETWORK_AVATAR_APPLY_ROTATION";

#if ENABLE_PHOTON
    private bool m_hasBeenInited = true;
#else
        private bool m_hasBeenInited = false;
#endif

        public bool HasBeenInited
        {
            get { return m_hasBeenInited; }
        }

#if ENABLE_PHOTON
    private PhotonView m_photonView;    

    public PhotonView PhotonView
    {
        get
        {
            if (m_photonView == null)
            {
                if (this != null)
                {
                    m_photonView = GetComponent<PhotonView>();
                }                
            }
            return m_photonView;
        }
    }
#elif ENABLE_MIRROR
        [SyncVar]
        public string InitialData;

        [SyncVar]
        public string InitialTypes;

        [SyncVar]
        public int Owner;

        [SyncVar]
        public int InstanceID;

        public object[] InstantiationData;

        private NetworkIdentity m_mirrorView;

        public NetworkIdentity MirrorView
        {
            get
            {
                if (m_mirrorView == null)
                {
                    if (this != null)
                    {
                        m_mirrorView = GetComponent<NetworkIdentity>();
                    }
                }
                return m_mirrorView;
            }
        }

        public void SetInitialData(params object[] _data)
        {
            InitialData = "";
            InitialTypes = "";
            MirrorController.Serialize(_data, ref InitialData, ref InitialTypes);
        }

        public override void OnStartClient()
        {
            List<object> parameters = new List<object>();
            MirrorController.Deserialize(parameters, InitialData, InitialTypes);
            InstantiationData = parameters.ToArray();
            m_hasBeenInited = true;
            SystemEventController.Instance.DispatchSystemEvent(NetworkController.EVENT_MIRROR_NETWORK_AVATAR_INITED, this.gameObject);
        }
#endif
        void Start()
        {
            SystemEventController.Instance.Event += OnSystemEvent;
        }

        void OnDestroy()
        {
            SystemEventController.Instance.Event -= OnSystemEvent;
        }

        private void OnSystemEvent(string _nameEvent, object[] _parameters)
        {
            if (_nameEvent == EVENT_NETWORK_AVATAR_APPLY_ROTATION)
            {
                if (this.gameObject == (GameObject)_parameters[0])
                {
#if ENABLE_MIRROR
                    CmdApplyRotation((Vector3)_parameters[1]);
#endif
                }
            }
        }

        public object[] GetInstantiationData()
        {
#if ENABLE_PHOTON
        return PhotonView.InstantiationData;
#elif ENABLE_MIRROR
            return InstantiationData;
#endif
            return null;
        }

        public void AddSystemEventListener(NetworkController.MultiplayerNetworkEvent _callback)
        {
#if ENABLE_PHOTON
            if (PhotonView != null)
            {
                if (NetworkController.Instance != null) NetworkController.Instance.NetworkEvent += _callback;
            }    
#elif ENABLE_MIRROR
            if (MirrorView != null)
            {
                if (NetworkController.Instance != null) NetworkController.Instance.NetworkEvent += _callback;
            }
#endif
        }

        public void RemoveSystemEventListener(NetworkController.MultiplayerNetworkEvent _callback)
        {
#if ENABLE_PHOTON
        if (PhotonView != null)
        {
            if (NetworkController.Instance != null)
            {
                if (NetworkController.Instance.IsServer) PhotonNetwork.Destroy(PhotonView);
                NetworkController.Instance.NetworkEvent -= _callback;
            }
        }
#elif ENABLE_MIRROR
            if (MirrorView != null)
            {
                if (NetworkController.Instance != null) NetworkController.Instance.NetworkEvent -= _callback;
            }
#endif
        }

        public bool VerifyIdentity(int _netID)
        {
#if ENABLE_PHOTON
        if (PhotonView != null)
        {
            return (PhotonView.ViewID == _netID);
        }
#elif ENABLE_MIRROR
            if (MirrorView != null)
            {
                return (MirrorView.netId == _netID);
            }
#endif
            return false;
        }


        public int GetNetID()
        {
#if ENABLE_PHOTON
        if (PhotonView != null)
        {
            return PhotonView.ViewID;
        }
#elif ENABLE_MIRROR
            if (MirrorView != null)
            {
                return (int)MirrorView.netId;
            }
#endif
            return -1;
        }

        public bool IAmOwner()
        {
#if ENABLE_PHOTON
        if (PhotonView != null)
        {
            return PhotonView.AmOwner;
        }
#elif ENABLE_MIRROR
            if (MirrorView != null)
            {
                return Owner == MirrorController.Instance.UniqueNetworkID;
            }
#endif
            return false;
        }

#if ENABLE_MIRROR
        [Command]
        public void CmdMoveToPosition(Vector3 _targetPosition, bool _useRigidBody)
        {
            Vector3 increment = _targetPosition;
            if (_useRigidBody)
            {
                transform.GetComponent<Rigidbody>().MovePosition(transform.position + increment);
            }
            else
            {
                transform.position += (increment / 4);
            }
        }

        [Command]
        public void CmdJump(Vector3 _force)
        {
            this.GetComponent<Rigidbody>().AddForce(_force);
        }

        [Command]
        public void CmdSetEulerAngles(Vector3 _eulerAngles)
        {
            transform.eulerAngles = _eulerAngles;
        }

        [Command]
        public void CmdApplyRotation(Vector3 _degreess)
        {
            transform.Rotate(_degreess);
        }
#endif
    }
}
