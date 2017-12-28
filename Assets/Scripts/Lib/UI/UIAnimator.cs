/******************************************************************************
*  @file       UIAnimator.cs
*  @brief      Handles simple two-state animations for UI
*  @author     Ron
*  @date       July 29, 2015
*      
*  @par [explanation]
*		> Provides two kinds of animators: Transform, and Color
*		 	Transform animator allows animation of position, rotation, and scale
*		 	Color animator allows animation of color and transparency
*		> How to use:
*			1. Create a UIAnimator instance by calling one of two constructors,
*				and passing in the object you need animated (either Transform
*				or Renderer)
*			2. Set values for the two animation states, e.g. 2 position values
*				if you're animating position
*			3. Set values for the animation speed (in speed, or time to animate)
*			4. Start the animation by calling either AnimateToState1 or
*				AnimateToState2
*		> NOTE: SpriteRenderer.color is different from Renderer.material.color.
*			Sprite objects ignore the value of Renderer.material.color. The
*			Color UIAnimator animates both of these different color fields.
******************************************************************************/

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

public class UIAnimator
{
	#region Public Interface

	/// <summary>
	/// Initializes a Transform animator.
	/// </summary>
	public UIAnimator(Transform transformToAnimate) :
		this(transformToAnimate, UIAnimatorType.TRANSFORM, true, true)
	{
		// Empty
	}

	/// <summary>
	/// Initializes a Color animator.
	/// </summary>
	public UIAnimator(Renderer rendererToAnimate, bool includeChildren = true, bool includeInactive = true) :
		this(rendererToAnimate.transform, UIAnimatorType.COLOR, includeChildren, includeInactive)
	{
		// Empty
	}

	/// <summary>
	/// Initializes a Transform or Color animator.
	/// </summary>
	public UIAnimator(Transform transformToAnimate, UIAnimatorType animatorType, bool includeChildren, bool includeInactive)
	{
		m_uiAnimatorType = animatorType;
		switch (animatorType)
		{
		case UIAnimatorType.TRANSFORM:
		default:
			m_transform = transformToAnimate;
			break;
		case UIAnimatorType.COLOR:
			m_spriteRenderers = includeChildren ? 
								transformToAnimate.GetComponentsInChildren<SpriteRenderer>(includeInactive) :
								new SpriteRenderer[]{ transformToAnimate.GetComponent<SpriteRenderer>() };
			m_renderers = includeChildren ?
						  transformToAnimate.GetComponentsInChildren<Renderer>(includeInactive) :
						  new Renderer[]{ transformToAnimate.GetComponent<Renderer>() };
			// Check if there are renderers to animate
			if (m_spriteRenderers.Length == 0 && m_renderers.Length == 0)
			{
				Debug.LogWarning("No sprite renderer(s) found");
			}
			break;
		}
		SetStartState(UIAnimationState.STATE1);
	}

	/// <summary>
	/// Sets the state the animation should start in.
	/// </summary>
	/// <param name="startState">Start state.</param>
	public void SetStartState(UIAnimationState startState)
	{
		m_uiAnimState = startState;
	}

	/// <summary>
	/// Starts animation from state 1 to state 2.
	/// </summary>
	public void AnimateToState2(bool resetToState1 = true)
	{
		if (resetToState1)
		{
			//ResetToState(UIAnimationState.STATE1);
			m_timeSinceAnimStart = 0.0f;
		}
		m_uiAnimState = UIAnimationState.TO_STATE2;
	}

	/// <summary>
	/// Starts animation from state 2 to state 1.
	/// </summary>
	public void AnimateToState1(bool resetToState2 = true)
	{
		if (resetToState2)
		{
			//ResetToState(UIAnimationState.STATE2);
			m_timeSinceAnimStart = 0.0f;
		}
		m_uiAnimState = UIAnimationState.TO_STATE1;
	}

