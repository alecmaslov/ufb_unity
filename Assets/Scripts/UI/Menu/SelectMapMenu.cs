using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UFB.Character;
using TMPro;
using UFB.Network.RoomMessageTypes;
using UFB.Map;
using System;
using UFB.Core;
using UnityEngine.TextCore.Text;
using UI.ThreeDimensional;
using UFB.Network.ApiTypes;
using UnityEngine.Serialization;

namespace UFB.UI
{
    public class SelectMapMenu : Menu
    {
        public Menu selectCharacterMenu;
        public Menu loadingMenu;

        [SerializeField]
        private GameObject mapList;

        [SerializeField]
        private ListItem mapItem;

        [SerializeField]
        private MapSelector mapSelector;

        [SerializeField]
        private Image _characterCard;

        [SerializeField]
        private Text _characterName;

        // [SerializeField] private Dictionary<string, Sprite> _characterSprites = new Dictionary<string, Sprite>();
        [SerializeField]
        private List<UfbCharacter> _characters;

        [SerializeField]
        private UIObject3D uIObject3D;

        [SerializeField]
        private GameObject[] uI3DMaps;

        private int _characterIndex = 0;

        public bool IsSoloMode = false;
        public bool IsJoinMode = false;
        
        [FormerlySerializedAs("joinRoomPanel")] public CreateRoomPanel createRoomPanel;
        
        // 0 : NEW GAME, 1 : JOIN GAME
        [SerializeField]
        public int menuType = 0;

        public void OnBackButton()
        {
            _menuManager.CloseMenu();
            _menuManager.OpenMenu(selectCharacterMenu);
        }
        private void OnEnable()
        {
            if (_menuManager.GetMenuData("joinOptions") == null)
            {
                _menuManager.SetMenuData("joinOptions", new UfbRoomJoinOptions());
            }

            if (_menuManager.GetMenuData("createOptions") == null)
            {
                _menuManager.SetMenuData("createOptions", new UfbRoomCreateOptions());
            }

            InitCharacterList();

            SetItemImage(mapSelector.GetSelectOptions()[0].id);

            mapSelector.OnSelectionChanged += OnMapSelectionChanged;

            var createOptions = _menuManager.GetMenuData("createOptions") as UfbRoomCreateOptions;
            createOptions.mapName = mapSelector.CurrentSelection.mapName;
            _menuManager.SetMenuData("createOptions", createOptions);
        }

        private void OnDisable()
        {
            mapSelector.OnSelectionChanged -= OnMapSelectionChanged;
        }
        public void InitCharacterList()
        {
            if (mapList.transform.childCount != 1) return;
            foreach (var item in mapSelector.GetSelectOptions())
            {
                ListItem li = Instantiate(mapItem, mapList.transform) as ListItem;
                li.SetImage(item.mapThumbnail);
                li.id = item.id;
                li.gameObject.SetActive(true);
            }
        }
        public void SetItemImage(string characterId)
        {
            for (int i = 0; i < mapList.transform.childCount; i++)
            {
                ListItem li = mapList.transform.GetChild(i).GetComponent<ListItem>();
                Color color = Color.white;
                if (li.id != characterId)
                {
                    color.a = 0.5f;
                }
                li.image.color = color;
                li.transform.GetComponent<Image>().color = color;
            }
        }

        private void OnMapSelectionChanged(UfbMap map)
        {
            var createOptions = _menuManager.GetMenuData("createOptions") as UfbRoomCreateOptions;
            createOptions.mapName = map.name;
            createOptions.ownerId = MainScene.instance.userData.id;
            _characterName.text = map.name;
            SetItemImage(map.id);
            _menuManager.SetMenuData("createOptions", createOptions);
        }

        public void OnConfirmButton()
        {
            try
            {
                var createOptions = _menuManager.GetMenuData("createOptions") as UfbRoomCreateOptions;
                var joinOptions = _menuManager.GetMenuData("joinOptions") as UfbRoomJoinOptions;

                MainScene.instance.userData.createOptions = createOptions;
                MainScene.instance.userData.joinOptions = joinOptions;
                //joinOptions.playerId =  MainScene.instance.userData.id;
                
                if (IsSoloMode)
                {
                    _menuManager.OpenMenu(loadingMenu);
                    createOptions.roomId = MainScene.instance.userData.id;
                    createOptions.ownerId = MainScene.instance.userData.id;
                    createOptions.turnIds = new[] { MainScene.instance.userData.id };
                    ServiceLocator.Current
                        .Get<GameService>()
                        .CreateGame(
                            createOptions,
                            joinOptions
                        );
                }
                else if(!IsJoinMode)
                {
                    LobbyService.Instance.CreateRoom(createOptions, joinOptions);
                    gameObject.SetActive(false);
                }
                else
                {
                    MainScene.instance.joinRoomPanel.gameObject.SetActive(true);
                }
                
                CloseMenu();
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
            }
        }

        public void OnSoloCreateButton()
        {
            IsSoloMode = true;
        }

        public void OnMultiPlayerCreateButton()
        {
            IsSoloMode = false;
            IsJoinMode = false;
        }

        public void OnMultiPlayerJoinButton()
        {
            IsSoloMode = false;
            IsJoinMode = true;
        }
    }
}
