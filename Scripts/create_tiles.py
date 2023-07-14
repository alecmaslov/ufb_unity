from PIL import Image
import argparse
import os

ALPHABETICAL_TILE_NAMES = [x for x in "ABCDEFGHIJKLMNOPQRSTUVWXYZ"]

def get_tiles_outpath(map_name):
    return f"../Assets/Resources/Maps/{map_name}/Tiles"

def query_tile(letter, number):
    tiles_basepath = get_tiles_outpath("kraken")
    tile_name = f"tile_{letter}_{number}.png"
    tile_path = os.path.join(tiles_basepath, tile_name)
    print(f"Loading tile {tile_name} from {tile_path}")
    tile = Image.open(tile_path)
    return tile

def split_into_tiles(image, map_name, tile_size=256, num_tiles=26):
    image_width, image_height = image.size
    tile_width = image_width // num_tiles
    tile_height = image_height // num_tiles

    print(f"Tile size: {tile_width} x {tile_height}")

    tiles_basepath = get_tiles_outpath(map_name)
    if not os.path.exists(tiles_basepath):
        os.makedirs(tiles_basepath)

    num_tile = 0
    for y in range(num_tiles):
        for x in range(num_tiles):
            tile = image.crop((x*tile_width, y*tile_height, (x+1)*tile_width, (y+1)*tile_height))
            row_name = x
            col_name = ALPHABETICAL_TILE_NAMES[y]
            outfile = os.path.join(tiles_basepath, f"tile_{col_name}_{row_name}.png")
            tile = tile.resize((tile_size, tile_size))
            tile.save(outfile)
            num_tile += 1
            print(f"\rCreated tile {col_name}{row_name} | {num_tile} of {num_tiles**2}", end="")

if __name__ == "__main__":
    args = argparse.ArgumentParser()
    args.add_argument("--tile_size", type=int, default=256)
    args.add_argument("--num_tiles", type=int, default=26)
    args.add_argument("--map_name", type=str, default="kraken")
    args.add_argument("--map_file", type=str, default="../Assets/Resources/Images/Map__KrakenCropped.png")

    args = args.parse_args()

    image_size = (5642, 5642) # this divides by 26 to 217px, so we have no fractional pixels
    image = Image.open(args.map_file)
    image = image.resize(image_size)

    split_into_tiles(image, args.map_name, args.tile_size, args.num_tiles)
    print(f"Wrote tiles to {get_tiles_outpath(args.map_name)}")