# AdHawk Eye Tracking Quest2 SDK + Examples

AdHawk Microsystems template project for our Unity Quest 2 Eyetracking development kit

A more detailed walkthrough can be found in the getting started pdf guide. If you are experienced using Unity this readme should be sufficient.

This project is designed to run on a computer running Windows 64bit with a discrete GPU that supports running VR applications on the Quest 2 Headset.

---

## Resources

- [A general Environment/Project setup guide](https://adhawkmicrosystems.github.io/unity/HtN_EnvironmentSetup_UnityVR.html)
- [VR development and eyetracking guide](https://adhawkmicrosystems.github.io/unity/HtN_GettingStarted_UnityVR.html)
- [Adhawk SDK](https://adhawkmicrosystems.github.io/api/api.html)

## Requirements

1. We recommend you install Unity Hub
1. Make sure to install Unity version 2021.1.21f1 with Windows Build Support. You can find this version of unity:
    * By [clicking here](unityhub://2021.1.21f1/f2d5d3c59f8c) if you have Unity Hub installed
    * By going to the [Unity Download Archive](https://unity3d.com/get-unity/download/archive#:~:text=Unity%20Hub-,Unity%202021.1.21,-15%20Sep%2C%202021)
1. The project should have all package manager dependencies setup appropriately. When opening the project for the first time Unity may grab the requirements automatically.

## Getting Started

The first thing you should do upon setting up your project is to run a build and try it out on the Quest 2 Headset while the headset is in Oculus Link Mode.

There are three main recommended scenes that should be added to build-settings in this index order:

0. Intro Scene
    - This scene should be modified with some intro and acts as a general "Press any button to start". This should allow the local backend eyetracking communication server to connect to the headset and start communicating with the application
1. Calibration Scene
    - As a rule of thumb, each time someone puts on the headset, you should run a calibration. Moving the headset too much—such as taking it off and putting it on—will change the position of the eye tracker relative to the eyes. When this happens, a calibration will fix things up.
2. Your App Here
    - This is where you shine. But keep in mind that there should likely be a way to explicitly trigger a calibration within the application in case eye tracking isn't great. By allowing the user to calibration from any point in your application, you allow them to retain their progress.
