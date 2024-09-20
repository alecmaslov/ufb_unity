// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.15
// 

using Colyseus.Schema;
using Action = System.Action;

namespace UFB.StateSchema {
	public partial class CharacterStatsState : Schema {
		[Type(0, "ref", typeof(RangedValueState))]
		public RangedValueState health = new RangedValueState();

		[Type(1, "ref", typeof(RangedValueState))]
		public RangedValueState energy = new RangedValueState();

		[Type(2, "ref", typeof(RangedValueState))]
		public RangedValueState ultimate = new RangedValueState();

		[Type(3, "boolean")]
		public bool isRevive = default(bool);

		[Type(4, "int32")]
		public int coin = default(int);

		[Type(5, "int32")]
		public int range = default(int);

		[Type(6, "int32")]
		public int bags = default(int);

		[Type(7, "int16")]
		public short arrowLimit = default(short);

		[Type(8, "int16")]
		public short bombLimit = default(short);

		[Type(9, "int8")]
		public sbyte maxMelee = default(sbyte);

		[Type(10, "int8")]
		public sbyte maxMana = default(sbyte);

		/*
		 * Support for individual property change callbacks below...
		 */

		protected event PropertyChangeHandler<RangedValueState> __healthChange;
		public Action OnHealthChange(PropertyChangeHandler<RangedValueState> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.health));
			__healthChange += __handler;
			if (__immediate && this.health != null) { __handler(this.health, null); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(health));
				__healthChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<RangedValueState> __energyChange;
		public Action OnEnergyChange(PropertyChangeHandler<RangedValueState> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.energy));
			__energyChange += __handler;
			if (__immediate && this.energy != null) { __handler(this.energy, null); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(energy));
				__energyChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<RangedValueState> __ultimateChange;
		public Action OnUltimateChange(PropertyChangeHandler<RangedValueState> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.ultimate));
			__ultimateChange += __handler;
			if (__immediate && this.ultimate != null) { __handler(this.ultimate, null); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(ultimate));
				__ultimateChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<bool> __isReviveChange;
		public Action OnIsReviveChange(PropertyChangeHandler<bool> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.isRevive));
			__isReviveChange += __handler;
			if (__immediate && this.isRevive != default(bool)) { __handler(this.isRevive, default(bool)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(isRevive));
				__isReviveChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<int> __coinChange;
		public Action OnCoinChange(PropertyChangeHandler<int> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.coin));
			__coinChange += __handler;
			if (__immediate && this.coin != default(int)) { __handler(this.coin, default(int)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(coin));
				__coinChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<int> __rangeChange;
		public Action OnRangeChange(PropertyChangeHandler<int> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.range));
			__rangeChange += __handler;
			if (__immediate && this.range != default(int)) { __handler(this.range, default(int)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(range));
				__rangeChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<int> __bagsChange;
		public Action OnBagsChange(PropertyChangeHandler<int> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.bags));
			__bagsChange += __handler;
			if (__immediate && this.bags != default(int)) { __handler(this.bags, default(int)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(bags));
				__bagsChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<short> __arrowLimitChange;
		public Action OnArrowLimitChange(PropertyChangeHandler<short> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.arrowLimit));
			__arrowLimitChange += __handler;
			if (__immediate && this.arrowLimit != default(short)) { __handler(this.arrowLimit, default(short)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(arrowLimit));
				__arrowLimitChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<short> __bombLimitChange;
		public Action OnBombLimitChange(PropertyChangeHandler<short> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.bombLimit));
			__bombLimitChange += __handler;
			if (__immediate && this.bombLimit != default(short)) { __handler(this.bombLimit, default(short)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(bombLimit));
				__bombLimitChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<sbyte> __maxMeleeChange;
		public Action OnMaxMeleeChange(PropertyChangeHandler<sbyte> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.maxMelee));
			__maxMeleeChange += __handler;
			if (__immediate && this.maxMelee != default(sbyte)) { __handler(this.maxMelee, default(sbyte)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(maxMelee));
				__maxMeleeChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<sbyte> __maxManaChange;
		public Action OnMaxManaChange(PropertyChangeHandler<sbyte> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.maxMana));
			__maxManaChange += __handler;
			if (__immediate && this.maxMana != default(sbyte)) { __handler(this.maxMana, default(sbyte)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(maxMana));
				__maxManaChange -= __handler;
			};
		}

		protected override void TriggerFieldChange(DataChange change) {
			switch (change.Field) {
				case nameof(health): __healthChange?.Invoke((RangedValueState) change.Value, (RangedValueState) change.PreviousValue); break;
				case nameof(energy): __energyChange?.Invoke((RangedValueState) change.Value, (RangedValueState) change.PreviousValue); break;
				case nameof(ultimate): __ultimateChange?.Invoke((RangedValueState) change.Value, (RangedValueState) change.PreviousValue); break;
				case nameof(isRevive): __isReviveChange?.Invoke((bool) change.Value, (bool) change.PreviousValue); break;
				case nameof(coin): __coinChange?.Invoke((int) change.Value, (int) change.PreviousValue); break;
				case nameof(range): __rangeChange?.Invoke((int) change.Value, (int) change.PreviousValue); break;
				case nameof(bags): __bagsChange?.Invoke((int) change.Value, (int) change.PreviousValue); break;
				case nameof(arrowLimit): __arrowLimitChange?.Invoke((short) change.Value, (short) change.PreviousValue); break;
				case nameof(bombLimit): __bombLimitChange?.Invoke((short) change.Value, (short) change.PreviousValue); break;
				case nameof(maxMelee): __maxMeleeChange?.Invoke((sbyte) change.Value, (sbyte) change.PreviousValue); break;
				case nameof(maxMana): __maxManaChange?.Invoke((sbyte) change.Value, (sbyte) change.PreviousValue); break;
				default: break;
			}
		}
	}
}
