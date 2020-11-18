using System;
using System.Collections.Generic;

namespace SnowrunnerMT {
	class TweakGroup{
		public String GroupName;
		public List<Tweak> Tweaks;
		public TweakGroup(String GroupName) {
			this.GroupName = GroupName;
			Tweaks = new List<Tweak>();
		}
	}
	class Tweak {
		public String TweakName;
		public String TweakType;
		public Int32 ApplyIndex;
		public Tweak(String TweakName, String TweakType,Int32 Index) {
			this.TweakName = TweakName;
			this.TweakType = TweakType;
			ApplyIndex = Index;
		}
	}
	class TweakToApply {
		public String DirectoryName;
		public String ParameterName;
		public String TweakType;

		public Int32 Action = 0;
		public Double Val1 = 0, Val2 = 0;

		private Double max,min; 
		public Double Min {
			get => min;
			set {
				min = value;
				MinChanged?.Invoke();
			}
		}
		public Double Max {
			get => max;
			set {
				max = value;
				MaxChanged?.Invoke();
			}
		}
		public List<Double> Values;

		

		public delegate void EventHandler();
		public event EventHandler MaxChanged,MinChanged;


		public TweakToApply(String DirectoryName, String ParameterName, String TweakType) {
			this.DirectoryName = DirectoryName;
			this.ParameterName = ParameterName;
			this.TweakType = TweakType;
			Values = new List<Double>();
		}
	}
	class SpecialTweaks {
		public String Name;
		public List<String> Directories;
		public List<String> Parameters;
		public List<String> Values;

		public SpecialTweaks(String name) {
			Name = name;
			Directories = new List<string>();
			Parameters = new List<string>();
			Values = new List<string>();
		}
	}
}
