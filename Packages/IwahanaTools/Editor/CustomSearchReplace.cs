using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.Animations;
using System.IO;
using System;

namespace CustomSearchReplace
{
    public class CustomSearchReplace : EditorWindow
	{
		[MenuItem ("Iwahana Tools/カスタムサーチリプレイス", false, 200)]
		static void CreateWindow ()
		{
            var window = GetWindow<CustomSearchReplace> ("CustomSearch");

			window.Search ();
		}

		void OnEnable ()
		{
			Search ();
		}

		public class FilterData
		{
			public string name;
			public bool isEnable;
		}

		List<FilterData> filterList = new List<FilterData> () {
			new FilterData (){ name = "AnimatorController", isEnable = false },
			new FilterData (){ name = "AnimationClip", isEnable = false },
			new FilterData (){ name = "AudioClip", isEnable = false },
			new FilterData (){ name = "AudioMixer", isEnable = false },
			new FilterData (){ name = "Font", isEnable = false },
			new FilterData (){ name = "GUISkin", isEnable = false },
			new FilterData (){ name = "Material", isEnable = false },
			new FilterData (){ name = "Mesh", isEnable = false },
			new FilterData (){ name = "Model", isEnable = false },
			new FilterData (){ name = "PhysicMaterial", isEnable = false },
			new FilterData (){ name = "Prefab", isEnable = false },
			new FilterData (){ name = "Scene", isEnable = false },
			new FilterData (){ name = "Script", isEnable = false },
			new FilterData (){ name = "ScriptableObject", isEnable = false },
			new FilterData (){ name = "Shader", isEnable = false },
			new FilterData (){ name = "Sprite", isEnable = false },
			new FilterData (){ name = "Texture", isEnable = false },
			new FilterData (){ name = "TimelineAsset", isEnable = false },
			new FilterData (){ name = "VideoClip", isEnable = false },
		};

		public class SearchAnimationClipInfo : IEquatable<SearchAnimationClipInfo>
		{
            public string renamePath { get; set; }
            public AnimationClip clip { get; set; }

			public override int GetHashCode ()
			{
				return this.clip.GetHashCode ();
			}

			bool IEquatable<SearchAnimationClipInfo>.Equals (SearchAnimationClipInfo r)
			{
				if (r == null)
					return false;
				return (this.clip == r.clip && this.renamePath == r.renamePath);
			}
		}

        bool filterFolding;
		string cutomFilter;
		UnityEngine.Object folderFilter;
		string searchWord;
		string replaceWord;
		static bool perfectMatch = false;
        static bool realTimeSearch = true;

		Vector2 scrollPos = Vector2.zero;

		List<string> searchAssetPassList = new List<string> ();
		List<SearchAnimationClipInfo> searchClipList = new List<SearchAnimationClipInfo> ();
		List<GameObject> searchPrefabList = new List<GameObject> ();

