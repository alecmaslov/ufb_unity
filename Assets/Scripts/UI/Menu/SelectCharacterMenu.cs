using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UFB.Character;
using TMPro;
using UFB.Network.RoomMessageTypes;
using UnityEngine.InputSystem.LowLevel;

namespace UFB.UI
{
    public class SelectCharacterMenu : Menu
    {

        public Menu mainMenu;
        public Menu selectMapMenu;

        [SerializeField]
        private GameObject _characterList;

        [SerializeField]
        private ListItem characterItem;

        [SerializeField]
        private Image _characterCard;

        [SerializeField]
        private Text _characterName;

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

            InitCharacterList();

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
            _characterCard.sprite = character.avatar;
            _characterName.text = character.characterName;
            SetItemImage(character.id);
        }

        public void InitCharacterList()
        {
            if (_characterList.transform.childCount != 1) return;
            foreach (var item in _characters)
            {
                ListItem li = Instantiate(characterItem, _characterList.transform) as ListItem;
                li.SetImage(item.avatar);
                li.id = item.id;
                li.gameObject.SetActive(true);
            }
        }

        public void SetItemImage(string characterId)
        {
            for(int i = 0; i <  _characterList.transform.childCount; i++)
            {
                ListItem li = _characterList.transform.GetChild(i).GetComponent<ListItem>();
                Color color = Color.white;
                if (li.id != characterId)
                {
                    color.a = 0.5f;
                }
                li.image.color = color;
                li.transform.GetComponent<Image>().color = color;
            }
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
