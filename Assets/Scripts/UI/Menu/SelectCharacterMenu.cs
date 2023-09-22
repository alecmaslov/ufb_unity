using System.Collections;
using System.Collections.Generic;
using UFB.Gameplay;
using UFB.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UFB.Player;
using TMPro;

namespace UFB.UI
{
    public class SelectCharacterMenu : Menu
    {
        public Menu mainMenu;

        [SerializeField] private Image _characterCard;
        [SerializeField] private TextMeshProUGUI _characterName;

        // [SerializeField] private Dictionary<string, Sprite> _characterSprites = new Dictionary<string, Sprite>();
        [SerializeField] private List<UfbCharacter> _characters;

        private int _characterIndex = 0;

        public void OnBackButton() => _menuManager.CloseMenu();

        private void OnEnable()
        {
            SetCharacter(_characterIndex);
        }

        public void OnConfirmButton()
        {           
            var joinOptions = _menuManager.GetMenuData("joinOptions") as UfbRoomJoinOptions;
            joinOptions.characterId = _characters[_characterIndex].id;
            // _menuManager.SetMenuData("joinOptions", joinOptions);
            _menuManager.CloseMenu();

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