		void OnGUI ()
		{
			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
			{
				EditorGUI.BeginChangeCheck ();
				if (filterFolding = EditorGUILayout.Foldout (filterFolding, "フィルター"))
				{
					using (new GUILayout.VerticalScope (GUI.skin.box, GUILayout.Width (400)))
					{
						for (int i = 0; i < filterList.Count;)
						{
							using (new GUILayout.HorizontalScope ())
							{
								for (int j = 0; j < 4; j++, i++)
								{
									if (filterList.Count <= i)
										break;
							
									var filter = filterList [i];
									filter.isEnable = EditorGUILayout.ToggleLeft (filter.name, filter.isEnable, GUILayout.Width (120));
								}
							}
						}
						using (new GUILayout.HorizontalScope ())
						{
							EditorGUILayout.LabelField ("Custom", GUILayout.Width (60));
							cutomFilter = EditorGUILayout.TextField ("", cutomFilter, GUILayout.Width (200));
							GUILayout.ExpandWidth (true);
						}
						using (new GUILayout.HorizontalScope ())
						{
							EditorGUILayout.LabelField ("フォルダ", GUILayout.Width (60));
							folderFilter = EditorGUILayout.ObjectField (folderFilter, typeof(UnityEngine.Object), false, GUILayout.Width (200));
							GUILayout.ExpandWidth (true);
						}
					}
				}
                using (new GUILayout.HorizontalScope())
                {
                    perfectMatch = EditorGUILayout.ToggleLeft("完全一致", perfectMatch, GUILayout.Width(120));
                    realTimeSearch = EditorGUILayout.ToggleLeft("リアルタイム検索", realTimeSearch, GUILayout.Width(120));
                }
				using (new GUILayout.HorizontalScope ())
				{
					EditorGUILayout.LabelField ("検索文字", GUILayout.Width (60));
					searchWord = EditorGUILayout.TextField ("", searchWord, GUILayout.Width (200));
					GUILayout.ExpandWidth (true);
				}
				using (new GUILayout.HorizontalScope ())
				{
					EditorGUILayout.LabelField ("置換文字", GUILayout.Width (60));
					replaceWord = EditorGUILayout.TextField ("", replaceWord, GUILayout.Width (200));
					GUILayout.ExpandWidth (true);
				}
				if (EditorGUI.EndChangeCheck ())
				{			
                    if (realTimeSearch)
                    {
                        Search();
                    }
				}
                if (!realTimeSearch && GUILayout.Button("検索", GUILayout.Width(260)))
                {
                    Search();
                }
				
				//アニメーションクリップの中のパス，prefabの中のオブジェクト,
				EditorGUILayout.LabelField ("アセット数:" + searchAssetPassList.Count ());
				EditorGUILayout.LabelField ("Animationパス数:" + searchClipList.Count ());
				EditorGUILayout.LabelField ("Prefab内オブジェクト数:" + searchPrefabList.Count ());

				//アセットリネーム
				if (searchAssetPassList.Count > 0)
				{
					using (new GUILayout.VerticalScope (GUI.skin.box))
					{
						using (new GUILayout.HorizontalScope ())
						{
							EditorGUILayout.LabelField ("Asset名", GUILayout.Width (150));
							EditorGUILayout.Space ();
							if (GUILayout.Button ("全置換", GUILayout.Width (100)))
							{
								ReplaceAssetName (searchAssetPassList, searchWord, replaceWord);
								Search ();
							}
						}
						GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
						for (int i = 0; i < searchAssetPassList.Count; i++)
						{
							var assetPath = searchAssetPassList [i];
							using (new GUILayout.HorizontalScope ())
							{
								var target = AssetDatabase.LoadAssetAtPath<UnityEngine.Object> (assetPath);
								if (GUILayout.Button (Path.GetFileName (assetPath), GUI.skin.label, GUILayout.Width (150)))
								{
									Selection.objects = new[] {
										target
									};
								}
								EditorGUILayout.Space ();
								if (GUILayout.Button ("置換", GUILayout.Width (100)))
								{
									ReplaceAssetName (assetPath, searchWord, replaceWord);
									Search ();
								}
							}
						}
					}
				}

				//クリップパスリネーム
				if (searchClipList.Count () > 0)
				{
					using (new GUILayout.VerticalScope (GUI.skin.box))
					{
						using (new GUILayout.HorizontalScope ())
						{
							EditorGUILayout.LabelField ("Asset名", GUILayout.Width (150));
							EditorGUILayout.LabelField ("Clip名", GUILayout.Width (100));
							EditorGUILayout.LabelField ("ClipPath", GUILayout.Width (100));
							EditorGUILayout.Space ();
							if (GUILayout.Button ("全置換", GUILayout.Width (100)))
							{
								ReplaceAnimationClipPath (searchClipList.Select ((i) => i.clip).ToArray (), searchWord, replaceWord);
								Search ();
							}
						}
						GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
						for (int i = 0; i < searchClipList.Count; i++)
						{
							var clipInfo = searchClipList [i];
							using (new GUILayout.HorizontalScope ())
							{
								if (GUILayout.Button (Path.GetFileName (AssetDatabase.GetAssetPath (clipInfo.clip)), GUI.skin.label, GUILayout.Width (150)))
								{
									Selection.objects = new[] {
										clipInfo.clip
									};
								}
								EditorGUILayout.LabelField (clipInfo.clip.name, GUILayout.Width (100));
								EditorGUILayout.LabelField (clipInfo.renamePath);
								if (GUILayout.Button ("置換", GUILayout.Width (100)))
								{
									ReplaceAnimationClipPath (clipInfo.clip, searchWord, replaceWord);
									Search ();
								}
							}
						}
					}
				}

				//prefab内オブジェクトリネーム
				if (searchPrefabList.Count > 0)
				{
					using (new GUILayout.VerticalScope (GUI.skin.box))
					{
						using (new GUILayout.HorizontalScope ())
						{
							EditorGUILayout.LabelField ("Prefab名", GUILayout.Width (150));
							EditorGUILayout.LabelField ("階層", GUILayout.Width (100));
							EditorGUILayout.Space ();
							if (GUILayout.Button ("全置換", GUILayout.Width (100)))
							{
								ReplaceChildName (searchPrefabList.ToArray (), searchWord, replaceWord);
								Search ();
							}
						}
						GUILayout.Box ("", GUILayout.ExpandWidth (true), GUILayout.Height (1));
						for (int i = 0; i < searchPrefabList.Count; i++)
						{
							var childObject = searchPrefabList [i];
							using (new GUILayout.HorizontalScope ())
							{
								var parentPath = AssetDatabase.GetAssetPath (childObject);
								if (GUILayout.Button (Path.GetFileName (parentPath), GUI.skin.label, GUILayout.Width (150)))
								{
									Selection.objects = new[] {
										AssetDatabase.LoadAssetAtPath<UnityEngine.Object> (parentPath)
									};
								}
								EditorGUILayout.LabelField (GetHierarchyPath (childObject.transform));
								if (GUILayout.Button ("置換", GUILayout.Width (100)))
								{
									ReplaceChildName (childObject, searchWord, replaceWord);
									Search ();
								}
							}
						}
					}
				}
			}

			EditorGUILayout.EndScrollView ();
		}

