using System.Collections.Generic;
using UFB.Character;
using UnityEngine;

namespace UFB.UI
{
    public class CharacterSelector : MenuSelector<UfbCharacter>
    {
        public override void OnClickImage()
        {
            throw new System.NotImplementedException();
        }

        public override void OnClickNext()
        {
            base.OnClickNext();
            SetDisplayContent(CurrentSelection.avatar);
        }

        public override void OnClickLast()
        {
            base.OnClickLast();
            SetDisplayContent(CurrentSelection.avatar);
        }
    }
}
