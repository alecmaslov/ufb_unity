// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.15
// 

using Colyseus.Schema;
using Action = System.Action;

namespace UFB.StateSchema {
	public partial class Player : Schema {
		[Type(0, "string")]
		public string id = default(string);

		[Type(1, "string")]
		public string sessionId = default(string);

		[Type(2, "string")]
		public string characterClass = default(string);

		[Type(3, "string")]
		public string displayName = default(string);

		[Type(4, "number")]
		public float joinIndex = default(float);

		/*
		 * Support for individual property change callbacks below...
		 */

		protected event PropertyChangeHandler<string> __idChange;
		public Action OnIdChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.id));
			__idChange += __handler;
			if (__immediate && this.id != default(string)) { __handler(this.id, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(id));
				__idChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<string> __sessionIdChange;
		public Action OnSessionIdChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.sessionId));
			__sessionIdChange += __handler;
			if (__immediate && this.sessionId != default(string)) { __handler(this.sessionId, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(sessionId));
				__sessionIdChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<string> __characterClassChange;
		public Action OnCharacterClassChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.characterClass));
			__characterClassChange += __handler;
			if (__immediate && this.characterClass != default(string)) { __handler(this.characterClass, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(characterClass));
				__characterClassChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<string> __displayNameChange;
		public Action OnDisplayNameChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.displayName));
			__displayNameChange += __handler;
			if (__immediate && this.displayName != default(string)) { __handler(this.displayName, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(displayName));
				__displayNameChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __joinIndexChange;
		public Action OnJoinIndexChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.joinIndex));
			__joinIndexChange += __handler;
			if (__immediate && this.joinIndex != default(float)) { __handler(this.joinIndex, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(joinIndex));
				__joinIndexChange -= __handler;
			};
		}

		protected override void TriggerFieldChange(DataChange change) {
			switch (change.Field) {
				case nameof(id): __idChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(sessionId): __sessionIdChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(characterClass): __characterClassChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(displayName): __displayNameChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(joinIndex): __joinIndexChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				default: break;
			}
		}
	}
}
