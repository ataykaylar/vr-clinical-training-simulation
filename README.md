# PPH VR Training Simulation

A research-oriented virtual reality simulation for postpartum hemorrhage (PPH) training and experimental evaluation.

The project is designed to provide an immersive clinical environment where participants can interact with a virtual patient, observe changing vital signs, and complete a PPH-related scenario under configurable experimental conditions.

> **Project status:** Under active development. The experiment setup and participant database are implemented, while the complete clinical scenario, performance evaluation, and event-recording systems are still being developed.

## Project Overview

The application begins with an experiment setup screen where the researcher enters participant information and selects the experimental conditions.

After the configuration is saved, the application:

1. Creates a new experiment record in a local SQLite database.
2. Stores the selected participant and experiment settings.
3. Loads the VR simulation scene.
4. Makes the experiment configuration available during the simulation.

The simulation currently includes a virtual clinical environment, patient characters, a patient monitor, simulated vital signs, and XR interaction components.

## Current Features

* Experiment setup interface
* Participant ID and age collection
* Clinical-experience selection
* Previous VR-experience selection
* Scenario-mode selection
* Guidance on/off configuration
* Immediate feedback on/off configuration
* Local SQLite experiment database
* Automatic UTC experiment start timestamp
* Persistent experiment record ID
* VR scene transition after successful database saving
* Simulated patient vital signs
* Patient condition states
* OpenXR-based VR support
* XR controller and interaction support
* XR Device Simulator support for testing without a headset

## Technologies

* **Unity:** 6000.2.2f1
* **Universal Render Pipeline:** 17.2.0
* **XR Interaction Toolkit:** 3.2.1
* **OpenXR Plugin:** 1.15.1
* **XR Plugin Management:** 4.5.1
* **Unity Input System:** 1.14.2
* **TextMesh Pro**
* **SQLite-net for Unity:** 1.3.2
* **C#**

## Project Structure

```text
Assets/
├── Project/
│   ├── Art/
│   │   ├── 2D_Elements/
│   │   ├── 3D_Animations/
│   │   ├── 3D_Models/
│   │   │   ├── Characters/
│   │   │   └── Environments/
│   │   ├── Archive/
│   │   └── Skymaps/
│   │
│   ├── Prefabs/
│   │   ├── Characters/
│   │   ├── Environment/
│   │   ├── Medical/
│   │   ├── Interaction/
│   │   ├── UI/
│   │   └── XR/
│   │
│   ├── Scenes/
│   │   ├── Setup/
│   │   ├── Simulation/
│   │   └── Archive/
│   │
│   ├── Scripts/
│   │   ├── Core/
│   │   ├── Experiment/
│   │   ├── Interaction/
│   │   ├── Patient/
│   │   └── UI/
│   │
│   └── Settings/
│       ├── Lighting/
│       └── Rendering/
│
├── Resources/
├── Samples/
├── TextMesh Pro/
├── XR/
├── XRI/
└── XRI_Examples/
```

## Main Scenes

### ExperimentSetup

The initial scene used to configure the experiment.

It collects:

* Participant ID
* Participant age
* Clinical experience
* Previous VR experience
* Scenario mode
* Guidance preference
* Immediate feedback preference

The entered data is validated and saved before the VR simulation begins.

### VR-Nurse_Training_Simulation

The main virtual reality simulation scene.

It currently contains:

* The clinical environment
* Virtual patient characters
* Patient monitor
* Simulated vital signs
* XR Origin and controllers
* Interaction components
* Scene-specific lighting data
* Reflection probes and baked lightmaps

## Experiment Data

Each experiment currently stores the following fields:

| Field                      | Description                                |
| -------------------------- | ------------------------------------------ |
| `Id`                       | Automatically generated database record ID |
| `ParticipantID`            | Research participant identifier            |
| `Age`                      | Participant age                            |
| `ClinicalExperience`       | Selected level of clinical experience      |
| `PreviousVRExperience`     | Selected level of previous VR experience   |
| `ScenarioMode`             | Selected experimental scenario mode        |
| `GuidanceEnabled`          | Whether guidance is enabled                |
| `ImmediateFeedbackEnabled` | Whether immediate feedback is enabled      |
| `StartedAtUtc`             | Experiment start time in UTC               |

