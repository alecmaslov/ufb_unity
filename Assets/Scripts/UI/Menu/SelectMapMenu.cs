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
            _characterName.text = map.name;
            SetItemImage(map.id);
            _menuManager.SetMenuData("createOptions", createOptions);
        }

        public void OnConfirmButton()
        {
            _menuManager.OpenMenu(loadingMenu);

            ServiceLocator.Current
                .Get<GameService>()
                .CreateGame(
                    _menuManager.GetMenuData("createOptions") as UfbRoomCreateOptions,
                    _menuManager.GetMenuData("joinOptions") as UfbRoomJoinOptions
                );

        }

        private Sprite TextureToSprite(Texture2D texture)
        {
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        }
    }
}
