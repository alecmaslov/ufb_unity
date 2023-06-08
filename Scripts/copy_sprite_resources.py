import glob
import shutil
import os

output_dir = "../Assets/Resources/Tiles/"
all_files = glob.glob("../Assets/Textures/TileSprites/**/*.png", recursive=True)

if not os.path.exists(output_dir):
    os.makedirs(output_dir)

for file in all_files:
    filename = os.path.basename(file)
    category = filename.split("_")[0]

    if not os.path.exists(os.path.join(output_dir, category)):
        os.makedirs(os.path.join(output_dir, category))
    
    # copy the file
    print(f'Copying {file} to {os.path.join(output_dir, category, filename)}')
    shutil.copy(file, os.path.join(output_dir, category, filename))