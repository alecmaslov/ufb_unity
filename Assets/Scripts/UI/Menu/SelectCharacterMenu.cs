using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UFB.Character;
using TMPro;
using UFB.Network.RoomMessageTypes;

namespace UFB.UI
{
    public class SelectCharacterMenu : Menu
    {

        public Menu mainMenu;
        public Menu selectMapMenu;

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
            _menuManager.OpenMenu(mainMenu);
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

            SetCharacter(_characterIndex);
        }

        public void OnConfirmButton()
        {
            var joinOptions = _menuManager.GetMenuData("joinOptions") as UfbRoomJoinOptions;
            joinOptions.characterClass = _characters[_characterIndex].id;
            // _menuManager.SetMenuData("joinOptions", joinOptions);
            _menuManager.CloseMenu();
            _menuManager.OpenMenu(selectMapMenu);
        }

        private void SetCharacter(int index)
        {
            var character = _characters[index];
            // Sprite avatarSprite = TextureToSprite(character.avatar);
            _characterCard.sprite = character.card;
            _characterName.text = character.characterName;
        }

        public void OnNextButton()
        {
            _characterIndex = (_characterIndex + 1) % _characters.Count;
            SetCharacter(_characterIndex);
        }

        public void OnPrevButton()
        {
            // _characterIndex = (_characterIndex - 1) % _characters.Count;
            _characterIndex -= 1;
            if (_characterIndex < 0)
            {
                _characterIndex = _characters.Count - 1;
            }
            SetCharacter(_characterIndex);
        }

        private Sprite TextureToSprite(Texture2D texture)
        {
            Rect rect = new Rect(0, 0, texture.width, texture.height);
            return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        }
    }
}
