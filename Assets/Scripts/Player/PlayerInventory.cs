using UFB.Items;
using System.Collections.Generic;


namespace UFB.Player {
    public class PlayerInventory {
        public List<IInventoryItem> Items { get; }
        public List<IInventoryItem> EquippedItems { get; }

        public void AddItem(IInventoryItem item) {
            Items.Add(item);
        }

    }
}