		public List<SearchAnimationClipInfo> SearchInAnimatorController (AnimatorController controller)
		{
			List<SearchAnimationClipInfo> ret = new List<SearchAnimationClipInfo> ();
			var clips = controller.animationClips.Distinct ().ToList ();
			for (int i = 0; i < clips.Count; i++)
			{
				var clip = clips [i];
				var clipList = SearchAnimationClip (clip);
				ret.AddRange (clipList);
			}

			return ret;
		}

		public List<SearchAnimationClipInfo> SearchAnimationClip (AnimationClip searchClip)
		{
			var bindings = AnimationUtility.GetCurveBindings (searchClip);
			var searchList = bindings.Where (c => {
				
				bool ret = c.path.Match (searchWord, perfectMatch, true);
				Debug.Log(c.path + ":" + ret);
				return ret;
			}).Select ((b) => new SearchAnimationClipInfo () {
				clip = searchClip,
				renamePath = b.path,
			}).ToList ();

			return searchList;
		}

		public void Search ()
		{
			searchAssetPassList.Clear ();
			searchClipList.Clear ();
			searchPrefabList.Clear ();

			if (string.IsNullOrEmpty (searchWord))
				return;
		
			searchAssetPassList = AssetDatabase.FindAssets (GetFilter ())
				.Select (guid => AssetDatabase.GUIDToAssetPath (guid))
				.Where (pass =>
			{
				if (folderFilter == null)
				{
					return true;
				}
				var folderPath = AssetDatabase.GetAssetPath (folderFilter);

				return pass.Contains (folderPath);
			}).Where (pass => pass.Match (searchWord, perfectMatch)).ToList ();
			
			//コントローラー内のクリップ検索
			var clipPathList = AssetDatabase.FindAssets ("t:AnimatorController")
				.Select (guid => AssetDatabase.GUIDToAssetPath (guid))
				.Where (pass =>
			{
				if (folderFilter == null)
				{
					return true;
				}
				var folderPath = AssetDatabase.GetAssetPath (folderFilter);

				return pass.Contains (folderPath);
			}).ToList ();
			foreach (var pass in clipPathList)
			{
				var controller = AssetDatabase.LoadAssetAtPath<AnimatorController> (pass);
				var searchList = SearchInAnimatorController (controller);
				searchClipList.AddRange (searchList);
				searchClipList = searchClipList.Distinct ().ToList ();
			}

			//クリップパス検索
			clipPathList = AssetDatabase.FindAssets ("t:AnimationClip")
				.Select (guid => AssetDatabase.GUIDToAssetPath (guid))
				.Where (pass =>
			{
				if (folderFilter == null)
				{
					return true;
				}
				var folderPath = AssetDatabase.GetAssetPath (folderFilter);

				return pass.Contains (folderPath);
			}).ToList ();
			foreach (var pass in clipPathList)
			{
				var clip = AssetDatabase.LoadAssetAtPath<AnimationClip> (pass);
				var searchList = SearchAnimationClip (clip);
				searchClipList.AddRange (searchList);
				searchClipList = searchClipList.Distinct ().ToList ();
			}

			var prefabList = AssetDatabase.FindAssets ("t:Prefab")
				.Select (guid => AssetDatabase.GUIDToAssetPath (guid))
				.Where (pass =>
			{
				if (folderFilter == null)
				{
					return true;
				}
				var folderPath = AssetDatabase.GetAssetPath (folderFilter);

				return pass.Contains (folderPath);
			}).ToList ();
			foreach (var pass in prefabList)
			{
				var prefabObject = AssetDatabase.LoadAssetAtPath<GameObject> (pass);
				var childList = prefabObject.GetComponentsInChildren<Transform> (true);
				foreach (var child in childList)
				{
					if (child.gameObject == prefabObject)
						continue;

					if (child.name.Match (searchWord, perfectMatch))
					{
						searchPrefabList.Add (child.gameObject);
					}
				}
			}
		}

