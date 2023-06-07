using UnityEngine;
using System.Collections;

enum TokenType {
    Melee,
    Magic,
    EnergyShard,
    EnergyCrystal,
    HeartCrystal
}


/// tokens will work very differently depending on implementation,
/// hence an interface
public interface IToken {

}