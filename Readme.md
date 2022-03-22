# VRChat Joke Jam

This project was created for a [10-Day World Jam](https://vrch.at/jokejam), and serves as a fun starter kit for a Joke world!

### Visit
The example world in VRChat here: [Demo World](https://vrchat.com/home/world/wrld_97b1a699-c924-46d8-80ee-2d9cc1f1a501).

### Download
A ready-to-use project here: [VRChat JokeJam Project](https://github.com/vrchat-community/VRChat-Joke-Jam/releases/tag/1.0.0).

For advanced users, you can fork [GitHub project](https://github.com/vrchat-community/VRChat-Joke-Jam) to easily stay up-to-date with any updates or fixes.

---

# Getting Started
Download the latest version and open the 'Assets/_WorldJam3/Scenes/DemoScene' scene. You can simply press the Play button to explore the scene and interactions using ClientSim!

## Things to try:
* Grab the Laser Pointer, then use it over by the [LaserCat](https://docs.vrchat.com/docs/lasercat).
* Walk over the [Jack in the Box](https://docs.vrchat.com/docs/jack-in-the-box) and click on it.
* Grab the big Cartoon Hammer, then press 'Escape' to bring up the ClientSim menu. Press 'Spawn Remote Player' and move the hammer around to ["Bop" them on the head](https://docs.vrchat.com/docs/bop-system). Watch the Bop Counter in the scene increase!

## ClientSim
The VRChat Client Simulator, aka [ClientSim](https://github.com/vrchat-community/ClientSim), is the easiest way to quickly test an Udon scene in Unity Editor Play mode. It _simulates_ what it's like to be in the world so it's not 100% the same as how things will work in VRChat, but it gives you the ability to move around the scene, interact with UdonBehaviours, Pickups, UI and other Interactable items, spawn Remote players and even test out network sync functionality. Once you have things working the way you'd like, use [Build & Test](doc:using-build-test) in the VRChat Control Panel to try it out in the VRChat network client to make sure it all still works as expected.

## UdonSharp / Udon Graph
This project uses [UdonSharp](https://github.com/vrchat-community/UdonSharp) for all of its scripting. With UdonSharp (also known as U#), you can write programs using C# syntax, and are turned into UdonAssembly programs to run in VRChat.

You can make Udon Graph programs that work alongside U# programs if the Graph is your jam. For this example, we've made these programs in Udon Graph as well:
* BopReceiver
* BopSender
* BopTracker

# Custom Prefabs
We've included some fun prefabs you can learn from and modify:

* [Bop System](https://docs.vrchat.com/docs/bop-system)
* [Jack in the Box](https://docs.vrchat.com/docs/jack-in-the-box)
* [LaserCat](https://docs.vrchat.com/docs/lasercat)

# Additional Systems
These scripts are easy to use independently of the Joke Jam demos:
* [Spring](https://docs.vrchat.com/docs/spring)
* [SimpleEyeLook](https://docs.vrchat.com/docs/simpleeyelook)
* [Helper Classes](https://docs.vrchat.com/docs/helper-classes)