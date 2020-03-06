/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections;
using System.Linq;

	/// <summary>
	/// Apply this to the GameObject which holds all your Splats. Make sure the origin is correctly centered at the base of the Character.
	/// </summary>
	public class SplatManager : MonoBehaviour {
		/// <summary>
		/// Determines whether the cursor should be hidden when a Splat is showing.
		/// </summary>
		public bool HideCursor;

		/// <summary>
		/// Returns all Spell Indicators belonging to the Manager.
		/// </summary>
		public SpellIndicator[] SpellIndicators { get; set; }

		/// <summary>
		/// Returns all Status Indicators belonging to the Manager.
		/// </summary>
		 

		/// <summary>
		/// Returns all Range Indicators belonging to the Manager.
		/// </summary>
		public RangeIndicator[] RangeIndicators { get; set; }

		/// <summary>
		/// Returns the currently selected Spell Indicator.
		/// </summary>
		public SpellIndicator CurrentSpellIndicator { get; private set; }

		/// <summary>
		/// Returns the currently selected Status Indicator.
		/// </summary>
		 

		/// <summary>
		/// Returns the currently selected Range Indicator.
		/// </summary>
		public RangeIndicator CurrentRangeIndicator { get; private set; }

		void OnEnable() {
			// Create a list of all the splats available to the manager
			SpellIndicators = GetComponentsInChildren<SpellIndicator>();
		 
			RangeIndicators = GetComponentsInChildren<RangeIndicator>();

			// Make sure each Splat has a reference to its Manager
			SpellIndicators.ToList().ForEach(x => x.Manager = this);
			 
			RangeIndicators.ToList().ForEach(x => x.Manager = this);

			// Initialize the Splats
			SpellIndicators.ToList().ForEach(x => x.Initialize());
			 
			RangeIndicators.ToList().ForEach(x => x.Initialize());

			// Make all Splats invisible to start with
			SpellIndicators.ToList().ForEach(x => x.gameObject.SetActive(false));
			RangeIndicators.ToList().ForEach(x => x.gameObject.SetActive(false));
		}

		// This Update method and the "HideCursor" variable can be deleted if you do not need this functionality
		void Update() {
			if(HideCursor) {
				if(CurrentSpellIndicator != null)
					Cursor.visible = false;
				else
					Cursor.visible = true;
			}
		}

		/// <summary>
		/// Position of Current Spell Indicator or Mouse Ray if unavailable
		/// </summary>
	 

		/// <summary>
		/// Select and make visible the Spell Indicator given by name.
		/// </summary>
		public void SelectSpellIndicator(string splatName) {
			CancelSpellIndicator();
			SpellIndicator indicator = GetSpellIndicator(splatName);

			if(indicator.RangeIndicator != null) {
				indicator.RangeIndicator.gameObject.SetActive(true);
				indicator.RangeIndicator.OnShow();
			}

			indicator.gameObject.SetActive(true);
			indicator.OnShow();
			CurrentSpellIndicator = indicator;
		}

	 

		/// <summary>
		/// Select and make visible the Range Indicator given by name.
		/// </summary>
		public void SelectRangeIndicator(string splatName) {
			CancelRangeIndicator();
			RangeIndicator indicator = GetRangeIndicator(splatName);

			// If current spell indicator uses same Range indicator then cancel it.
			if(CurrentSpellIndicator != null && CurrentSpellIndicator.RangeIndicator == indicator) {
				CancelSpellIndicator();
			}

			indicator.gameObject.SetActive(true);
			indicator.OnShow();
			CurrentRangeIndicator = indicator;
		}

		/// <summary>
		/// Return the Spell Indicator given by name.
		/// </summary>
		public SpellIndicator GetSpellIndicator(string splatName) {
			return SpellIndicators.Where(x => x.name == splatName).FirstOrDefault();
		}
 

		/// <summary>
		/// Return the Range Indicator given by name.
		/// </summary>
		public RangeIndicator GetRangeIndicator(string splatName) {
			return RangeIndicators.Where(x => x.name == splatName).FirstOrDefault();
		}

		/// <summary>
		/// Hide Spell Indicator
		/// </summary>
		public void CancelSpellIndicator() {
			if(CurrentSpellIndicator != null) {
				if(CurrentSpellIndicator.RangeIndicator != null) {
					CurrentSpellIndicator.RangeIndicator.gameObject.SetActive(false);
				}

				CurrentSpellIndicator.OnHide();
				CurrentSpellIndicator.gameObject.SetActive(false);
				CurrentSpellIndicator = null;
			}
		}
		/// <summary>
		/// Hide Range Indicator
		/// </summary>
		public void CancelRangeIndicator() {
			if(CurrentRangeIndicator != null) {
				CurrentRangeIndicator.OnHide();
				CurrentRangeIndicator.gameObject.SetActive(false);
				CurrentRangeIndicator = null;
			}
		}
	}

