/// Copyright (C) 2012-2014 Soomla Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///      http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.

using UnityEngine;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Soomla.Highway
{

	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif
	/// <summary>
	/// This class holds the store's configurations.
	/// </summary>
	public class HighwaySettings : ISoomlaSettings
	{

		#if UNITY_EDITOR
		static HighwaySettings instance = new HighwaySettings();
		static HighwaySettings()
		{
			SoomlaEditorScript.addSettings(instance);
		}

		GUIContent highwayGameKeyLabel = new GUIContent("Game Key [?]:", "The SOOMLA Highway game key for your game");
		GUIContent highwayEnvKeyLabel = new GUIContent("Env Key [?]:", "The SOOMLA Highway environment key for your game");
		GUIContent frameworkVersion = new GUIContent("Highway Version [?]", "The SOOMLA Framework Highway Module version. ");
		GUIContent buildVersion = new GUIContent("Highway Build [?]", "The SOOMLA Framework Highway Module build.");

		public void OnEnable() {
			// No enabling, leave empty and let StoreManifestTools do the work
		}

		public void OnModuleGUI() {
		}

		public void OnInfoGUI() {
			SoomlaEditorScript.SelectableLabelField(frameworkVersion, "1.3.13");
			SoomlaEditorScript.SelectableLabelField(buildVersion, "1");
			EditorGUILayout.Space();
		}

		public void OnSoomlaGUI() {
			///
			/// Create Highway Game key and Env key labels and text fields
			///
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(highwayGameKeyLabel,  GUILayout.Width(150), SoomlaEditorScript.FieldHeight);
			HighwayGameKey = EditorGUILayout.TextField(HighwayGameKey, SoomlaEditorScript.FieldHeight);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(highwayEnvKeyLabel,  GUILayout.Width(150), SoomlaEditorScript.FieldHeight);
			HighwayEnvKey = EditorGUILayout.TextField(HighwayEnvKey, SoomlaEditorScript.FieldHeight);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}
		#endif

		public static string HIGHWAY_GAME_KEY_DEFAULT_MESSAGE = "[YOUR GAME KEY]";

		public static string HighwayGameKey
		{
			get {
				string value;
				return SoomlaEditorScript.Instance.SoomlaSettings.TryGetValue("HighwayGameKey", out value) ? value : HIGHWAY_GAME_KEY_DEFAULT_MESSAGE;
			}
			set
			{
				string v;
				SoomlaEditorScript.Instance.SoomlaSettings.TryGetValue("HighwayGameKey", out v);
				if (v != value)
				{
					SoomlaEditorScript.Instance.setSettingsValue("HighwayGameKey",value);
					SoomlaEditorScript.DirtyEditor ();
				}
			}
		}

		public static string HIGHWAY_ENV_KEY_DEFAULT_MESSAGE = "[YOUR ENV KEY]";

		public static string HighwayEnvKey
		{
			get {
				string value;
				return SoomlaEditorScript.Instance.SoomlaSettings.TryGetValue("HighwayEnvKey", out value) ? value : HIGHWAY_ENV_KEY_DEFAULT_MESSAGE;
			}
			set
			{
				string v;
				SoomlaEditorScript.Instance.SoomlaSettings.TryGetValue("HighwayEnvKey", out v);
				if (v != value)
				{
					SoomlaEditorScript.Instance.setSettingsValue("HighwayEnvKey",value);
					SoomlaEditorScript.DirtyEditor ();
				}
			}
		}
	}
}
