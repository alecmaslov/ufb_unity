using UnityEngine;
using System.Collections;

namespace UFB.Items {
    
    enum TokenType {
        Melee,
        Magic,
        EnergyShard,
        EnergyCrystal,
        HeartCrystal
    }


    /// tokens will work very differently depending on implementation,
    /// hence an interface
    // all the things that make a token unique in the interface
    // for instance there can be a section that displays tokens
    public interface IToken {

    }

}