﻿// ===========================================================================================================
//
// Class/Library: ApplicationManager (Singleton Class)
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

#define	USES_STATUSMANAGER				// #define = Scene has a  StatusManager Prefab,			#undef = Scene does not have a  StatusManager Prefab
#define	USES_DATABASEMANAGER			// #define = Scene has a  DatabaseManager Prefab,		#undef = Scene does not have a  DatabaseManager Prefab
#define	USES_NETWORKMANAGER				// #define = Scene has an AppNetworkManager Prefab,	#undef = Scene does not have an AppNetworkManager Prefab

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public	partial	class	ApplicationManager : MonoBehaviour
{

	#region "PRIVATE VARIABLES"

		static	ApplicationManager	_instance							= null;

		[SerializeField]
		private bool								_blnCanWorkOffline		= false;


		private bool								_blnIsLoggedIn				= false;
		private bool								_blnIsWorkingOffline	= false;

		#if USES_NETWORKMANAGER
		private GameObject					_userObj							= null;
		private User								_user									= null;
		#endif

		#if UNITY_WEBPLAYER
		public	static	string			webplayerQuitURL		= "http://google.com";
		#endif

		private string							_strDebugLog				= "";

	#endregion

	#region "PRIVATE PROPERTIES"

		#if USES_STATUSMANAGER
		private StatusManager				_stm					= null;
		#endif
		#if USES_DATABASEMANAGER
		private DatabaseManager			_dbm					= null;
		#endif
		#if USES_NETWORKMANAGER
		private AppNetworkManager		_nwm					= null;
		#endif

		#if USES_STATUSMANAGER
		private StatusManager				Status
		{
			get
			{
				if (_stm == null)
						_stm = StatusManager.Instance;
				return _stm;
			}
		}
		#endif
		#if USES_DATABASEMANAGER
		private DatabaseManager			Database
		{
			get
			{
				if (_dbm == null)
						_dbm = DatabaseManager.Instance;
				return _dbm;
			}
		}
		#endif
		#if USES_NETWORKMANAGER
		private AppNetworkManager		Net
		{
			get
			{
				if (_nwm == null)
						_nwm = AppNetworkManager.Instance;
				return _nwm;
			}
		}
		#endif

	#endregion

	#region "PUBLIC PROPERTIES"

		public	enum Classifications	: int { Off = 0, Unclassified = 1, Confidential = 2, FOUO = 3, Secret = 4, SecretNoForn = 5, TopSecret = 6, TopSecretSCI = 7, TopSecretSAP = 8 }
		public	enum DevPhases				: int	{ Experimental = 1, Development  = 2, InternalAlpha = 3, ClosedAlpha = 4, ClosedBeta = 5, OpenBeta = 6, Testing = 7, Release = 8 }

		public	static	ApplicationManager	Instance
		{
			get
			{
				return GetInstance();
			}
		}
		public	static	ApplicationManager	GetInstance()
		{
			if (_instance == null)
				_instance = (ApplicationManager)GameObject.FindObjectOfType(typeof(ApplicationManager));
			return _instance;
		}

		public	bool								IsServer
		{
			get
			{
				#if USES_NETWORKMANAGER
				if (Net != null)
					return Net.IsServer;
				else
					return true;
				#else
				return true;
				#endif
			}
		}
		public	bool								IsLoggedIn
		{
			get
			{
				return _blnIsLoggedIn;
			}
			set
			{
				_blnIsLoggedIn = value;
				#if USES_STATUSMANAGER
				if (Status != null)
						Status.UpdateStatus();
				#endif
			}
		}
		public	bool								CanWorkOffline
		{
			get
			{
				#if UNITY_WEBGL
				return false;
				#elif USES_DATABASEMANAGER
				return _blnCanWorkOffline && Database.ClientsCanUse;
				#else
				return _blnCanWorkOffline;
				#endif
			}
			set
			{
				_blnCanWorkOffline = value;
			}
		}
		public	bool								IsWorkingOffline
		{ 
			get
			{
				return _blnIsWorkingOffline;
			}
			set
			{
				_blnIsWorkingOffline = value;
			}
		}
		public	string							GameBuildName
		{
			get
			{
				switch (GameBuildType)
				{
					case 2:	return "Development";
					case 3: return "Internal Alpha";
					case 4: return "Closed Alpha";
					case 5: return "Closed Beta";
					case 6: return "Open Beta";
					case 7: return "Testing";
					case 8: return "Release";
					default: return "Experimental";
				}
			}
		}
		public	string							GetGameBuildName(int i)
		{
			switch (i)
			{
				case 2:	return "Development";
				case 3: return "Internal Alpha";
				case 4: return "Closed Alpha";
				case 5: return "Closed Beta";
				case 6: return "Open Beta";
				case 7: return "Testing";
				case 8: return "Release";
				default: return "Experimental";
			}
		}
		public	string							GameVersionString
		{
			get
			{
				if ((int)AppClassification > 0)
					return AppClassification.ToString() + "\nVersion " + GameVersion + "\n" +GameBuildName;
				else
					return "Version " + GameVersion + "\n" + GameBuildName;
			}
		}
		public	string							GameDisplayString
		{
			get
			{
				return "<size=20><b>" + GameName + "</b></size>\n" + GameVersionString;
			}
		}
		public	string							GameCodeString
		{
			get
			{
				return GameCode + "_" + GameVersion + "_" + GameBuildType.ToString() + "_" + ((int)AppClassification).ToString();
			}
		}

		public	string							ApplicationClassifiedTitle
		{
			get
			{
				switch (AppClassification)
				{
					case Classifications.Unclassified:
						return "UNCLASSIFIED";
					case Classifications.Confidential:
						return "CONFIDENTIAL";
					case Classifications.FOUO:
						return "FOR OFFICIAL USE ONLY";
					case Classifications.Secret:
						return "SECRET RELEASEABLE";
					case Classifications.SecretNoForn:
						return "SECRET / NOFORN";
					case Classifications.TopSecret:
						return "TOP SECRET";
					case Classifications.TopSecretSCI:
						return "TOP SECRET / SCI";
					case Classifications.TopSecretSAP:
						return "TOP SECRET / SAP";
					default:
						return "";
				}
			}
		}
		public	Color								ApplicationClassifiedColor
		{
			get
			{
				switch (AppClassification)
				{
					case Classifications.Unclassified:
					case Classifications.Confidential:
					case Classifications.FOUO:
						return new Color(0, 0.5f, 0);
					case Classifications.Secret:
					case Classifications.SecretNoForn:
						return new Color(0.5f, 0, 0);
					case Classifications.TopSecret:
					case Classifications.TopSecretSCI:
					case Classifications.TopSecretSAP:
						return new Color(0.6875f, 0.55f, 0.0625f);
					default:
						return new Color(0, 0, 0, 0.05f);
				}
			}
		}

		public	int									UserLogInType
		{
			get
			{
				return UserLoginType;
			}
			set
			{
				if (Application.isWebPlayer)
					UserLoginType = 1;
				else
					UserLoginType = value;
			}
		}
		
		#if USES_NETWORKMANAGER
		public	GameObject					UserObj
		{
			get
			{
				return _userObj;
			}
			set
			{
				_userObj = value;
			}
		}
		public	User								GameUser
		{
			get
			{
				if (_user == null)
					if (this.UserObj != null)
						_user = this.UserObj.GetComponent<User>();
				return _user;
			}
			set
			{
				_user = value;
			}
		}
		#endif

		#region "APPLICATION SETTINGS (FROM TEXT FILE)"

			// APPLICATION SETTINGS
			public	string	CLASSIFICATION_HEADING			= "";

			// GAME MANAGER SETTINGS
			public	float		DEFAULT_WIDTH					= 1440;
			public	float		DEFAULT_HEIGHT				= 900;
			public	int			DEFAULT_APP_TIMEOUT		= 60;		// IN MINUTES

		#endregion

	#endregion

	#region "PUBLIC EDITOR PROPERTIES"

		[SerializeField]
		public	string							GameName					= "DEMO GAME";
		[SerializeField]
		public	string							GameCode					= "DEMO_";
		[SerializeField]
		public	string							GameVersion				= "1.00.0000";
		[SerializeField]
		public	int									GameBuildType			= 0;
		[SerializeField]
		public	Classifications			AppClassification = Classifications.Off;

		[SerializeField]
		public	GameObject					GameObjectPrefab	= null;		
		[SerializeField]
		public	int									MaxFPS						= 60;

		[HideInInspector, SerializeField]
		public	int									UserLoginType			= 1;				// 1=Username/Password, 2=Windows Username
		[SerializeField]
		public	bool								AllowSignUp				= true;
		[SerializeField]
		public	bool								AutoCreateAccount	= false;		// ONLY WHEN UserLogInType=2

		[SerializeField]
		public	GameObject					PlayerPrefab			= null;

		[System.NonSerialized, HideInInspector]
		public	GameObject					UserGameObject		= null;

		[SerializeField]
		public	Button							WriteErrorLogButton	= null;

	#endregion

	#region "PRIVATE FUNCTIONS"
	
		private IEnumerator		PrivQuitApplication()
		{
			// WAITING FOR PANELS AND SOUNDS TO FADE OUT
			yield return new WaitForSeconds(2.5f);

			#if USES_NETWORKMANAGER
			if (Net != null && Net.IsConnected)
			{
				if (Net.IsClient) 
					Net.ClientDisconnect();
				else if (Net.IsServer)
					Net.ServerDisconnect();
				else if (Net.IsHost)
					Net.HostDisconnect();
			}
			#endif
			#if USES_DATABASEMANAGER
			if (Database != null && Database.IsConnectedCheck)
					Database.CloseDatabase();
			#endif

			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#elif UNITY_WEBPLAYER
				Application.OpenURL(webplayerQuitURL);
			#else
				Application.Quit();
			#endif
		}
		private void					PrivateQuitApplication()
		{
			try
			{ 
/*
				#if USES_NETWORKMANAGER
				if (Net != null && Net.IsConnected)
				{
					if (Net.IsClient) 
						Net.ClientDisconnect();
					else if (Net.IsServer)
						Net.ServerDisconnect();
					else if (Net.IsHost)
						Net.HostDisconnect();
				}
				#endif
*/
				#if USES_DATABASEMANAGER
				if (Database != null && Database.IsConnectedCheck)
						Database.CloseDatabase();
				#endif

			} catch { }

			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#elif UNITY_WEBPLAYER
				Application.OpenURL(webplayerQuitURL);
			#else
				Application.Quit();
			#endif
			Application.Quit();
		}
		private void					LoadApplicationSettings()
		{
			string[]	strLines = null;
			string		AppSettingsFile = "ApplicationSettings.txt";
			bool			blnOkayToProcessTextFile = false;
			int				intWidth				= 0;
			int				intHeight				= 0;

			if (!Util.FileExists("", AppSettingsFile))
			{
				Status.Status			= "Unable to find file \"" + AppSettingsFile + "\". Using Default Settings.";
			} else {
				strLines = Util.ReadTextFile("", AppSettingsFile).Split('\n');
				blnOkayToProcessTextFile = strLines != null && strLines.Length > 0;
				Status.Status = AppSettingsFile + " found. " + strLines.Length.ToString() + " lines Read In.";
			}

			if (blnOkayToProcessTextFile)
			{
				foreach (string st in strLines)
				{
					if (!st.StartsWith("//") && st.Trim() != "" && st.Contains("="))
					{
						string[] s = st.Trim().Split('=');
						if (s.Length > 2)
						{
							for (int i = 2; i < s.Length; i++)
							s[1] += "=" + s[i];
						}
						switch (s[0].Trim().ToUpper())
						{
							// APPLICATION MANAGER SETTINGS
							case "CLASSIFICATION":
								AppClassification = (Classifications)Util.ConvertToInt(s[1].Trim());
								break;
							case "CLASSIFICATION_HEADING":
								CLASSIFICATION_HEADING	= s[1].Trim();
								break;
							case "MAX_FPS":
							case "MAXFPS":
								MaxFPS = Util.ConvertToInt(s[1].Trim());
								break;

								// GAME MANAGER SETTINGS
							case "DEFAULT_WIDTH":
								DEFAULT_WIDTH		= Util.ConvertToFloat(s[1].Trim());
								intWidth				= (int) DEFAULT_WIDTH;
								break;
							case "DEFAULT_HEIGHT":
								DEFAULT_HEIGHT	= Util.ConvertToFloat(s[1].Trim());
								intHeight				= (int) DEFAULT_HEIGHT;
								break;
							case "DEFAULT_APP_TIMEOUT":
								DEFAULT_APP_TIMEOUT = Util.ConvertToInt(s[1].Trim());
								break;
						}
					}
				}
			}

			// UPDATE BUTTON
			Status.UpdateStatus();
      if (intWidth > 0 && intHeight > 0)
			{
				Screen.fullScreen = false;
				Screen.SetResolution(intWidth, intHeight, false);
			}
		}
		
	#endregion

	#region "PUBLIC FUNCTIONS"

		public	string				GetWindowsUsername()
		{
			string strUsername = "";
			strUsername = System.Environment.UserName;
			return strUsername;
		} 
		public	void					LogOff()
		{
			#if USES_NETWORKMANAGER
			if (Net != null && Net.IsConnected)
			{
				if (Net.IsHost || Net.ForceHostMode)
					Net.HostDisconnect();
				else if (Net.IsClient)
					Net.ClientDisconnect();
				else if (Net.IsServer)
					Net.ServerDisconnect();
				PanelManager.Instance.ShowConnectPanel();
			}
			#else
			QuitApplication();
			#endif
		}
		public	void					QuitApplication()
		{
			PrivateQuitApplication();
		}
		public	IEnumerator		DelayedQuitApplication(float fDelay = 1.0f)
		{
			yield return new WaitForSeconds(fDelay);
			Status.Status = "Shutting the Server Down...";
			PrivateQuitApplication();
		}
		public	void					AddToDebugLog(string strMsg)
		{
			if (strMsg.Trim() != "")
			{
				_strDebugLog += "[" + System.DateTime.Now.ToString("MM/dd HH:mm:ss") + "] " + strMsg + "\r\n";
				#if IS_DEBUGGING
//				Debug.Log(strMsg);
				#endif
			}
		}
		public	void					WriteDebugLog()
		{
			if (_strDebugLog.Trim() != "")
			{
				string strFilename = "DebugLog_" + System.DateTime.Now.ToString("MMMdd-HHmmss") + ".txt";
				Status.Status = "Writing Results to Log File (" + strFilename + ")";
				Util.WriteTextFile("Debug Logs", strFilename, _strDebugLog);
				_strDebugLog = "";
			}
		}
		public	void					DebugLogButtonVisibility(int intShow = -1)
		{
			if (WriteErrorLogButton != null)
					WriteErrorLogButton.gameObject.SetActive( (intShow != 0) && (GameBuildType < ((int)ApplicationManager.DevPhases.Release) ));
		}
		public	void					ResetScreenSize()
		{
			Screen.fullScreen = false;
			Screen.SetResolution((int)DEFAULT_WIDTH, (int)DEFAULT_HEIGHT, false);
		}

	#endregion

	#region "START FUNCTION"

		private void						Awake()
		{
			_instance = this;
			#if USES_STATUSMANAGER
			_stm = StatusManager.Instance;
			#endif
			#if USES_DATABASEMANAGER
			_dbm = DatabaseManager.Instance;
			#endif
			#if USES_NETWORKMANAGER
			_nwm = AppNetworkManager.Instance;
			#endif

			// LOAD APPLICATION SETTINGS
			LoadApplicationSettings();

			if (MaxFPS > 0)
				Application.targetFrameRate = MaxFPS;
		}
		private void						Start()
		{
			// DISPLAY A COVER SCREEN
			if (PanelManager.Instance != null)	
					PanelManager.Instance.ShowLoadingPanel();

			// CHECK IF WE ARE CONNECTED TO A NETWORK
			#if !UNITY_WEBPLAYER
				if (!Application.isWebPlayer)
				{
					if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
					{
						#if USES_STATUSMANAGER
						Status.Status = "No Network Available";
						#else
						Debug.LogWarning("No Network Available");
						#endif
						return;
					}
				}
			#endif

			// ACTIVATE CONNECTION
			#if USES_NETWORKMANAGER
			if (Net != null && !Net.ForceOffline)
			{
//			if (Net.UsesMatchMaking)
//				Net.StartMatchMaking();
//			else 
				if (Net.IsServer || Net.IsHost)
					StartCoroutine(DoServerStart());
				else
					PanelManager.Instance.ShowConnectPanel();
			}
			#endif
		}
		public	IEnumerator			DoServerStart()
		{
			yield return new WaitForSeconds(0.2f);

			#if USES_DATABASEMANAGER
			// WAIT FOR DATABASE TO BE CONNECTED
			if (Database != null)
			{
				int i = 0;
				while (!Database.IsConnected && i < 1000)
				{
					yield return null;
					i++;
				}
			}
			#endif

			// START SERVER
			#if USES_NETWORKMANAGER
			if (Net != null)
			{
				if (Net.ServerAlsoPlays)
				{
//				if (Net.UsesMatchMaking)
//					Net.StartMatchHost(this.GameName, "");		// NO PASSWORD
//				else
						Net.HostStart();
				} else	
					Net.ServerStart();
			}
			#endif

			yield return null;
		}

	#endregion

}
