using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UFB.Character;
using TMPro;
using UFB.Network.RoomMessageTypes;
using UFB.Map;
using System;
using UFB.Core;

namespace UFB.UI
{
    public class SelectMapMenu : Menu
    {
        public Menu selectCharacterMenu;
        public Menu loadingMenu;

        [SerializeField]
        private MapSelector mapSelector;

        [SerializeField]
        private Image _characterCard;

        [SerializeField]
        private TextMeshProUGUI _characterName;

        // [SerializeField] private Dictionary<string, Sprite> _characterSprites = new Dictionary<string, Sprite>();
        [SerializeField]
        private List<UfbCharacter> _characters;

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
            mapSelector.OnSelectionChanged += OnMapSelectionChanged;
        }

        private void OnDisable()
        {
            mapSelector.OnSelectionChanged -= OnMapSelectionChanged;
        }

        private void OnMapSelectionChanged(UfbMap map)
        {
            var createOptions = _menuManager.GetMenuData("createOptions") as UfbRoomCreateOptions;
            createOptions.mapName = map.name;
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
