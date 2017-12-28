/******************************************************************************
*  @file       BulletHole.cs
*  @brief      Handles a bullet hole instance in the Blood MiniGame
*  @author     Ron
*  @date       July 31, 2015
*      
*  @par [explanation]
*		> 
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TouchScript;

#endregion // Namespaces

public class BulletHole : MonoBehaviour
{
	#region Public Interface

	/// <summary>
	/// Initializes this instance.
	/// </summary>
	/// <param name="sceneMaster">Scene master handling this instance.</param>
	/// <param name="bloodDropResource">Resource for spawning blood drop objects.</param>
	/// <param name="bloodDropVelocity">Blood drop velocity on spawn.</param>
	/// <param name="bloodDropGravity">Blood drop vertical acceleration.</param>
	/// <param name="deleteY">Vertical level below which blood drops would be deleted.</param>
	public void Initialize(BloodMGSceneMaster sceneMaster, GameObject bloodDropResource, Vector2 bloodDropVelocity,
	                       float bloodDropGravity, float deleteY)
	{
		m_sceneMaster = sceneMaster;
		m_bloodDropResource = bloodDropResource;
		m_bloodDropVelocity = bloodDropVelocity;
		m_bloodDropGravity = bloodDropGravity;
		m_deleteY = deleteY;

		InitializePlugAnimation();
	}

	/// <summary>
	/// Plugs this bullet hole.
	/// </summary>
	public void Plug()
	{
		if (!m_isPlugged)
		{
			m_isPlugged = true;
			m_plugAnimator.AnimateToState2(false);
			// Show check sprite
			m_checkSprite.gameObject.SetActive(true);
			// Play squelch sound
			Locator.GetSoundSystem().PlayOneShot(SoundInfo.SFXID.BLOOD_SQUELCH);
		}
	}

	/// <summary>
	/// Unplugs this bullet hole.
	/// </summary>
	public void Unplug()
	{
		if (m_isPlugged)
		{
			m_isPlugged = false;
			m_plugAnimator.AnimateToState1(false);
			// Hide check sprite
			m_checkSprite.gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Makes this bullet hole start spawning blood drops.
	/// </summary>
	public void StartLeaking()
	{
		Show();
		m_startedLeaking = true;
	}

	/// <summary>
	/// Shows this bullet hole.
	/// </summary>
	public void Show()
	{
		m_spriteRenderer.enabled = true;
	}

	/// <summary>
	/// Hides this bullet hole.
	/// </summary>
	public void Hide()
	{
		m_spriteRenderer.enabled = false;
	}

	/// <summary>
	/// Pauses this instance.
	/// </summary>
	public void Pause()
	{
		m_isPaused = true;
		foreach (BloodDrop bloodDrop in m_bloodDropList)
		{
			bloodDrop.Pause();
		}
	}

	/// <summary>
	/// Unpauses this instance.
	/// </summary>
	public void Unpause()
	{
		m_isPaused = false;
		foreach (BloodDrop bloodDrop in m_bloodDropList)
		{
			bloodDrop.Unpause();
		}
	}

	/// <summary>
	/// Makes this bullet hole ignore input, keeping it plugged or unplugged regardless of touch input.
	/// </summary>
	public void IgnoreInput()
	{
		m_ignoreInput = true;
	}

	/// <summary>
	/// Notifies of a blood drop deletion.
	/// </summary>
	/// <param name="deletedBloodDrop">Deleted blood drop.</param>
	public void NotifyBloodDropDeletion(BloodDrop deletedBloodDrop)
	{
		m_bloodDropList.Remove(deletedBloodDrop);
	}

	/// <summary>
	/// Deletes this instance.
	/// </summary>
	public void Delete()
	{
		// Delete blood drops
		for (int index = m_bloodDropList.Count - 1; index >= 0; --index)
		{
			m_bloodDropList[index].Delete(true);
		}
		m_bloodDropList.Clear();

		GameObject.Destroy(this.gameObject);
	}

	/// <summary>
	/// Gets whether this bullet hole has started leaking.
	/// </summary>
	public bool StartedLeaking
	{
		get { return m_startedLeaking; }
	}

	/// <summary>
	/// Gets whether this bullet hole is plugged.
	/// </summary>
	public bool IsPlugged
	{
		get { return m_isPlugged; }
	}

	#endregion // Public Interface

	#region Serialized Variables

	[SerializeField] private SpriteRenderer m_spriteRenderer	= null;
	[SerializeField] private SpriteRenderer m_checkSprite		= null;

	// Interval between spawning blood drops
	[SerializeField] private float 	m_bloodDropSpawnInterval 	= 0.3f;

	// Bullet hole scale when not plugged
	[SerializeField] private float 	m_unpluggedScale 			= 1.0f;
	// Bullet hole scale when plugged
	[SerializeField] private float 	m_pluggedScale				= 0.8f;
	// Speed at which hole scales from unplugged to plugged, and vice versa
	[SerializeField] private float 	m_plugAnimSpeed				= 8.0f;

	#endregion // Serialized Variables

	#region Variables

	private	BloodMGSceneMaster m_sceneMaster = null;
	
	private bool 	m_isPaused 	= false;

	#endregion // Variables

	#region Blood Drop Spawning

	private List<BloodDrop>	m_bloodDropList	= new List<BloodDrop>();

	private	GameObject m_bloodDropResource	= null;

	private Vector2 m_bloodDropVelocity	= Vector2.zero;
	// Blood drop vertical acceleration
	private float 	m_bloodDropGravity	= 9.8f;

	private bool	m_isPlugged			= false;
	private bool	m_startedLeaking	= false;
	private float	m_deleteY			= 0.0f;

	// Set to true when game is over
	private bool	m_ignoreInput		= false;

	private float	m_timeSinceLastBloodDropSpawn = 0.0f;

	/// <summary>
	/// Updates the blood drop spawning.
	/// </summary>
	private void UpdateBloodDropSpawning()
	{
		if (!m_startedLeaking || m_isPlugged)
		{
			return;
		}

		m_timeSinceLastBloodDropSpawn += Time.deltaTime;
		if (m_timeSinceLastBloodDropSpawn > m_bloodDropSpawnInterval)
		{
			m_timeSinceLastBloodDropSpawn = 0.0f;

			BloodDrop bloodDrop = SpawnBloodDrop(this.transform.position);
			bloodDrop.Initialize(this, m_bloodDropGravity, m_bloodDropVelocity, m_deleteY);
		}
	}

	/// <summary>
	/// Spawns a blood drop.
	/// </summary>
	/// <returns>The blood drop.</returns>
	private BloodDrop SpawnBloodDrop(Vector3 position)
	{
		GameObject bloodDropObj = GameObject.Instantiate(m_bloodDropResource) as GameObject;
		bloodDropObj.transform.parent = m_sceneMaster.DynamicObjects;
		bloodDropObj.transform.position = position;

		BloodDrop bloodDrop = bloodDropObj.AddComponentNoDupe<BloodDrop>();
		m_bloodDropList.Add(bloodDrop);
		return bloodDrop;
	}

	#endregion // Blood Drop Spawning

	#region Input

	/// <summary>
	/// Checks if a touch is plugging this bullet hole.
	/// </summary>
	/// <returns><c>true</c>, if plugged, <c>false</c> otherwise.</returns>
	private bool UpdatePlugState()
	{
		// If ignoreInput is set, the isPlugged state will no longer change
		if (m_ignoreInput)
		{
			return m_isPlugged;
		}
		foreach (ITouch touch in TouchManager.Instance.ActiveTouches)
		{
			RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(touch.Position), Vector2.zero);
			foreach (RaycastHit2D hit in hits)
			{
				if (hit.transform == this.transform)
				{
					Plug();
					return true;
				}
			}
		}
		Unplug();
		return false;
	}

	#endregion // Input

	#region Animation

	private UIAnimator 	m_plugAnimator 		= null;

	/// <summary>
	/// Initializes the plug animation.
	/// </summary>
	private void InitializePlugAnimation()
	{
		m_plugAnimator = new UIAnimator(m_spriteRenderer.transform);
		m_plugAnimator.SetScaleAnimation(Vector3.one * m_unpluggedScale, Vector3.one * m_pluggedScale);
		m_plugAnimator.SetAnimSpeed(m_plugAnimSpeed);
	}

	#endregion // Animation

	#region MonoBehaviour

	/// <summary>
	/// Awake this instance.
	/// </summary>
	private void Awake()
	{

	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start()
	{
		
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update()
	{
		if (m_isPaused || !m_startedLeaking)
		{
			return;
		}

		UpdateBloodDropSpawning();

		UpdatePlugState();

		m_plugAnimator.Update(Time.deltaTime);
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{

	}

	#endregion // MonoBehaviour
}
