// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.15
// 

using Colyseus.Schema;
using Action = System.Action;

namespace UFB.StateSchema {
	public partial class WaitingState : Schema {
		[Type(0, "string")]
		public string roomId = default(string);

		[Type(1, "string")]
		public string ownerId = default(string);

		[Type(2, "string")]
		public string mapName = default(string);

		[Type(3, "number")]
		public float maxPlayers = default(float);

		[Type(4, "array", typeof(ArraySchema<Player>))]
		public ArraySchema<Player> players = new ArraySchema<Player>();

		/*
		 * Support for individual property change callbacks below...
		 */

		protected event PropertyChangeHandler<string> __roomIdChange;
		public Action OnRoomIdChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.roomId));
			__roomIdChange += __handler;
			if (__immediate && this.roomId != default(string)) { __handler(this.roomId, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(roomId));
				__roomIdChange -= __handler;
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

		protected event PropertyChangeHandler<string> __mapNameChange;
		public Action OnMapNameChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.mapName));
			__mapNameChange += __handler;
			if (__immediate && this.mapName != default(string)) { __handler(this.mapName, default(string)); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(mapName));
				__mapNameChange -= __handler;
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

		protected event PropertyChangeHandler<ArraySchema<Player>> __playersChange;
		public Action OnPlayersChange(PropertyChangeHandler<ArraySchema<Player>> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.players));
			__playersChange += __handler;
			if (__immediate && this.players != null) { __handler(this.players, null); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(players));
				__playersChange -= __handler;
			};
		}

		protected override void TriggerFieldChange(DataChange change) {
			switch (change.Field) {
				case nameof(roomId): __roomIdChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(ownerId): __ownerIdChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(mapName): __mapNameChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(maxPlayers): __maxPlayersChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(players): __playersChange?.Invoke((ArraySchema<Player>) change.Value, (ArraySchema<Player>) change.PreviousValue); break;
				default: break;
			}
		}
	}
}
