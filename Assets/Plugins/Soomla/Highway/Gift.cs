using UnityEngine;
using System.Collections;

namespace Soomla.Gifting
{
	/// <summary>
	/// Represents a gift from one user to the other
	/// </summary>
	public class Gift {
		/// <summary>
		/// The Gift's ID, only when received from server
		/// </summary>
		public string ID = "";
		/// <summary>
		/// User's UID from which the gift originated
		/// </summary>
		public string FromUID = "";
		/// <summary>
		/// Social provider ID of the user to which the gift was sent
		/// </summary>
		public int ToProvider = 0;
		/// <summary>
		/// The receiving user's ID on the provider social provider
		/// </summary>
		public string ToProfileId = "";
		/// <summary>
		/// Payload for the gift
		/// </summary>
		public GiftPayload Payload = null;

		public JSONObject toJSONObject() {
			JSONObject jsonObject = new JSONObject ();
			jsonObject.AddField(GIFT_ID, ID);
			jsonObject.AddField(TO_PROVIDER, ToProvider);
			jsonObject.AddField(FROM_UID, FromUID);
			jsonObject.AddField(TO_PROFILE_ID, ToProfileId);
			if (Payload != null) {
				jsonObject.AddField(PAYLOAD, Payload.toJSONObject());
			}
			return jsonObject;
		}
		
		public Gift(JSONObject jsonObject) {
			if (jsonObject[GIFT_ID]) {
				ID = jsonObject[GIFT_ID].str;
			}

			if (jsonObject[FROM_UID]) {
				FromUID = jsonObject[FROM_UID].str;
			}

			if (jsonObject[TO_PROVIDER]) {
				ToProvider = System.Convert.ToInt32(jsonObject[TO_PROVIDER].n);
			}

			if (jsonObject[TO_PROFILE_ID]) {
				ToProfileId = jsonObject[TO_PROFILE_ID].str;
			}

			if (jsonObject[PAYLOAD]) {
				Payload = new GiftPayload(jsonObject[PAYLOAD]);
			}
		}

		private const string GIFT_ID = "giftId";
		private const string FROM_UID = "fromUid";
		private const string TO_PROVIDER = "toProvider";
		private const string TO_PROFILE_ID = "toProfileId";
		private const string PAYLOAD = "payload";
	}
}