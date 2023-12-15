This is the term project for COMPSCI 576 by Ryan Fletcher, Kevin Chen, and Eric Stevens
Link to the GitHub page: https://github.com/NephilimGreen/576-final-project

Before starting the game, make sure MainMenu scene is loaded and SampleScene scene is either not loaded or unloaded. Maximize the gameplay screen.
If something is broken, make sure both the main menu scene and the SampleScene are in the build path in File>>Build Settings.

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
    Wrote all of Equation.cs (~140 lines)
    Wrote ~50 out of 70 lines of MazeRenderer.createEnemy()
    Wrote renderHealthBar() function in MazeRenderer.cs (lines 622-627)
    Baked each floor's NavMeshSurface and placed NavMeshAgents in MazeRenderer.cs (lines 561-568)
    Imported RigidBodyFirstPersonController from Assignment 5
    Wrote all of EnemyUtility.cs (~105 lines)
    Implemented behavior logic for all enemies:
        MoveToRandomPoint() coroutine (lines 109-122) and most of SlowUpdate() function (lines 80-107) in EnemyChaserController.cs
        Most of MoveToPatrolPoints() coroutine (lines 50-66) in EnemyPatrollerController.cs
        MoveToRandomPoint() coroutine (lines 115-127) and most of SlowUpdate() function (lines 78-113) in EnemyHunterController.cs
        Sounds for the above 3 scripts were added by Ryan and animations were added by Eric
    Wrote pausing functionality in CanvasManager.cs (lines 39-52, 70-91)
    Wrote all of DraggableItem.cs (~35 lines) and ItemSlot.cs (~33 lines)
        Followed Youtube tutorial: https://www.youtube.com/watch?v=kWRyZ3hb1Vc
        Created UI GameObject hierarchy for hotbar, chest inventory, and equation solving menus
    Wrote all of Chest.cs (~100 lines)
    Rewrote Ryan's Inventory.cs to implement a hotbar with draggable item movement (lines 22-59)
    Wrote all of PickUpAnimation.cs (~30 lines)
    Wrote all of PoofTrapAnimation.cs (~30 lines)
    Added win/lose sounds, and sounds for dragging items around in inventory

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
