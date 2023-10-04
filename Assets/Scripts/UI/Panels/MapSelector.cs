using System.Collections;
using System.Collections.Generic;
using UFB.Map;
using UnityEngine;

namespace UFB.UI
{
    public class MapSelector : MenuSelector<UfbMap>
    {
        public override void OnClickImage()
        {
            throw new System.NotImplementedException();
        }

        public override void OnClickNext()
        {
            base.OnClickNext();
        }

        public override void OnClickLast()
        {
            base.OnClickLast();
            // SetDisplayContent(CurrentSelection.avatar);
        }
    }
}