		string GetFilter ()
		{
			string ret = "";
			foreach (var filter in filterList)
			{
				if (filter.isEnable)
				{
					ret += "t:" + filter.name + " ";
				}
			}
			ret += cutomFilter + " ";
			ret += searchWord;
			return ret;
		}

		static void ReplaceAssetName (List<string> targetPassList, string searchWord, string replaceWord)
		{
			foreach (var pass in targetPassList)
			{
				ReplaceAssetName (pass, searchWord, replaceWord);
			}
		}

		static void ReplaceAssetName (string targetPass, string searchWord, string replaceWord)
		{
			var fileName = System.IO.Path.GetFileNameWithoutExtension (targetPass);
			var renameFileName = fileName.Replace (searchWord, replaceWord);
			AssetDatabase.RenameAsset (targetPass, renameFileName);
		}

		static void ReplaceChildName (GameObject[] targetList, string searchWord, string replaceWord)
		{
			Undo.RecordObjects (targetList, "replace child paths");
			List<GameObject> replaceList = new List<GameObject> ();
			for (int j = 0; j < targetList.Length; j++)
			{
				var target = targetList [j];
				var childList = target.GetComponentsInChildren<Transform> (true);

				for (int i = 0; i < childList.Length; i++)
				{
					var child = childList [i];
					if (child == target)
						continue;
					replaceList.Add (child.gameObject);
				}
			}

			foreach (var child in replaceList)
			{
				ReplaceChildName (child, searchWord, replaceWord);
			}
		}

		static void ReplaceChildName (GameObject child, string searchWord, string replaceWord)
		{
			if (child.name.Match (searchWord, perfectMatch))
			{
				child.name = child.name.Replace (searchWord, replaceWord);
			}
		}

		static void ReplaceAnimationClipPath (AnimationClip[] clips, string searchWord, string replaceWord)
		{
			Undo.RecordObjects (clips, "replace animationclip paths");
			foreach (var clip in clips)
			{
				ReplaceAnimationClipPath (clip, searchWord, replaceWord);
			}
		}

		static void ReplaceAnimationClipPath (AnimationClip clip, string searchWord, string replaceWord)
		{
			var bindings = AnimationUtility.GetCurveBindings (clip);
			var removeBindings = bindings.Where (c => c.path.Match (searchWord, perfectMatch, true));

			foreach (var binding in removeBindings)
			{
				var curve = AnimationUtility.GetEditorCurve (clip, binding);
				var newBinding = binding;
				newBinding.path = newBinding.path.Replace (searchWord, replaceWord);
				AnimationUtility.SetEditorCurve (clip, binding, null);
				AnimationUtility.SetEditorCurve (clip, newBinding, curve);
			}
		}

		static string GetHierarchyPath (Transform self)
		{
			string path = self.gameObject.name;
			Transform parent = self.parent;
			while (parent != null)
			{
				path = parent.name + "/" + path;
				parent = parent.parent;
			}
			return path;
		}
	}

	public static class StringExtension
	{
		public static bool Match (this string path, string matchWord, bool perfectMatch, bool directoryMatch = false)
		{
			var fileName = System.IO.Path.GetFileNameWithoutExtension (path);
			if (perfectMatch)
			{
				if (directoryMatch)
				{
					return fileName == matchWord || path.StartsWith(matchWord + "/", StringComparison.Ordinal) || path.Contains ("/" + matchWord + "/");
				}
				else
				{
					return fileName == matchWord;
				}
			}
			else
			{
				if (directoryMatch)
				{
					return path.Contains (matchWord);
				}
				else
				{
					return fileName.Contains (matchWord);
				}
			}
		}
	}
}