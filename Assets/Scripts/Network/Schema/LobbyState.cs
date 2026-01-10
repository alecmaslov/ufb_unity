// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.15
// 

using Colyseus.Schema;
using Action = System.Action;

namespace UFB.StateSchema {
	public partial class LobbyState : Schema {
		[Type(0, "map", typeof(MapSchema<LobbyRoomInfo>))]
		public MapSchema<LobbyRoomInfo> rooms = new MapSchema<LobbyRoomInfo>();

		/*
		 * Support for individual property change callbacks below...
		 */

		protected event PropertyChangeHandler<MapSchema<LobbyRoomInfo>> __roomsChange;
		public Action OnRoomsChange(PropertyChangeHandler<MapSchema<LobbyRoomInfo>> __handler, bool __immediate = true) {
			if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
			__callbacks.AddPropertyCallback(nameof(this.rooms));
			__roomsChange += __handler;
			if (__immediate && this.rooms != null) { __handler(this.rooms, null); }
			return () => {
				__callbacks.RemovePropertyCallback(nameof(rooms));
				__roomsChange -= __handler;
			};
		}

		protected override void TriggerFieldChange(DataChange change) {
			switch (change.Field) {
				case nameof(rooms): __roomsChange?.Invoke((MapSchema<LobbyRoomInfo>) change.Value, (MapSchema<LobbyRoomInfo>) change.PreviousValue); break;
				default: break;
			}
		}
	}
}