The data is stored in:

```text
pph_experiments.db
```

The database file is created inside Unity’s:

```csharp
Application.persistentDataPath
```

The exact location depends on the operating system and build platform.

## Opening the Project

1. Clone or download the repository.

```bash
git clone <repository-url>
```

2. Open Unity Hub.

3. Select **Add project from disk**.

4. Select the project folder.

5. Open the project with Unity `6000.2.2f1`.

6. Allow Unity to restore and import the required packages.

7. Wait until asset importing and script compilation are complete.

8. Check the Console for errors before entering Play Mode.

## Scene Configuration

The scenes should be included in the build in this order:

```text
0 - Assets/Project/Scenes/Setup/ExperimentSetup.unity
1 - Assets/Project/Scenes/Simulation/VR-Nurse_Training_Simulation.unity
```

The simulation scene is loaded using:

```csharp
SceneManager.LoadScene("VR-Nurse_Training_Simulation");
```

The scene filename must therefore remain `VR-Nurse_Training_Simulation.unity` unless the corresponding code is also updated.

## Running the Experiment

1. Open the `ExperimentSetup` scene.
2. Enter a participant ID.
3. Enter a valid age.
4. Complete all dropdown selections.
5. Configure guidance and immediate feedback.
6. Start the experiment.
7. Confirm that the experiment record is saved.
8. Confirm that the VR simulation scene loads successfully.

## Testing Without a VR Headset

The project includes Unity’s XR Device Simulator sample.

It can be used to test:

* Head movement
* Controller movement
* Controller rotation
* XR interaction
* UI interaction
* Basic scene navigation

A physical OpenXR-compatible headset is recommended for final interaction and usability testing.

## Current Limitations

* The complete PPH scenario progression is not yet implemented.
* Guided and unguided scenario behaviours are still under development.
* The current database stores experiment setup data but not the full simulation performance.
* Intervention events are not yet recorded.
* Experiment completion time is not yet stored.
* Error counts and clinical performance scores are not yet calculated.
* Some imported XR sample assets are still referenced by the simulation.
* Some archived assets are retained until all scene dependencies are verified.

## Planned Improvements

* Replace integer-based scenario steps with a state-driven scenario system
* Implement guided and unguided scenario modes
* Record participant actions as timestamped experiment events
* Record correct and incorrect interventions
* Measure time spent on each scenario step
* Save experiment completion time and completion status
* Add clinical performance scoring
* Add visible validation and database error messages
* Improve patient-condition transitions
* Decouple the patient monitor from the main game manager
* Reduce dependencies on unused XR sample content
* Add Edit Mode and Play Mode tests
* Improve VR performance and asset optimization
* Prepare a standardized research export format

## Research and Clinical Disclaimer

This project is intended for research, education, and simulation-development purposes.

It is not a certified medical device and should not be used as a substitute for professional clinical education, institutional protocols, supervised simulation training, or medical decision-making.

## Third-Party Assets

The project includes or depends on third-party packages and assets, including:

* Unity XR Interaction Toolkit samples
* TextMesh Pro
* OpenXR
* SQLite-net for Unity
* AllSky Free
* Real-Time Procedural Cable Simple

All third-party assets remain subject to their respective licenses. Their inclusion in this repository should be reviewed before public distribution.

## Version Control

Because the project contains large binary assets such as models, textures, lightmaps, and reflection probes, Git LFS is recommended.

Typical Git LFS file types include:

```text
*.fbx
*.png
*.jpg
*.psd
*.tif
*.exr
*.wav
*.mp3
*.ogg
```

Unity-generated folders such as `Library`, `Temp`, `Logs`, `obj`, and local build outputs should not be committed to the repository.

## License

No project-specific open-source license has been assigned yet.

Third-party packages and assets remain governed by their original licenses. Review all licensing requirements before sharing, publishing, or redistributing the complete Unity project.
