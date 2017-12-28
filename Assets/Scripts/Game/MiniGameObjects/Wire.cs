/******************************************************************************
*  @file       Wire.cs
*  @brief      Handles a wire in the Wire MiniGame
*  @author     Ron
*  @date       August 9, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using TouchScript;
using TouchScript.Gestures;

#endregion // Namespaces

public class Wire : InteractiveObject
{
	#region Public Interface

	/// <summary>
	/// Initializes this wire.
	/// </summary>
	public void Initialize(WireMGSceneMaster sceneMaster, WireType wireType, Color color,
	                       GameObject connectGuideResource, GameObject electricBoltResource)
	{
		m_sceneMaster = sceneMaster;
		m_wireType = wireType;
		m_wireInsulation.color = color;
		m_connectGuideResource = connectGuideResource;
		m_electricBoltResource = electricBoltResource;

		// Subscribe to input events
		AddPressDelegate(OnWirePress);
		this.GetComponent<ReleaseGesture>().Released += OnWireRelease;
	}

	/// <summary>
	/// Notifies the pause.
	/// </summary>
	public override void NotifyPause()
	{
		m_isPaused = true;

		// Pause sound
		if (m_connectSound != null)
		{
			m_connectSound.Pause();
		}

		// Disable collider
		m_isColliderEnabledBeforePause = this.GetComponent<Collider2D>().enabled;
		this.GetComponent<Collider2D>().enabled = false;
	}

	/// <summary>
	/// Notifies the unpause.
	/// </summary>
	public override void NotifyUnpause()
	{
		m_isPaused = false;

		// Unpause sound
		if (m_connectSound != null)
		{
			m_connectSound.Unpause();
		}
		// If connect guide is no longer active on unpause, reset it
		if (!m_isGuideActive)
		{
			ResetConnectGuide();
		}
		// Re-enable collider if it was enabled before pause
		this.GetComponent<Collider2D>().enabled = m_isColliderEnabledBeforePause;
	}

	/// <summary>
	/// Notifies the stop game.
	/// </summary>
	public void NotifyStopGame()
	{
		// Behaviour on game end is similar to that when paused
		NotifyPause();
	}

	/// <summary>
	/// Sets this wire as connected.
	/// </summary>
	public void SetConnected()
	{
		if (m_isConnected)
		{
			return;
		}
		m_isConnected = true;
		
		// Hide copper part of wire
		m_wireCopper.enabled = false;
		
		// Disable collider
		m_collider2D.enabled = false;
		
		// Unsubscribe from input events
		RemovePressDelegate(OnWirePress);
		this.GetComponent<ReleaseGesture>().Released -= OnWireRelease;
	}

	/// <summary>
	/// Gets whether this wire is connected to its partner on the other side of the screen.
	/// </summary>
	public bool IsConnected
	{
		get { return m_isConnected; }
	}

	public enum WireGroup
	{
		TOP,
		BOTTOM
	}

	/// <summary>
	/// Gets this wire's group (whether this wire is at the top or bottom side of the screen).
	/// </summary>
	public WireGroup GetWireGroup()
	{
		return m_wireGroup;
	}

	public enum WireType
	{
		TYPE0,
		TYPE1,
		TYPE2,
		TYPE3
	}

	/// <summary>
	/// Gets this wire's type, used to determine a match with other wires.
	/// </summary>
	public WireType GetWireType()
	{
		return m_wireType;
	}

	/// <summary>
	/// Gets the index of this wire in the top or bottom group.
	/// </summary>
	public int GroupIndex
	{
		get { return m_groupIndex; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private SpriteRenderer m_wireInsulation 	= null;
	[SerializeField] private SpriteRenderer m_wireCopper	 	= null;
	[SerializeField] private WireGroup 		m_wireGroup 		= WireGroup.TOP;
	// Index of this wire in its wire group
	[SerializeField] private int			m_groupIndex		= 0;
	[SerializeField] private Transform		m_wireSourcePos 	= null;

	#endregion // Serialized Variables

	#region Variables

	private WireMGSceneMaster m_sceneMaster = null;

	private bool m_isPaused = false;

	private bool m_isColliderEnabledBeforePause = false;

	#endregion // Variables

	#region Wire

	private WireType 	m_wireType 		= WireType.TYPE0;
	private bool		m_isConnected	= false;
	private ITouch 		m_wireTouch 	= null;

	/// <summary>
	/// Checks if wire touch connects with another wire.
	/// </summary>
	private void CheckConnectWithWire()
	{
		if (m_wireTouch == null)
		{
			return;
		}
		// Check if touch that selected this wire connects with another wire on the opposite side of the screen
		RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(m_wireTouch.Position), Vector2.zero);
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.transform == null)
			{
				continue;
			}

			// Skip checking this wire
			if (hit.transform == this.transform)
			{
				continue;
			}

			Wire otherWire = hit.transform.GetComponent<Wire>();
			if (otherWire == null)
			{
				continue;
			}

			// If other wire is the same type as this wire, connect the two
			if (otherWire.GetWireType() == m_wireType)
			{
				SetConnected();
				otherWire.SetConnected();

				// Play plug-in sound
				Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.WIRE_PLUGIN);

				ResetConnectGuide();

				m_sceneMaster.NotifyWireConnect(this, otherWire);
			}
			// Else, game over
			else
			{
				// Play zap sound
				Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.WIRE_ZAP);

				// Replace connect guide with electric bolt
				ReplaceGuideWithBolt();

				m_sceneMaster.NotifyWireMismatch();
			}

			m_wireTouch = null;

			// Stop connect sound
			m_connectSound.Stop();

			break;
		}
	}

	#endregion // Wire

	#region Connect Guide

	private GameObject 	m_connectGuideResource 	= null;
	private Transform 	m_connectGuide 			= null;

	/// <summary>
	/// Creates a connect guide.
	/// </summary>
	/// <returns>The connect guide.</returns>
	private Transform CreateConnectGuide()
	{
		GameObject connectGuideObj = GameObject.Instantiate(m_connectGuideResource) as GameObject;
		connectGuideObj.SetActive(true);
		connectGuideObj.transform.parent = this.transform;
		return connectGuideObj.transform;
	}

	/// <summary>
	/// Maintains the connect guide connection between selected wire and touch.
	/// </summary>
	private void UpdateConnectGuide()
	{
		if (!m_isGuideActive || m_wireTouch == null)
		{
			return;
		}

		Vector3 destPos = Camera.main.ScreenToWorldPoint(m_wireTouch.Position);
		destPos.z = 0.0f;

		// Position connect guide at the center of source and destination
		m_connectGuide.position = (m_wireSourcePos.position + destPos) * 0.5f;
		// Scale connect guide to the distance between source and destination
		m_connectGuide.SetScaleX(Vector3.Distance(m_wireSourcePos.position, destPos));
		// Rotate connect guide to point from source to destination
		Vector3 sourceToDestDir = Vector3.Normalize(destPos - m_wireSourcePos.position);
		float angle = Mathf.Atan(sourceToDestDir.y / sourceToDestDir.x) * Mathf.Rad2Deg;
		m_connectGuide.SetRotZ(angle);
	}

	/// <summary>
	/// Resets the connect guide to hidden state.
	/// </summary>
	private void ResetConnectGuide()
	{
		if (m_connectGuide == null)
		{
			return;
		}
		m_connectGuide.position = m_wireSourcePos.position;
		m_connectGuide.eulerAngles = Vector3.zero;
		m_connectGuide.SetScaleX(0.0f);
	}

	#endregion // Connect Guide
	
	#region Electric Bolt
	
	private GameObject	m_electricBoltResource	= null;
	
	/// <summary>
	/// Replaces the connect guide with an electric bolt.
	/// </summary>
	/// <returns>The electric bolt.</returns>
	private Transform ReplaceGuideWithBolt()
	{
		GameObject electricBoltObj = GameObject.Instantiate(m_electricBoltResource) as GameObject;
		electricBoltObj.transform.parent = this.transform;
		// Copy connect guide's transform properties
		electricBoltObj.transform.position = m_connectGuide.position;
		electricBoltObj.transform.eulerAngles = m_connectGuide.eulerAngles;
		electricBoltObj.transform.localScale = m_connectGuide.localScale;
		// Enable electric bolt and disable connect guide
		electricBoltObj.SetActive(true);
		m_connectGuide.gameObject.SetActive(false);
		// Return the electric bolt transform
		return electricBoltObj.transform;
	}
	
	#endregion // Electric Bolt

	#region Input Handling
	
	private SoundObject m_connectSound 	= null;

	private bool		m_isGuideActive = false;

	/// <summary>
	/// Raises the wire press event.
	/// </summary>
	private void OnWirePress(object sender, System.EventArgs e)
	{
		// Process only one touch per wire
		if (m_isGuideActive)
		{
			return;
		}
		m_isGuideActive = true;
		
		// Lazy init
		if (m_connectGuide == null)
		{
			m_connectGuide = CreateConnectGuide();
		}

		// Play connect sound
		if (m_connectSound == null)
		{
			m_connectSound = Locator.GetSoundSystem().PlaySound(SoundInfo.SFXID.WIRE_CONNECT);
		}
		else
		{
			m_connectSound.Play();
		}

		// Find and store a reference to the touch
		foreach (ITouch touch in m_pressGesture.ActiveTouches)
		{
			if (m_pressGesture.HasTouch(touch))
			{
				m_wireTouch = touch;
				break;
			}
		}
	}

	/// <summary>
	/// Raises the wire release event.
	/// </summary>
	private void OnWireRelease(object sender, System.EventArgs e)
	{
		m_isGuideActive = false;

		// Stop connect sound
		m_connectSound.Stop();

		// When game is paused, just leave the connect guide shown, then hide it when the game is unpaused.
		if (!m_isPaused)
		{
			ResetConnectGuide();
		}
	}

	#endregion // Input Handling

	#region MonoBehaviour

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	protected override void Start()
	{
		base.Start();
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	protected override void Update()
	{
		base.Update();

		if (m_isPaused)
		{
			return;
		}

		UpdateConnectGuide();
		CheckConnectWithWire();
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	#endregion // MonoBehaviour
}