	/// <summary>
	/// Resets the animation to the specified state.
	/// </summary>
	public void ResetToState(UIAnimationState state)
	{
		m_uiAnimState = state;
		switch (m_uiAnimatorType)
		{
		case UIAnimatorType.TRANSFORM:
		default:
			if (m_animatePosition)
			{
				m_transform.position = (state == UIAnimationState.STATE1) ?
									   m_animState1_position :
									   m_animState2_position;
			}
			if (m_animateRotation)
			{
				m_transform.eulerAngles = (state == UIAnimationState.STATE1) ?
										  m_animState1_rotation :
										  m_animState2_rotation;
			}
			if (m_animateScale)
			{
				m_transform.localScale = (state == UIAnimationState.STATE1) ?
										 m_animState1_scale :
										 m_animState2_scale;
			}
			break;
		case UIAnimatorType.COLOR:
			if (m_animateAlpha)
			{
				foreach (SpriteRenderer spriteRenderer in m_spriteRenderers)
				{
					spriteRenderer.SetAlpha((state == UIAnimationState.STATE1) ?
											m_animState1_alpha :
											m_animState2_alpha);
				}
				foreach (Renderer renderer in m_renderers)
				{
					renderer.SetAlpha((state == UIAnimationState.STATE1) ?
					                  m_animState1_alpha :
					                  m_animState2_alpha);
				}
			}
			if (m_animateColor)
			{
				foreach (SpriteRenderer spriteRenderer in m_spriteRenderers)
				{
					spriteRenderer.color = (state == UIAnimationState.STATE1) ?
										   m_animState1_color :
										   m_animState2_color;
				}
				foreach (Renderer renderer in m_renderers)
				{
					renderer.material.color = (state == UIAnimationState.STATE1) ?
											  m_animState1_color :
											  m_animState2_color;
				}
			}
			break;
		}
	}

	/// <summary>
	/// Updates the UI animation.
	/// </summary>
	/// <param name="deltaTime">Delta time.</param>
	public void Update(float deltaTime)
	{
		UpdateUIAnimation(deltaTime);
	}

	/// <summary>
	/// Sets values for animating the position.
	/// </summary>
	/// <param name="positionInState1">Position in state 1.</param>
	/// <param name="positionInState2">Position in state 2.</param>
	public void SetPositionAnimation(Vector3 positionInState1, Vector3 positionInState2)
	{
		if (m_uiAnimatorType != UIAnimatorType.TRANSFORM)
		{
			if (BuildInfo.IsDebugMode)
			{
				Debug.LogWarning("Position animations can only be set in Transform animators");
			}
			return;
		}
		m_animatePosition = true;
		m_animState1_position = positionInState1;
		m_animState2_position = positionInState2;
	}

	/// <summary>
	/// Sets values for animating the rotation in euler angles.
	/// </summary>
	/// <param name="rotationInState1">Rotation in state 1.</param>
	/// <param name="rotationInState2">Rotation in state 2.</param>
	public void SetRotationAnimation(Vector3 rotationInState1, Vector3 rotationInState2)
	{
		if (m_uiAnimatorType != UIAnimatorType.TRANSFORM)
		{
			if (BuildInfo.IsDebugMode)
			{
				Debug.LogWarning("Rotation animations can only be set in Transform animators");
			}
			return;
		}
		m_animateRotation = true;
		m_animState1_rotation = rotationInState1;
		m_animState2_rotation = rotationInState2;
	}

	/// <summary>
	/// Sets values for animating the local scale.
	/// </summary>
	/// <param name="scaleInState1">Scale in state 1.</param>
	/// <param name="scaleInState2">Scale in state 2.</param>
	public void SetScaleAnimation(Vector3 scaleInState1, Vector3 scaleInState2)
	{
		if (m_uiAnimatorType != UIAnimatorType.TRANSFORM)
		{
			if (BuildInfo.IsDebugMode)
			{
				Debug.LogWarning("Scale animations can only be set in Transform animators");
			}
			return;
		}
		m_animateScale = true;
		m_animState1_scale = scaleInState1;
		m_animState2_scale = scaleInState2;
	}

