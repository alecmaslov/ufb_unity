// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.15
// 

using Colyseus.Schema;
using Action = System.Action;

namespace UFB.StateSchema {
	public partial class LobbyRoomInfo : Schema {
		[Type(0, "string")]
		public string roomId = default(string);

		[Type(1, "string")]
		public string name = default(string);

		[Type(2, "number")]
		public float maxPlayers = default(float);

		[Type(3, "number")]
		public float playerCount = default(float);

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

		protected override void TriggerFieldChange(DataChange change) {
			switch (change.Field) {
				case nameof(roomId): __roomIdChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(name): __nameChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
				case nameof(maxPlayers): __maxPlayersChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				case nameof(playerCount): __playerCountChange?.Invoke((float) change.Value, (float) change.PreviousValue); break;
				default: break;
			}
		}
	}
}
