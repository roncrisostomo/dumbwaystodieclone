using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

namespace Soomla.Gifting
{
	/// <summary>
	/// Represents a manager class which is in charge of sending and receiving gifts
	/// between users playing the same game.
	/// </summary>
	public class SoomlaGifting {
		#if UNITY_IOS  && !UNITY_EDITOR
		[DllImport ("__Internal")]
		private static extern int soomlaGifting_initialize(bool autoGiftHandout);
		[DllImport ("__Internal")]
		private static extern int soomlaGifting_sendGift(int toProvider, string toProfileId, string itemId, int amount, bool deductFromUser, out bool outResult);
		[DllImport ("__Internal")]
		private static extern int soomlaGifting_handoutGifts(string giftsJsonArrayStr);
		#endif

		/// <summary>
		/// Initializes the gifting manager
		/// </summary>
		public static void Initialize() {
			Initialize (true);
		}
		public static void Initialize(bool autoGiftHandout) {
			SoomlaUtils.LogDebug (TAG, "SOOMLA/UNITY Initializing Gifting");
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaSyncClass = new AndroidJavaClass("com.soomla.gifting.SoomlaGifting")) {

				AndroidJavaObject jniSoomlaSyncInstance = jniSoomlaSyncClass.CallStatic<AndroidJavaObject>("getInstance");
				jniSoomlaSyncInstance.Call("initialize", autoGiftHandout);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			#elif UNITY_IOS && !UNITY_EDITOR
			soomlaGifting_initialize(autoGiftHandout);
			#endif
		}

		public static void HandoutGifts(List<Gift> gifts) {
#if !UNITY_EDITOR

			if (gifts == null || gifts.Count == 0) {
				SoomlaUtils.LogError(TAG, "You tried to handout empty gifts list.");
				return;
			}

			JSONObject giftsJSONs = new JSONObject (JSONObject.Type.ARRAY);
			foreach (Gift gift in gifts) {
				giftsJSONs.Add(gift.toJSONObject());
			}
			string giftsJSONsStr = giftsJSONs.print ();

#if UNITY_ANDROID
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaSyncClass = new AndroidJavaClass("com.soomla.gifting.SoomlaGifting")) {
				
				AndroidJavaObject jniSoomlaSyncInstance = jniSoomlaSyncClass.CallStatic<AndroidJavaObject>("getInstance");
				jniSoomlaSyncInstance.Call("handoutGifts", giftsJSONsStr);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
#elif UNITY_IOS
			soomlaGifting_handoutGifts(giftsJSONsStr);
#endif

#endif
		}

		/// <summary>
		/// Sends a gift from the currently logged in user (with Profile) to
		/// the provided user.
		/// This method gives the gift from the game without affecting the player.
		/// </summary>
		/// <returns><c>false</c>, if the operation cannot be started, <c>true</c> otherwise.</returns>
		/// <param name="toProvider">The social provider ID of the user to send gift to.</param>
		/// <param name="toProfileId">The social provider user ID to send gift to.</param>
		/// <param name="itemId">The virtual item ID to give as a gift.</param>
		/// <param name="amount">The amount of virtual items to gift.</param>
		public static bool SendGift(int toProvider, string toProfileId, string itemId, int amount) {
			return SendGift(toProvider, toProfileId, itemId, amount, false);
		}

		/// <summary>
		/// Sends a gift from the currently logged in user (with Profile) to
		/// the provided user.
		/// </summary>
		/// <returns><c>false</c>, if the operation cannot be started, <c>true</c> otherwise.</returns>
		/// <param name="toProvider">The social provider ID of the user to send gift to.</param>
		/// <param name="toProfileId">The social provider user ID to send gift to.</param>
		/// <param name="itemId">The virtual item ID to give as a gift.</param>
		/// <param name="amount">The amount of virtual items to gift.</param>
		/// <param name="deductFromUser">Should the virtual items be deducted from the
		/// player upon sending the gift</param>
		public static bool SendGift(int toProvider, string toProfileId, string itemId, int amount, bool deductFromUser) {
			SoomlaUtils.LogDebug (TAG, "SOOMLA/UNITY Sending Gift");
			bool result = false;
			
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaSyncClass = new AndroidJavaClass("com.soomla.gifting.SoomlaGifting")) {

				AndroidJavaObject jniSoomlaSyncInstance = jniSoomlaSyncClass.CallStatic<AndroidJavaObject>("getInstance");
				result = jniSoomlaSyncInstance.Call<bool>("sendGift", toProvider, toProfileId, itemId, amount, deductFromUser);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);

			#elif UNITY_IOS && !UNITY_EDITOR
			soomlaGifting_sendGift(toProvider, toProfileId, itemId, amount, deductFromUser, out result);
			#endif

			return result;
		}

		private const string TAG = "SOOMLA SoomlaGifting";
	}
}