	/// <summary>
	/// Sets values for animating the SpriteRenderer's transparency.
	/// </summary>
	/// <param name="alphaInState1">Alpha in state 1.</param>
	/// <param name="alphaInState2">Alpha in state 2.</param>
	public void SetAlphaAnimation(float alphaInState1, float alphaInState2)
	{
		if (m_uiAnimatorType != UIAnimatorType.COLOR)
		{
			if (BuildInfo.IsDebugMode)
			{
				Debug.LogWarning("Alpha animations can only be set in Color animators");
			}
			return;
		}
		m_animateAlpha = true;
		m_animState1_alpha = alphaInState1;
		m_animState2_alpha = alphaInState2;
	}

	/// <summary>
	/// Sets values for animating the SpriteRenderer's color.
	/// </summary>
	/// <param name="alphaInState1">Color in state 1.</param>
	/// <param name="alphaInState2">Color in state 2.</param>
	public void SetColorAnimation(Color colorInState1, Color colorInState2)
	{
		if (m_uiAnimatorType != UIAnimatorType.COLOR)
		{
			if (BuildInfo.IsDebugMode)
			{
				Debug.LogWarning("Color animations can only be set in Color animators");
			}
			return;
		}
		m_animateColor = true;
		m_animState1_color = colorInState1;
		m_animState2_color = colorInState2;
	}

	/// <summary>
	/// Sets the animation speed.
	/// Overrides any previous SetAnimTime calls.
	/// </summary>
	/// <param name="animSpeed">Animation speed.</param>
	public void SetAnimSpeed(float animSpeed)
	{
		m_useAnimSpeed = true;
		m_animSpeed = animSpeed;
		if (animSpeed != 0.0f)
		{
			m_animTime = 1 / animSpeed;
		}
		// If animSpeed is 0, animation stays in current state
	}

	/// <summary>
	/// Sets the time the animation should take to get from state to state.
	/// Overrides any previous SetAnimSpeed calls.
	/// </summary>
	/// <param name="animTime">Animation time.</param>
	public void SetAnimTime(float animTime)
	{
		m_useAnimSpeed = false;
		m_animTime = animTime;
		if (animTime != 0.0f)
		{
			m_animSpeed = 1 / animTime;
		}
		// If animation time is 0, animation will switch immediately to the target state
	}

	/// <summary>
	/// Removes all animations.
	/// </summary>
	public void RemoveAllAnimations()
	{
		RemovePositionAnimation();
		RemoveRotationAnimation();
		RemoveScaleAnimation();
		RemoveAlphaAnimation();
		RemoveColorAnimation();
		m_animSpeed = 0.0f;
		m_animTime = 0.0f;
	}

	/// <summary>
	/// Removes the position animation.
	/// </summary>
	public void RemovePositionAnimation()
	{
		m_animatePosition = false;
	}

	/// <summary>
	/// Removes the rotation animation.
	/// </summary>
	public void RemoveRotationAnimation()
	{
		m_animateRotation = false;
	}

	/// <summary>
	/// Removes the scale animation.
	/// </summary>
	public void RemoveScaleAnimation()
	{
		m_animateScale = false;
	}

	/// <summary>
	/// Removes the color animation.
	/// </summary>
	public void RemoveColorAnimation()
	{
		m_animateColor = false;
	}

	/// <summary>
	/// Removes the alpha animation.
	/// </summary>
	public void RemoveAlphaAnimation()
	{
		m_animateAlpha = false;
	}

	/// <summary>
	/// Pauses the UI animation.
	/// </summary>
	public void Pause()
	{
		m_uiAnimStateBeforePause = m_uiAnimState;

	}

	/// <summary>
	/// Unpauses the UI animation.
	/// </summary>
	public void Unpause()
	{
		m_uiAnimState = m_uiAnimStateBeforePause;
	}

