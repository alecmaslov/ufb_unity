using Colyseus.Schema;
using System.Collections;
using System.Collections.Generic;
using UFB.Character;
using UFB.Core;
using UFB.Events;
using UFB.Items;
using UFB.Network.RoomMessageTypes;
using UFB.StateSchema;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UFB.UI
{
    public class ResourcePanel : MonoBehaviour
    {
        public static ResourcePanel instance;
        [SerializeField]
        private ItemCard item;

        [SerializeField]
        private Image _avatarImage;

        [SerializeField]
        private Text coinText;

        [SerializeField]
        private Text itemBagText;

        [SerializeField]
        private Text itemText;

        [SerializeField]
        private Text[] stacks;

        [SerializeField]
        private Text heartText;

        [SerializeField]
        private Text crystalText;

        [SerializeField]
        private Text quiverText;

        [SerializeField]
        private Text bombText;

        [SerializeField]
        private Text potionText;

        [SerializeField]
        private Text elixirText;

        [SerializeField]
        private Text featherText;

        [SerializeField]
        private Text warpCrystalText;

        [SerializeField]
        Image heartBackImage;

        [SerializeField]
        Image crystalBackImage;

        [SerializeField]
        ItemDetailPanel bombDetailPanel;

        [SerializeField]
        ItemDetailPanel arrowDetailPanel;

        private void OnEnable()
        {

            /*var gameService = ServiceLocator.Current.Get<GameService>();
            if (gameService.Room == null)
            {
                Debug.LogError("Room is null");
                return;
            }
            gameService.SubscribeToRoomMessage<GetResourceDataMessage>(
                "sendResourceList",
                GetResourceDataReceived
            );*/

            /*EventBus.Publish(
                RoomSendMessageEvent.Create(
                    "getResourceList",
                    new RequestGetResourceMessage
                    {
                        playerId = "",
                    }
                )
            );*/


        }

        private void OnDisable()
        {
            
        }

        /*private void GetResourceDataReceived( GetResourceDataMessage message )
        {
            if ( message == null ) return;

            CharacterState characterState = message.characterState;
            coinText.text = characterState.stats.coin.ToString();
            itemBagText.text = characterState.stats.bags.ToString();

            ArraySchema<Item> items = characterState.items;
            items.ForEach(item =>
            {
                ITEM type = (ITEM)item.id;
                if (type == ITEM.Feather)
                {
                    featherText.text = item.count.ToString();
                }
                else if (type == ITEM.Potion)
                {
                    potionText.text = item.count.ToString();
                }
                else if (type == ITEM.Elixir)
                {
                    potionText.text = item.count.ToString();
                }
                else if (type == ITEM.WarpCrystal)
                {
                    potionText.text = item.count.ToString();
                }
            });

            List<ITEM> arrows = new List<ITEM>
                {
                    ITEM.IceArrow,
                    ITEM.BombArrow,
                    ITEM.FireArrow,
                    ITEM.VoidArrow,
                    ITEM.Arrow
                };

            quiverText.text = GetItemTotalCount(items, ITEM.Quiver, arrows, 1);

            List<ITEM> bombs = new List<ITEM>
                {
                    ITEM.Bomb,
                    ITEM.IceBomb,
                    ITEM.VoidBomb,
                    ITEM.FireBomb
                };

            bombText.text = GetItemTotalCount(items, ITEM.BombBag, bombs, 1);

            heartText.text = GetItemTotalCount(items, ITEM.HeartCrystal, new List<ITEM> { ITEM.HeartPiece }, 4);

            crystalText.text = GetItemTotalCount(items, ITEM.EnergyCrystal, new List<ITEM> { ITEM.EnergyShard }, 3);

        }*/

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        public void InitInstance()
        {
            if (instance == null)
                instance = this;
        }

        public void OnCharacterValueEvent(ChangeCharacterStateEvent e)
        {
            Addressables
            .LoadAssetAsync<UfbCharacter>("UfbCharacter/" + e.state.characterClass)
            .Completed += (op) =>
            {
                if (
                    op.Status
                    == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded
                )
                    _avatarImage.sprite = op.Result.avatar;
                else
                    Debug.LogError(
                        "Failed to load character avatar: " + op.OperationException.Message
                    );
            };


            coinText.text = e.state.stats.coin.ToString();
            itemBagText.text = e.state.stats.bags.ToString();

            ArraySchema<Item> items = e.state.items;
            items.ForEach(item =>
            {
                ITEM type = (ITEM)item.id;
                if (type == ITEM.Feather)
                {
                    featherText.text = item.count.ToString();
                }
                else if (type == ITEM.Potion)
                {
                    potionText.text = item.count.ToString();
                }
                else if (type == ITEM.Elixir)
                {
                    elixirText.text = item.count.ToString();
                }
                else if (type == ITEM.WarpCrystal)
                {
                    warpCrystalText.text = item.count.ToString();
                }
            });

            List<ITEM> arrows = new List<ITEM>
            {
                ITEM.IceArrow,
                ITEM.BombArrow,
                ITEM.FireArrow,
                ITEM.VoidArrow,
                ITEM.Arrow
            };

            quiverText.text = GlobalResources.instance.GetItemTotalCount(items, ITEM.Quiver, arrows, 1).ToString();

            List<ITEM> bombs = new List<ITEM>
            {
                ITEM.Bomb,
                ITEM.IceBomb,
                ITEM.VoidBomb,
                ITEM.FireBomb,
                ITEM.caltropBomb,
            };

            bombText.text = GlobalResources.instance.GetItemTotalCount(items, ITEM.BombBag, bombs, 1).ToString();

            int heartPieceNum = GlobalResources.instance.GetItemTotalCount(items, ITEM.HeartCrystal, new List<ITEM> { ITEM.HeartPiece }, 4);
            heartText.text = heartPieceNum.ToString();
            heartBackImage.sprite = GlobalResources.instance.divideTo4[heartPieceNum % 5];

            int crystalPieceNum = GlobalResources.instance.GetItemTotalCount(items, ITEM.EnergyCrystal, new List<ITEM> { ITEM.EnergyShard }, 3);
            crystalText.text = crystalPieceNum.ToString();
            crystalBackImage.sprite = GlobalResources.instance.divideTo3[crystalPieceNum % 4];
        }

        public void OnBombItemDetailClicked()
        {
            bombDetailPanel.Init();
        }

        public void OnArrowsItemDetailClicked()
        {
            arrowDetailPanel.Init();
        }
    }

 

}

