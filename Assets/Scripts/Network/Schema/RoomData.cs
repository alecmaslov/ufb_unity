// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.15
// 

using Colyseus.Schema;
using Action = System.Action;

namespace UFB.StateSchema {
	public partial class RoomData : Schema {
		[Type(0, "string")]
		public string id = default(string);

		[Type(1, "string")]
		public string name = default(string);

		[Type(2, "number")]
		public float maxPlayers = default(float);

		[Type(3, "number")]
		public float playerCount = default(float);

		[Type(4, "string")]
		public string ownerId = default(string);

		[Type(5, "string")]
		public string inviteToken = default(string);

		[Type(6, "boolean")]
		public bool isPrivate = default(bool);

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

		protected event PropertyChangeHandler<string> __nameChange;
		public Action OnNameChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.name));
			__nameChange += __handler;
			if (__immediate && this.name != default(string)) { __handler(this.name, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(name));
				__nameChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __maxPlayersChange;
		public Action OnMaxPlayersChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.maxPlayers));
			__maxPlayersChange += __handler;
			if (__immediate && this.maxPlayers != default(float)) { __handler(this.maxPlayers, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(maxPlayers));
				__maxPlayersChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<float> __playerCountChange;
		public Action OnPlayerCountChange(PropertyChangeHandler<float> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.playerCount));
			__playerCountChange += __handler;
			if (__immediate && this.playerCount != default(float)) { __handler(this.playerCount, default(float)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(playerCount));
				__playerCountChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<string> __ownerIdChange;
		public Action OnOwnerIdChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.ownerId));
			__ownerIdChange += __handler;
			if (__immediate && this.ownerId != default(string)) { __handler(this.ownerId, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(ownerId));
				__ownerIdChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<string> __inviteTokenChange;
		public Action OnInviteTokenChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.inviteToken));
			__inviteTokenChange += __handler;
			if (__immediate && this.inviteToken != default(string)) { __handler(this.inviteToken, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(inviteToken));
				__inviteTokenChange -= __handler;
			};
		}

		protected event PropertyChangeHandler<bool> __isPrivateChange;
		public Action OnIsPrivateChange(PropertyChangeHandler<bool> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.isPrivate));
			__isPrivateChange += __handler;
			if (__immediate && this.isPrivate != default(bool)) { __handler(this.isPrivate, default(bool)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(isPrivate));
				__isPrivateChange -= __handler;
			};
		}

		protected override void TriggerFieldChange(DataChange change) {
			switch (change.Field) {
				case nameof(id): __idChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(name): __nameChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(maxPlayers): __maxPlayersChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(playerCount): __playerCountChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(ownerId): __ownerIdChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(inviteToken): __inviteTokenChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(isPrivate): __isPrivateChange?.Invoke((bool) change.Value, (bool) change.PreviousValue); break;
				default: break;
			}
		}
	}
}
