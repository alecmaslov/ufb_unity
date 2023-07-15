using UFB.Entities;


// something that can be spawned onto a GameBoard
public interface ISpawnable 
{
    TileAttachable TileAttachable { get; }
    void Spawn(TileEntity tile);
    void Despawn();
}