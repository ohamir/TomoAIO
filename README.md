# TomoAIO - Living the Dream Tool (v1.2)
<img width="1412" height="813" alt="TomoAIO_UD7HUpK06q" src="https://github.com/user-attachments/assets/e483f3ba-0d27-45a7-aba8-9aefd9ce7222" />


Welcome to **TomoAIO**, the ultimate All-In-One save editing and modding toolkit for Tomodachi Life: Living the Dream Mii data and User Generated Content (UGC). 

Whether you want to share custom Mii identities with your friends or inject your own custom textures into the game, TomoAIO handles all the complex hex-editing, texture swizzling, and Zstandard compression for you safely behind a clean UI.

## ✨ Features

* **Mii Import & Export (.ltd files):** Easily share your custom Miis! The tool extracts and packs DNA, Personality, Voice, Birthday, Pronouns, Style, and Sexuality bits perfectly.
* **Automatic Facepaint / DNA Triggers:** When importing a Mii with custom facepaint or makeup, the tool automatically registers the `faceID` in your `Player.sav` file and injects the necessary canvas data without overflowing the registry limit.
* **UGC Creator (Texture Injector):** Swap out in-game items (like food, drinks, or props) with your own custom images (PNG/JPG). 
* **Color Correction:** Automatically swizzles standard linear images and fixes RGBA/BGRA channel swaps so your textures look perfect in-game with no "slanted" or "neon" graphical glitches.
* **Auto-Backups:** Never lose a save file! Every time you import a Mii, the tool automatically creates a timestamped backup of your `Mii.sav`, `Player.sav`, and `Ugc` folder.

---

## 📥 Installation

1. Go to the **Releases** tab on the right side of this GitHub page.
2. Download the latest `TomoAIO_v1.x.zip` file.
3. Extract the `.zip` file to an empty folder on your PC.
4. **Important:** Ensure `TomoAIO.exe` and `ZstdSharp.dll` are kept in the exact same folder. If you separate them, the UGC Creator will crash!

---

## 🎮 How to Use

### 1. Opening your Save Data
Before you can edit Miis, you need to point the tool to your save data.
* Open `TomoAIO.exe`.
* Click the **Mii Import** button on the main menu.
* Click **Load Save Folder** in the top right.
* Browse to the folder containing your `Mii.sav` and `Player.sav` files and select it. The tool will automatically read your Mii slots.

### 2. Mii Importer / Exporter
* **To Export:** Select a Mii from the list on the left, change the dropdown action to **Export**, and click **Go**. Save the `.ltd` file anywhere on your PC.
* **To Import:** 1. Select the slot in the list you want to overwrite.
  2. Change the dropdown action to **Import**.
  3. Drag and drop a `.ltd` Mii file into the text box (or click Browse to find one).
  4. Click **Go**. The tool will inject the DNA and safely register any custom facepaint!

### 3. UGC Creator (Custom Items)
* Click the **UGC Creator** button on the main menu to open the texture dashboard.
* Click the **Load Ugc Folder** button to link the tool to your mod's `Ugc` folder.
* Select an item from the list (e.g., `UgcFood001.canvas.zs`). The tool will automatically unpack it and show you a 384x384 preview.
* Click **Import** and select your custom PNG or JPG.
* *Note: Your custom image is preferably 256x256, the tool will resize your attached image but it might look squished, if so you can fix it in game at the Pallete House*
---

## ⚠️ Safety & Backups

Modding save files can sometimes be risky. TomoAIO includes an automatic failsafe. Every time you hit the "Go" button to import a Mii, the tool takes a snapshot of your files and places them inside a `backup` folder located right next to `TomoAIO.exe`. 

If your game ever fails to load, simply open the `backup` folder, find the most recent timestamp, and copy `Mii.sav` and `Player.sav` back to your Switch.

---

## 💬 Community

Need help, have questions, or want to share the custom Miis and UGC items you've made? 
Join the community Discord https://discord.gg/jhdupweGw7
