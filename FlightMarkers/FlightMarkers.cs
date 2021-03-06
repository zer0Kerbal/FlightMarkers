﻿using FlightMarkers.Utilities;
using KSP.Localization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlightMarkers
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class FlightMarkers : MonoBehaviour
	{
		public enum Strings
		{
			FlightMarkersOn,
			FlightMarkersOff,
			CombineLiftOn,
			CombineLiftOff
		}

		public static Dictionary<Strings, string> LocalStrings;
		public static FlightMarkers Instance { get; private set; }
		public static event Action OnUpdateEvent;
		public static event Action OnRenderObjectEvent;


		private void Awake()
		{
			if (Instance != null)
			{
				DestroyImmediate(this);
				return;
			}

			LocalStrings = new Dictionary<Strings, string>();

			OnLanguageSwitched();

			Instance = this;
		}


		private void Start()
		{
			UpdateSettings();

			GameEvents.onLanguageSwitched.Add(OnLanguageSwitched);
			GameEvents.onHideUI.Add(OnHideUI);
			GameEvents.onShowUI.Add(OnShowUI);
			GameEvents.OnGameSettingsApplied.Add(OnGameSettingsApplied);
			GameEvents.onVesselGoOnRails.Add(OnVesselGoOnRails);
		}


		private void OnVesselGoOnRails(Vessel v)
		{
			var vfm = VesselFlightMarkers.VesselModules[v];

			if (vfm == null) return;

			vfm.MarkersEnabled = false;
		}


		private void OnLanguageSwitched()
		{
			LocalStrings[Strings.FlightMarkersOn] = Localizer.Format("#SSC_FM_000001",
				Localizer.GetStringByTag("#SSC_FM_000002"));

			LocalStrings[Strings.FlightMarkersOff] = Localizer.Format("#SSC_FM_000001",
				Localizer.GetStringByTag("#SSC_FM_000003"));

			LocalStrings[Strings.CombineLiftOn] = Localizer.Format("#SSC_FM_000004",
				Localizer.GetStringByTag("#SSC_FM_000002"));

			LocalStrings[Strings.CombineLiftOff] = Localizer.Format("#SSC_FM_000004",
				Localizer.GetStringByTag("#SSC_FM_000003"));
		}


		private void OnHideUI()
		{
			foreach (var module in VesselFlightMarkers.VesselModules.Values)
			{
				module.Hidden = true;
			}
		}


		private void OnShowUI()
		{
			foreach (var module in VesselFlightMarkers.VesselModules.Values)
			{
				module.Hidden = false;
			}
		}


		private void OnGameSettingsApplied()
		{
			UpdateSettings();
		}


		private void UpdateSettings()
		{
			VesselFlightMarkers.CenterOfLiftCutoff = HighLogic.CurrentGame.Parameters.CustomParams<Settings>().LiftCutoff;
			VesselFlightMarkers.BodyLiftCutoff = HighLogic.CurrentGame.Parameters.CustomParams<Settings>().BodyLiftCutoff;
			VesselFlightMarkers.DragCutoff = HighLogic.CurrentGame.Parameters.CustomParams<Settings>().DragCutoff;
		}


		private void Update()
		{
			OnUpdateEvent?.Invoke();
		}


		private void OnRenderObject()
		{
			OnRenderObjectEvent?.Invoke();
		}


		private void OnGUI()
		{
			DrawTools.NewFrame();
		}


		private void OnDestroy()
		{
			enabled = false;

			GameEvents.onLanguageSwitched.Remove(OnLanguageSwitched);
			GameEvents.onHideUI.Remove(OnHideUI);
			GameEvents.onShowUI.Remove(OnShowUI);
			GameEvents.OnGameSettingsApplied.Remove(OnGameSettingsApplied);

			OnUpdateEvent = null;
			OnRenderObjectEvent = null;
			Instance = null;
		}
	}
}
