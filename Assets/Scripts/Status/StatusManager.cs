﻿// ===========================================================================================================
//
// Class/Library: StatusManager  (Singleton)
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Apr 21, 2016
//	
// VERS 1.0.000 : Apr 21, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else
	#undef		IS_DEBUGGING
#endif

#define	USES_APPLICATIONMANAGER		// #define = Scene has an ApplicationManager Prefab,	#undef = Scene does not have an ApplicationManager Prefab
#define	USES_DATABASEMANAGER			// #define = Scene has a DatabaseManager Prefab,			#undef = Scene does not have an DatabaseManager Prefab
#define	USES_NETWORKMANAGER				// #define = Scene has a AppNetworkManager Prefab,		#undef = Scene does not have an AppNetworkManager Prefab

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusManager : MonoBehaviour 
{

	#region "PRIVATE VARIABLES"

		static					StatusManager				_instance					= null;

		#if USES_APPLICATIONMANAGER
		private					ApplicationManager	_apm							= null;
		#endif
		#if USES_NETWORKMANAGER
		private					AppNetworkManager		_nwm							= null;
		#endif
		#if USES_DATABASEMANAGER
		private					DatabaseManager			_dbm							= null;
		#endif

		private					Image								_img							= null;
		private					Text								_txtClass					= null;
		private					Text								_txtPCcnt					= null;
		private					Text								_txtStatus				= null;
		private					Image								_connNet					= null;
		private					Image								_connUsr					= null;
		private					Image								_connDB						= null;
		
		private					Coroutine						_lastCoroutine		= null;
		private					bool								_blnInitialized		= false;

	#endregion

	#region "PRIVATE PROPERTIES"

		public	static	StatusManager				Instance
		{
			get
			{
				return GetInstance();
			}
		}
		public	static	StatusManager				GetInstance()
		{
			if (_instance == null)
					_instance = (StatusManager)GameObject.FindObjectOfType(typeof(StatusManager));
			if (!_instance.gameObject.activeSelf)
					_instance.gameObject.SetActive(true);
			return _instance;
		}

		#if USES_APPLICATIONMANAGER
		private ApplicationManager					App
		{
			get
			{
				if (_apm == null)
						_apm = ApplicationManager.Instance;
				return _apm;
			}
		}
		#endif
		#if USES_NETWORKMANAGER
		private AppNetworkManager						Net
		{
			get
			{
				if (_nwm == null)
						_nwm = AppNetworkManager.Instance;
				return _nwm;
			}
		}
		#endif
		#if USES_DATABASEMANAGER
		private DatabaseManager							Database
		{
			get
			{
				if (_dbm == null)
						_dbm = DatabaseManager.Instance;
				return _dbm;
			}
		}
		#endif

		private	Image												Img
		{
			get
			{
				if (_img == null)
						_img = GetComponent<Image>();
				return _img;
			}
		}
		private Text												TxtClassification
		{
			get
			{
				if (_txtClass == null)
						_txtClass = transform.GetChild(0).GetComponent<Text>();
				return _txtClass;
			}
		}
		private Text												TxtStatus
		{
			get
			{
				if (_txtStatus == null)
						_txtStatus = transform.GetChild(4).GetComponent<Text>();
				return _txtStatus;
			}
		}
		private Image												ConnNet
		{
			get
			{
				if (_connNet == null)
						_connNet = transform.GetChild(1).GetComponent<Image>();
				return _connNet;
			}
		}
		private Image												ConnUSR
		{
			get
			{
				if (_connUsr == null)
						_connUsr = transform.GetChild(2).GetComponent<Image>();
				return _connUsr;
			}
		}
		private Image												ConnDB
		{
			get
			{
				if (_connDB == null)
						_connDB = transform.GetChild(3).GetComponent<Image>();
				return _connDB;
			}
		}
		private Text												TxtPCcnt
		{
			get
			{
				if (_txtPCcnt == null)
						_txtPCcnt = transform.GetChild(2).GetChild(0).GetComponent<Text>();
				return _txtPCcnt;
			}
		}

	#endregion

	#region "PUBLIC PROPERTIES"

		public	string			Status
		{
			get
			{
				return TxtStatus.text;
			}
			set
			{
				try 
				{
					if (!_blnInitialized)
					{ 
						GetInstance();
						_blnInitialized = true;
					}
					this.gameObject.SetActive(true);
					if (_lastCoroutine != null)
							StopCoroutine(_lastCoroutine);
					_lastCoroutine = StartCoroutine(UpdateStatusText(value));
				} catch { }
			}
		}
		public	string			StatusText
		{
			set
			{
				TxtStatus.text = value;
			}
		}

	#endregion

	#region "START FUNCTION"

		private void		Awake()
		{
			GetInstance();

			#if USES_APPLICATIONMANAGER
			_apm = ApplicationManager.Instance;
			#endif
			#if USES_DATABASEMANAGER
			_dbm = DatabaseManager.Instance;
			#endif
			#if USES_NETWORKMANAGER
			_nwm = AppNetworkManager.Instance;
			#endif
		}
		private void		Start()
		{
			// SET VARIABLES
			_img				= GetComponent<Image>();
			_txtClass		= transform.GetChild(0).GetComponent<Text>();
			_connNet		= transform.GetChild(1).GetComponent<Image>();
			_connUsr		= transform.GetChild(2).GetComponent<Image>();
			_connDB			= transform.GetChild(3).GetComponent<Image>();
			_txtStatus	= transform.GetChild(4).GetComponent<Text>();
			_txtPCcnt		= transform.GetChild(2).GetChild(0).GetComponent<Text>();

			// SET OBJECT ON TOP
			transform.SetSiblingIndex(transform.parent.childCount - 1);
			UpdateStatus();
			_blnInitialized = true;
		}

	#endregion

	#region "PRIVATE FUNCTIONS"

		private IEnumerator		UpdateStatusText(string strMessage)
		{
			StopCoroutine("UpdateStatusText");
			#if IS_DEBUGGING
			Debug.Log(strMessage);
			#endif
			TxtStatus.text = strMessage;
			yield return new WaitForSeconds(8.0f);
			TxtStatus.text = "";
			_lastCoroutine = null;
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void					UpdateStatus()
		{
			// DISPLAY CONNECTION STATUS
			#if USES_NETWORKMANAGER
			if (Net != null)
			{ 
				if (Net.IsConnected || Net.IsServer)
					ConnNet.sprite	= Resources.Load<Sprite>("Images/UI/StatusManager/Network-Connected-icon") as Sprite;
				else
					ConnNet.sprite	= Resources.Load<Sprite>("Images/UI/StatusManager/Network-Disconnected-icon") as Sprite;
				ConnNet.gameObject.SetActive(!Net.IsWorkingOffline);
			} else
				ConnNet.gameObject.SetActive(false);
			#else
			ConnNet.gameObject.SetActive(false);
			#endif

			// DISPLAY DATABASE STATUS
			#if USES_DATABASEMANAGER
				bool bln = Database.ClientsCanUse;
				#if USES_NETWORKMANAGER
						bln = bln || ((Net != null && Net.IsServer));
				#endif
				#if USES_APPLICATIONMANAGER
						bln = bln && App.CanWorkOffline;
				#endif

				if (bln)
				{ 
					ConnDB.gameObject.SetActive(true);
				 
					if (Database != null && Database.IsConnected)
						ConnDB.sprite		= Resources.Load<Sprite>("Images/UI/StatusManager/Database-Connected-icon") as Sprite;
					else
						ConnDB.sprite		= Resources.Load<Sprite>("Images/UI/StatusManager/Database-Disconnected-icon") as Sprite;
				} else {
					ConnDB.gameObject.SetActive(false);
				}
			#else
			ConnDB.gameObject.SetActive(false);
			#endif

			// DISPLAYER LOGIN STATUS (CLIENT) / PLAYER COUNT (SERVER)
			#if USES_NETWORKMANAGER
			if (Net != null && (Net.IsLoggedIn || Net.IsServer))
			{
				ConnUSR.sprite	= Resources.Load<Sprite>("Images/UI/StatusManager/User-Connected-icon") as Sprite;
				TxtPCcnt.text		= (Net.IsServer) ? Net.PlayerCount.ToString() : "";
			} else {
				ConnUSR.sprite	= Resources.Load<Sprite>("Images/UI/StatusManager/User-Disconnected-icon") as Sprite;
				TxtPCcnt.text		= "";
			}
			#else
			ConnUSR.gameObject.SetActive(false);
			#endif

			// DISPLAY CLASSIFICATION LEVEL
			#if USES_APPLICATIONMANAGER
			TxtClassification.text = App.ApplicationClassifiedTitle;
			Img.color = App.ApplicationClassifiedColor;
			#else
			TxtClassification.text = "";
			Img.color = new Color(0.125f, 0.125f, 0.125f, 0);
			#endif

			// INSURE THAT THIS ITEM IS ON TOP
			transform.SetSiblingIndex(transform.parent.childCount - 1);
		}

	#endregion

}
					
