# TomoAIO - Living the Dream Tool (v2.0)
<img width="1412" height="813" alt="TomoAIO_mOLCteojqa" src="https://github.com/user-attachments/assets/8bc4762d-5256-4c6f-942c-efe99a7500f9" />
Welcome to **TomoAIO**, the ultimate All-In-One save editing, island management, and modding toolkit for Tomodachi Life: Living the Dream. 

Whether you want to skip the grind and unlock all map facilities, share custom Mii identities with your friends, or inject your own custom textures into the game, TomoAIO handles all the complex hex-editing, texture swizzling, and Zstandard compression for you safely behind a clean UI.

## ✨ Features

* **👑 NEW: Island Management:** Take total control of your save file. Instantly edit your Island Funds, or use the powerful one-click unlockers to get all Quik Builds (Map Facilities), Room Interiors, Clothes, and Food.
* **⚡ NEW: Smart First-Time Setup:** No more repetitive folder browsing! TomoAIO now guides you through setup on your first boot, validates your files, and remembers your paths automatically.
* **Mii Import & Export (.ltd files):** Easily share your custom Miis! The tool extracts and packs DNA, Personality, Voice, Birthday, Pronouns, Style, and Sexuality bits perfectly.
* **Automatic Facepaint / DNA Triggers:** When importing a Mii with custom facepaint or makeup, the tool automatically registers the `faceID` in your `Player.sav` file and injects the necessary canvas data without overflowing the registry limit.
* **UGC Creator (Texture Injector):** Swap out in-game items (like food, drinks, or props) with your own custom images (PNG/JPG). 
* **Color Correction:** Automatically swizzles standard linear images and fixes RGBA/BGRA channel swaps so your textures look perfect in-game with no "slanted" or "neon" graphical glitches.
* **Auto-Backups:** Never lose a save file! Every time you import a Mii, the tool automatically creates a timestamped backup of your `Mii.sav`, `Player.sav`, and `Ugc` folder.

---

## 📥 Installation

1. Go to the **Releases** tab on the right side of this GitHub page.
2. Download the latest `TomoAIO_v2.x.zip` file.
3. Extract the `.zip` file to an empty folder on your PC.
4. **Important:** Ensure `TomoAIO.exe` and `ZstdSharp.dll` are kept in the exact same folder. If you separate them, the UGC Creator will crash!

---

## 🎮 How to Use

### 1. First-Time Setup
On your very first launch, TomoAIO will prompt you to locate your Main Save Folder and your UGC Folder. It validates and saves these paths so you never have to select them again!
* *Need to edit a different island? Just click **Change Save Folders** on the main menu to reset your paths.*

### 2. Island Management
* Click the **Island Management** button on the main menu.
* **To Edit Money:** Type in your desired bank balance and click Save.
* **To Unlock Items:** Click any of the four unlock buttons to instantly complete your collections. The Quik Build unlocker safely triggers the proper event flags for all 77 map locations!

### 3. Mii Importer / Exporter
* Click the **Mii Import** button on the main menu.
* **To Export:** Select a Mii from the list on the left, change the dropdown action to **Export**, and click **Go**. Save the `.ltd` file anywhere on your PC.
* **To Import:** 1. Select the slot in the list you want to overwrite.
  2. Change the dropdown action to **Import**.
  3. Drag and drop a `.ltd` Mii file into the text box (or click Browse to find one).
  4. Click **Go**. The tool will inject the DNA and safely register any custom facepaint!

### 4. UGC Creator (Custom Items)
* Click the **UGC Creator** button on the main menu to open the texture dashboard.
* Select an item from the list (e.g., `UgcFood001.canvas.zs`). The tool will automatically unpack it and show you a preview.
* Click **Import** and select your custom PNG or JPG.
* *Note: Your custom image should preferably be 256x256. The tool will resize your attached image, but if it looks squished, you can fix it in-game at the Palette House.*

---

## ⚠️ Safety & Backups

Modding save files can sometimes be risky. TomoAIO includes an automatic failsafe. Every time you hit the "Go" button to import a Mii, the tool takes a snapshot of your files and places them inside a `backup` folder located right next to `TomoAIO.exe`. 

If your game ever fails to load, simply open the `backup` folder, find the most recent timestamp, and copy `Mii.sav` and `Player.sav` back to your Switch/Emulator.

---

## 💬 Community

Need help, have questions, or want to share the custom Miis and UGC items you've made? 
Join the community Discord: [https://discord.gg/jhdupweGw7](https://discord.gg/jhdupweGw7)

## 📜 Credits

This project utilizes the following open-source libraries and community research:

* **[ZstdSharp.Port](https://github.com/mkoloschak/ZstdSharp)** - Used for Zstandard compression and decompression.
* **[AssetRipper.TextureDecoder](https://github.com/AssetRipper/TextureDecoder)** - Used for handling game texture formats.
* **[astcenc](https://github.com/ARM-software/astc-encoder)** - Utilized for decoding ASTC compressed textures via command-line interface.
* **[ShareMii](https://github.com/Star-F0rce/ShareMii)** - For the original Mii data structures and `.ltd` file format inspiration.
* **[Living the Dream Toolkit (MadMax1960)](https://github.com/MadMax1960/LivingTheDreamToolkit)** - Huge thanks for the under-the-hood UGC Editor logic and Texture Processing that makes custom texture injections work flawlessly.
* **[rafacasari's Save Editor](https://github.com/Rafacasari/ltd-save-editor)** - A massive shoutout for their open-source work on reverse-engineering the save files. The interior and map facility hashes used in the TomoAIO Island Management were referenced from their tool!