	/// <summary>
	/// Gets the UI animation state.
	/// </summary>
	public UIAnimationState UIAnimState
	{
		get { return m_uiAnimState; }
	}

	/// <summary>
	/// Gets whether animator has any active animations.
	/// </summary>
	public bool IsActive
	{
		get { return m_animatePosition ||
					 m_animateRotation ||
					 m_animateScale ||
					 m_animateAlpha ||
					 m_animateColor; }
	}

	/// <summary>
	/// Gets whether this UI is currently animating.
	/// </summary>
	public bool IsAnimating
	{
		get { return m_uiAnimState == UIAnimationState.TO_STATE1 ||
					 m_uiAnimState == UIAnimationState.TO_STATE2; }
	}

	/// <summary>
	/// Gets whether animation is in State 1.
	/// </summary>
	public bool IsInState1
	{
		get { return m_uiAnimState == UIAnimationState.STATE1; }
	}

	/// <summary>
	/// Gets whether animation is in State 2.
	/// </summary>
	public bool IsInState2
	{
		get { return m_uiAnimState == UIAnimationState.STATE2; }
	}

	#endregion // Public Interface

	#region UI Animator Type

	public enum UIAnimatorType
	{
		TRANSFORM,
		COLOR
	}
	private UIAnimatorType m_uiAnimatorType = UIAnimatorType.TRANSFORM;

	#endregion // UI Animator Type

	#region Animations

	// Transform
	private Transform	m_transform				= null;
	// Position (world position)
	private bool		m_animatePosition		= false;
	private Vector3 	m_animState1_position	= Vector3.zero;
	private Vector3 	m_animState2_position	= Vector3.zero;
	// Rotation (euler angles)
	private bool		m_animateRotation		= false;
	private Vector3 	m_animState1_rotation 	= Vector3.zero;
	private Vector3 	m_animState2_rotation 	= Vector3.zero;
	// Scale (local scale)
	private bool		m_animateScale			= false;
	private Vector3 	m_animState1_scale		= Vector3.zero;
	private Vector3 	m_animState2_scale		= Vector3.zero;

	// Renderer
	private	SpriteRenderer[] m_spriteRenderers	= null;	// Used specifically for sprites
	private Renderer[]	m_renderers				= null;
	// Alpha
	private bool		m_animateAlpha			= false;
	private float 		m_animState1_alpha		= 0.0f;
	private float 		m_animState2_alpha		= 0.0f;
	// Color
	private bool		m_animateColor 			= false;
	private Color		m_animState1_color 		= Color.white;
	private Color		m_animState2_color 		= Color.white;

	private float 		m_timeSinceAnimStart 	= 0.0f;
	private bool		m_useAnimSpeed			= false;
	private float		m_animSpeed				= 5.0f;
	private float		m_animTime				= 0.5f;

	public enum UIAnimationState
	{
		NONE,
		TO_STATE1,
		STATE1,
		TO_STATE2,
		STATE2
	}
	private UIAnimationState m_uiAnimState = UIAnimationState.NONE;

	private UIAnimationState m_uiAnimStateBeforePause = UIAnimationState.NONE;

