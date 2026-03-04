# About
This repository is an attempt to add a modding framework to the game "Holder of Place". 
It consists of two parts: a DLLeditor that injects entry points into the source code and code that uses these entry points to establish a DLL reading framework.
The setup will require a bit of technical know-how, but the installation below should smooth out the rough patches.

# Installation [WORK IN PROGRESS]
## Step 0: Prerequisite Downloads
There are a few files and programs that you need to install first. They come from pretty standard websites.

---

### Visual Studios (or similar C# IDE)
1. For maximum simplicity, download and install visual studios (VS): https://visualstudio.microsoft.com. The default free version is Visual Studios 2026 Community. This project was made in an older version of VS, but as long as it can run .Net Frmaework 4.8.1, it should work fine.

<p align=center>
<img width="523" height="297" alt="image" src="https://github.com/user-attachments/assets/4384a5a5-b428-4847-95f7-52c6a9fd4192" />
</p>

2. Run the installer. It will ask you what workload you want to install. You must install "ASP.Net and Web Development" or whatever looks the most similar to that. If you want to work on mods, installing "Game Development with Unity" would be nice but not required.

<p align=center>
<img width="425" height="138" alt="image" src="https://github.com/user-attachments/assets/e4e9d6d0-5445-427f-8f77-f9836434dccf" />
<img width="424" height="144" alt="image" src="https://github.com/user-attachments/assets/438fabae-cdd9-4ab9-926f-b7473f4b2264" />
</p>
  
4. Start the installation. You can finish the rest of steps 0-1 as you wait for the installation to finish.

---

### ModBootstrap and Harmony (w/Dependencies)

We will require a few C# libraries that will not be packaged in with Visual Studios. [PAST ME: please add the dll file in this repo] There should be a folder in this repository marked "dlls". These are all the external dlls that you need to run this. If this is your first time trying to do something like this, this may seem a bit sketchy. Here is a summary of all of the dlls and where you can read up on or cross-validate them.
- `ModBootstrap.dll`: This will be the central library for modding Holder of Place. It will start Harmony, interface with the source code, and load mods. It is created by me, and the code is accessible in this very repository.
  - Official github: You are here.
- `0Harmony.dll`: This is the harmony library, created by Andreas Pardeike. In the words of its GitHub, it is "a library for patching, replacing and decorating .NET and Mono methods during reuntime". This is the library that will allow modders to modify the source code in a reversible way.
  - Official Github: https://github.com/pardeike/Harmony
  - API documentation: https://harmony.pardeike.net/articles/patching.html
  - This dll was obtained on the Unity Explorer repository (Standalone version): https://github.com/sinai-dev/UnityExplorer
- `MonoMod.*`:  An open-source library to help with C# modding. Harmony uses it as a dependency.
  - Official Github: https://github.com/MonoMod
  - These dlls were obtained on the Unity Explorer repository (Standalone): https://github.com/sinai-dev/UnityExplorer
- `Mono.Cecil.*`: Mono.Cecil are libraries with the purpose of modifying dlls. They are used by MonoMod.
  - Official website: https://www.mono-project.com/docs/tools+libraries/libraries/Mono.Cecil
  - These dlls were obtained from the Unity Explorer repository (Standalone): https://github.com/sinai-dev/UnityExplorer

Download these dlls either from this repository or another source.

---

### StreamingAssets

In this repository, there is a folder named `StreamingAssets`. You can browse its contents and find only a single 100x100 image. This will be the folder that the modding framework will check for mods (among other things) and save configs, but right now, it's essentially empty.

Once you have that downloaded, you are ready for the next step.

---

## Step 1: Locating and Moving Files [IN PROGRESS]
First, find where the game's source code is saved on your computer. 
- The easiest way to do this is right-click the game in steam and select "Manage > Browse Local Files".
- For the folder that just popped up, click "HolderOfPlace_Data" and then "Managed".
- inside the "Managed" folder is a bunch of dll files.
- You need to (1) make a copy of "AssemblyCSharp.dll" somewhere else (I copied it onto my desktop) and (2) save the file address of the new dll location and the address of the managed folder.
- Place the Harmony dll and its dependency dlls into the managed folder.

## Step 2: Running the Code [IN PROGRESS]
The repository has the code to add entry points to the game's source code. Distributing the modified source code would put me in a legal bind, but running the code on your copy of the source code should be fine.
- Download the repository as a zip folder and unpack it.
- Open the .sln file in Visual Studios
- There are three file addresses at the top of the DLLEditor.cls file. Change them to your version of the three.
- Run the DLLEditor file (there should be a green play button somewhere at the top). This wil be the step where your computer and antivirus is warning you (if not before).
You have the code in front of you, you can read through to see if there is anything questionable with it.
- You should get an output that looks like this. If you did, good job!

## Step 3: Run the Game [IN PROGRESS]
Run the game and you should see a debug log pop-up. That means everything is running smoothly.
