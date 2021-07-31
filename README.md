# MHTri Server
Monster Hunter Tri Custom Server Implementation
## *HELP WANTED*

Current Status
--------------
![Progress](https://i.imgur.com/eRaTuhu.gif)

As you can see the project is not yet functional. We are stuck at Spawning the player at the gate.

Todo:
- [x] Bypass Nintendo Servers
- [x] Capcom ID selection:
- [x] Server selection:
- [ ] Time system:
  - [X] Send specific time
  - [ ] Simulate day/night cycle
  - [x] Sandstorm
- [ ] Gate:
  - [X] Gate Selection
  - [ ] Spawn At Gate
- [ ] City 
  - [ ] Create a city
  - [ ] Join a city
  - [ ] Search a city
- [ ] Quest
  - [ ] Submit a quest
  - [ ] Join a quest
  - [ ] Start a quest
  - [ ] Event Quest
  - [ ] Arena Quest
- [ ] Chat

Prerequisites
-------------
 * Monster Hunter 3 (~tri) w/ Nintendo servers patch
 * Nintendo Wi-Fi Connection Server (wiimmfi, altwfc, etc...)
 * [Dolphin Emulator](https://github.com/dolphin-emu/dolphin)
 * C# Compiler (Net Core 3.1)

Monster Hunter 3 (~tri)
-----------------------
In order to use custom servers on Monster Hunter Tri you first need to patch the game to bypass the Nintendo servers. Their servers are blocking the game and prevent it to connect to Capcom servers. I **strongly recommend** that you use [altwfc](https://github.com/polaris-/dwc_network_server_emulator) for that, but any other alternative should work as well.

1. **Patch the game**
   * Download the **wiimmfi-patcher** from the "*How-To*" section.
   * Carefully **read** the patcher's *README*.
   * **Run the patcher** corresponding to your OS.
   * Launch your **patched game**.
2. **DNS Redirection**
   * Rather than using a DNS server, edit the **hosts file** in your system.
      * **Windows** Location: ```%SystemRoot%\system32\drivers\etc\hosts```
      * **Mac OS X** Location: ```/private/etc/hosts```
      * **Linux** Location: ```/etc/hosts```

Your host file should include:
```
127.0.0.1 mh.capcom.co.jp
``` 
If you are using a local Nintendo Wifi Connection Server and patched the game with the wimmfi patcher also include:
```
127.0.0.1 naswii.wiimmfi.de
127.0.0.1 nas.wiimmfi.de
127.0.0.1 gamestats.gs.wiimmfi.de
127.0.0.1 gamestats2.gs.wiimmfi.de
127.0.0.1 wiinat.available.gs.wiimmfi.de
127.0.0.1 wiinat.natneg1.gs.wiimmfi.de
127.0.0.1 wiinat.natneg2.gs.wiimmfi.de
127.0.0.1 wiinat.natneg3.gs.wiimmfi.de
127.0.0.1 gpcm.gs.wiimmfi.de
127.0.0.1 gpsp.gs.wiimmfi.de 
```
3. **SSL Certificate**
   * In order for the game to communicate with the first server (port 8200) correctly we need a valid SSL Certificate, **but** we can bypass that by using [Dolphin Wii Emulator](https://github.com/dolphin-emu/dolphin). In order to disable SSL Ceritifcate verification you need to:
     * Options
       * Configuration
         * *Tick* Show Debugging UI
     * Open Network Pane
       * *Untick* Verify Certificates (Only leave it untick if you want to test the server)

Servers
-----------
Open the solution file, then **Build** *->* **Run** the solution to start the server. When a client connect, you would see all the packet being logged to the console. You would also need the Nintendo Wifi Connection server running.

Special Thanks
-----------
- [Sepalani](https://github.com/sepalani)
- [Dolphin Emulator Team](https://github.com/orgs/dolphin-emu/people)
- [Altwfc](https://github.com/barronwaffles/dwc_network_server_emulator)
- [Wimmfi](https://wiimmfi.de/)