	/// <summary>
	/// Updates the pledge popup animation.
	/// </summary>
	private void UpdateUIAnimation(float deltaTime)
	{
		if (!IsActive || (m_useAnimSpeed && m_animSpeed == 0.0f))
		{
			return;
		}

		switch (m_uiAnimState)
		{
		case UIAnimationState.TO_STATE1:
			// Animate from state 2 to state 1
			m_timeSinceAnimStart += deltaTime;
			if (m_animatePosition)
			{
				m_transform.position = Vector3.Lerp(m_animState2_position, m_animState1_position,
				                                    m_timeSinceAnimStart * m_animSpeed);
			}
			if (m_animateRotation)
			{
				m_transform.eulerAngles = Vector3.Lerp(m_animState2_rotation, m_animState1_rotation,
				                                       m_timeSinceAnimStart * m_animSpeed);
			}
			if (m_animateScale)
			{
				m_transform.localScale = Vector3.Lerp(m_animState2_scale, m_animState1_scale,
				                                       m_timeSinceAnimStart * m_animSpeed);
			}
			if (m_animateAlpha)
			{
				foreach (SpriteRenderer spriteRenderer in m_spriteRenderers)
				{
					spriteRenderer.SetAlpha(Mathf.Lerp(m_animState2_alpha, m_animState1_alpha,
				                                 	   m_timeSinceAnimStart * m_animSpeed));
				}
				foreach (Renderer renderer in m_renderers)
				{
					renderer.SetAlpha(Mathf.Lerp(m_animState2_alpha, m_animState1_alpha,
					                             m_timeSinceAnimStart * m_animSpeed));
				}
			}
			if (m_animateColor)
			{
				foreach (SpriteRenderer spriteRenderer in m_spriteRenderers)
				{
					spriteRenderer.color = Color.Lerp(m_animState2_color, m_animState1_color,
					                            	  m_timeSinceAnimStart * m_animSpeed);
				}
				foreach (Renderer renderer in m_renderers)
				{
					renderer.material.color = Color.Lerp(m_animState2_color, m_animState1_color,
					                          			 m_timeSinceAnimStart * m_animSpeed);
				}
			}
			// Check if animation is done
			if (m_timeSinceAnimStart > m_animTime)
			{
				m_timeSinceAnimStart = 0.0f;
				m_uiAnimState = UIAnimationState.STATE1;
			}
			break;
		case UIAnimationState.STATE1:
			// Now in animation state 1
			break;

		case UIAnimationState.TO_STATE2:
			// Animate from state 1 to state 2
			m_timeSinceAnimStart += deltaTime;
			if (m_animatePosition)
			{
				m_transform.position = Vector3.Lerp(m_animState1_position, m_animState2_position,
				                                    m_timeSinceAnimStart * m_animSpeed);
			}
			if (m_animateRotation)
			{
				m_transform.eulerAngles = Vector3.Lerp(m_animState1_rotation, m_animState2_rotation,
				                                       m_timeSinceAnimStart * m_animSpeed);
			}
			if (m_animateScale)
			{
				m_transform.localScale = Vector3.Lerp(m_animState1_scale, m_animState2_scale,
				                                      m_timeSinceAnimStart * m_animSpeed);
			}
			if (m_animateAlpha)
			{
				foreach (SpriteRenderer spriteRenderer in m_spriteRenderers)
				{
					spriteRenderer.SetAlpha(Mathf.Lerp(m_animState1_alpha, m_animState2_alpha,
					                             	   m_timeSinceAnimStart * m_animSpeed));
				}
				foreach (Renderer renderer in m_renderers)
				{
					renderer.SetAlpha(Mathf.Lerp(m_animState1_alpha, m_animState2_alpha,
					                             m_timeSinceAnimStart * m_animSpeed));
				}
			}
			if (m_animateColor)
			{
				foreach (SpriteRenderer spriteRenderer in m_spriteRenderers)
				{
					spriteRenderer.color = Color.Lerp(m_animState1_color, m_animState2_color,
					                            	  m_timeSinceAnimStart * m_animSpeed);
				}
				foreach (Renderer renderer in m_renderers)
				{
					renderer.material.color = Color.Lerp(m_animState1_color, m_animState2_color,
					                                     m_timeSinceAnimStart * m_animSpeed);
				}
			}
			// Check if animation is done
			if (m_timeSinceAnimStart > m_animTime)
			{
				m_timeSinceAnimStart = 0.0f;
				m_uiAnimState = UIAnimationState.STATE2;
			}
			break;
		case UIAnimationState.STATE2:
		default:
			// Now in animation state 2
			break;
		}
	}

	#endregion // Animations
}
