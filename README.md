This is the term project for COMPSCI 576 by Ryan Fletcher, Kevin Chen, and Eric Stevens

If you're not launching in the main menu when you start the game, or if clicking "Start Maze" doesn't bring you to the maze, add both the main menu scene and the SampleScene to the build path in File>>Build Settings, load the Main Menu scene, and unload the SampleScene scene before launching the game.

Individual Contributions:

Ryan:
    Wrote all of MazeGenerator.cs (~460 lines)
        (Small contributions from Eric)
    Wrote most of MazeRenderer.cs (~630 lines)
        Kevin wrote much of createEnemy(), the healthbar, and the NavMesh stuff
        Kevin fixed a bug with the bounds sizing
    Wrote PickUp.cs (76 lines)
    In EnemyPatrollerController: (25 lines)
        Lines 14-22
        Lines 68-80
        5 lines in onCollisionEnter
    In EnemyHunterController: (31 lines)
        Lines 15-24
        Line 39
        Lines 51-58
        Lines 66-75
        5 lines in OnCollisionEnter
    In EnemyChaserController.cs: (31 lines)
        Lines 15-24
        Line 41
        Lines 53-60
        Lines 68-77
        5 lines in OnCollisionEnter
    Wrote first version of Inventory.cs (31 lines)
        Heavily modified by Kevin later
    Wrote Minimap.cs (182 lines)
    Wrote PoofTrap.cs (52 lines)
    Added all the enemy sounds, pickup sound, teleport sound
    Added all the fonts
    Hand-drew the victory prize image

Kevin:
    Wrote ~50 out of 70 lines of MazeRenderer.createEnemy()
    Wrote the healthbar and MavMesh code in MazeRenderer (~15 lines)
    Added default CharacterController

Eric:
    Small contribution to MazeGenerator.cs (Menu parameters lines 394 - 397).
    Wrote all of menuScript.cs.
    Wrote all of menuParamsScript.cs.
    CanvasManager.cs lines 54-69, 101-107.
    chest.cs lines 92-94.
    EnemyChaserController.cs lines 91-94, 101-104, 131-135.
    EnemyHunterController.cs lines 97-100, 107-110, 140-143.
    EnemyPatrollerController.cs lines 58-60, 93-96.
    MazeRenderer lines 278-281, 296-299, 314-317.
    Wrote all of CharacterControllerScript.cs, (not used after switching to first person game).
    Implemented models for all enemies.
    Implemented animations for all enemies.
    Designed and implemented all of the start menu, pause menu, victory menu, and loss menu.
