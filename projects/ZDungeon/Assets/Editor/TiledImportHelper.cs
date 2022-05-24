using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class TiledImportHelper
{
    // Data:
    private class ImportCommand
    {
        public string function = "";
        public List<string> parameters = new List<String>();
        public int linkParamter = int.MaxValue;
        public Transform target = null;

        public ImportCommand(string function, Transform target)
        {
            this.function = function;
            this.target = target;
        }
    }


	[MenuItem("Tools/Import Tiled2Unity %&i")]
	public static void TiledImport()
	{
        // Data
        List<ImportCommand> commandList = new List<ImportCommand>();
        Transform Tilemap = null;
        int errorCount = 0;
        bool hadError = false;
        UnityEngine.SceneManagement.Scene currScene = 
            UnityEngine.SceneManagement.SceneManager.GetActiveScene ();
        GameObject[] rootObjects = currScene.GetRootGameObjects ();
        foreach (GameObject rootobj in rootObjects)
        {
            if (rootobj.name == "Test_Dungeon")
            {
                Tilemap = rootobj.transform;
            }
        }

        // 1st: Clear Hierarchy
        PrefabUtility.RevertPrefabInstance(Tilemap.gameObject);

        // 2nd: Insert all Commands for the Level-Hierarchie into a list
        foreach (Transform DungeonChild in Tilemap)
        {
            foreach (Transform LayerObject in DungeonChild)
            {
                // Now we are at the level of all of the Dungeon Objects defined in tiled. Check if they are a command
                if (LayerObject.name.Contains(":") || LayerObject.name.Contains(";")
                    || LayerObject.name.Contains(".") || LayerObject.name.Contains(","))
                {
                    commandList.Add(new ImportCommand(LayerObject.name, LayerObject));
                }
            }
        }

        // 3rd: Check all Commands for general syntax Errors and expand the ImportCommand Class
        foreach (var currCommand in commandList) 
        {
            string command = currCommand.function;

            if (currCommand.target == null)
            {
                Debug.LogError("One of the commands does not have a target: " + command);
                hadError = true;
                errorCount++;
                continue;
            }

            // 3.1 function definition:
            if (!command.Contains(":")) 
            {
                Debug.LogError("One of the commands does not include a function seperator ':' " + command);
                hadError = true;
                errorCount++;
                continue;
            }
            string[] splitter = command.Split(':');
            if (splitter.Length != 2)
            {
                Debug.LogError("This command cannot be split into two by the function seperator ':' " + command);
                foreach (var splitPart in splitter)
                {
                    Debug.LogError("Current Splitters: " + splitPart);
                }
                hadError = true;
                errorCount++;
                continue;
            }                
            currCommand.function = splitter[0];
            command = splitter[1];

            // 3.2 link paramter
            splitter = command.Split('.');
            if (!(splitter.Length == 1 || splitter.Length == 2))
            {
                Debug.LogError("This command cannot be split into two or not by the link seperator '.' " + command);
                foreach (var splitPart in splitter)
                {
                    Debug.LogError("Current Splitters: " + splitPart);
                }
                hadError = true;
                errorCount++;
                continue;
            }
            command = splitter[0];
            if (splitter.Length == 2) currCommand.linkParamter = int.Parse(splitter[1]);

            // 3.3 other parameters
            splitter = command.Split(';');
            foreach (var parameter in splitter)
            {
                currCommand.parameters.Add(parameter);
            }
        }

        // 4th: Check all Commands for logic and function specific syntax Errors.
        foreach (var currCommand in commandList)
        {
            switch (currCommand.function)
            {
                case "enim":
                    {
                        if (!TestEnimCommand(currCommand))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                case "rup":
                    {
                        if(!TestRupCommand(currCommand))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                case "switch":
                    {
                        if(!TestSwitchCommand(currCommand))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                case "door":
                    {
                        if(!TestDoorCommand(currCommand))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                case "chest":
                    {
                        if(!TestChestCommand(currCommand))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                case "doorcoll":
                    {
                        if(!TestDoorCollCommand(currCommand))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                case "skey":
                    {
                        if(!TestSkeyCommand(currCommand))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                default:
                    {
                        Debug.LogError("Checking Command Logic: Found undefined Command: " + currCommand.function);
                        hadError = true;
                        errorCount++;
                    }
                    break;
            } 
        }

        // 5th: Execute these Commands on an per Object Basis.
        Tiled2Unity.TiledMap mapScript = Tilemap.GetComponent<Tiled2Unity.TiledMap>();
        foreach (var currCommand in commandList)
        {
            switch (currCommand.function)
            {
                case "enim":
                    {
                        if(!ExecEnimCommand(currCommand, mapScript))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                case "rup":
                    {
                        if(!ExecRupCommand(currCommand, mapScript))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                case "switch":
                    {
                        if(!ExecSwitchCommand(currCommand, mapScript))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                case "door":
                    {
                        if(!ExecDoorCommand(currCommand, mapScript))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                case "chest":
                    {
                        if(!ExecChestCommand(currCommand, mapScript))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                case "doorcoll":
                    {
                        if(!ExecDoorCollCommand(currCommand, mapScript))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                case "skey":
                    {
                        if(!ExecSkeyCommand(currCommand, mapScript))
                        {
                            hadError = true;
                            errorCount++;
                        }
                    }
                    break;
                default:
                    {
                        Debug.LogError("Executing Command Logic: Found undefined Command: " + currCommand.function);
                        hadError = true;
                        errorCount++;
                    }
                    break;
            } 
        }

        // 6th: Execute the Linking Phase.
        int currLinkNumber = 1;
        List<ImportCommand> linkPartners = new List<ImportCommand>();
        do
        {
            linkPartners.Clear();
            foreach (var command in commandList) 
            {
                if(command.linkParamter == currLinkNumber)
                {
                    linkPartners.Add(command);
                }
            }

            if(linkPartners.Count > 0)
            {
                if(!doLinking(linkPartners))
                {
                    hadError = true;
                    errorCount++;
                }
                currLinkNumber++;
            }

        } while (linkPartners.Count > 0);

        // 7th: Other Changes/Fixes not tied to Commands:
        foreach (Transform Tileset in Tilemap)
        {
            if (Tileset.name == "Above")
            {
                foreach (Transform AboveChild in Tileset)
                {
                    AboveChild.GetComponent<Renderer>().sortingOrder = 2;
                    break;
                }
            }
        }

        if (!hadError) UnityEngine.Debug.Log("Extra Tiled Conversion Done.");
        else UnityEngine.Debug.LogError("Error during extra Tiled conversion, had " + errorCount + " Errors!");
		
	}

    // Test Command Logic and Syntax
    private static bool TestEnimCommand(ImportCommand command)
    {
        // check param count
        if (command.parameters.Count != 2)
        {            
            Debug.LogError("enim command does not have two parameters: " + command.target.name);
            return false;
        }
            
        // check 1st Param
        if (!(command.parameters[0] == "darkfairy"))
        {
            Debug.LogError("enim command ( " + command.target.name +") unknown Enemytype Parameter: " + command.parameters[0]);
            return false;
        }

        // check 2nd Param
        if (!(command.parameters[1] == "0x0" || command.parameters[1].Contains("r1") || command.parameters[1].Contains("r5") ))
        {
            Debug.LogError("enim command ( " + command.target.name +") unknown Loot Parameter: " + command.parameters[1]);
            return false;
        }
        return true;
    }
    private static bool TestRupCommand(ImportCommand command)
    {
        // check param count
        if (command.parameters.Count != 1)
        {            
            Debug.LogError("rup command does not have one parameters: " + command.target.name);
            return false;
        }

        // check 1st Param
        if (!(command.parameters[0] == "1" || command.parameters[0] == "5"))
        {
            Debug.LogError("rup command ( " + command.target.name +") unknown RupeAmount Parameter: " + command.parameters[0]);
            return false; 
        }

        // check link Param
        if (command.linkParamter != int.MaxValue)
        {
            Debug.LogError("rup command does have a linkParamter which is prohibited: " + command.target.name);
            return false;
        }
        return true;
    }
    private static bool TestSwitchCommand(ImportCommand command)
    {
        // check param count
        if (command.parameters.Count != 1)
        {            
            Debug.LogError("switch command does not have two parameters: " + command.target.name);
            return false;
        }

        // check 1st Param
        if (command.parameters[0] != "d")
        {
            Debug.LogError("switch command ( " + command.target.name +") unknown Connection Parameter: " + command.parameters[0]);
            return false;
        }

        // check link Param
        if (command.linkParamter == int.MaxValue)
        {
            Debug.LogError("switch command's link parameter not set: " + command.target.name);
            return false;
        }
        return true;
    }
    private static bool TestDoorCommand(ImportCommand command)
    {
        // check param count
        if (command.parameters.Count != 2)
        {            
            Debug.LogError("door command does not have two parameters: " + command.target.name);
            return false;
        }

        // check 1st Param
        if (!(command.parameters[0] == "n" || command.parameters[0] == "e" 
           || command.parameters[0] == "s" || command.parameters[0] == "w"))
        {
            Debug.LogError("door command ( " + command.target.name +") unknown direction Parameter: " + command.parameters[0]);
            return false;
        }

        // check 2nd Param
        if (!(command.parameters[1] == "sk" || command.parameters[1] == "s" || command.parameters[1] == "enim"))
        {
            Debug.LogError("door command ( " + command.target.name +") unknown interactSource Parameter: " + command.parameters[1]);
            return false;
        }

        // check link Param
        if (command.parameters[1] == "sk" && command.linkParamter != int.MaxValue)
        {
            Debug.LogError("door command does have a linkParamter although it is a smallkey-door " +
                "which is prohibited: " + command.target.name);
            return false;
        }
        else if (command.parameters[1] == "s" && command.linkParamter == int.MaxValue)
        {
            Debug.LogError("door command's link parameter not set although it is a switch-door: " + command.target.name);
            return false;
        }
        else if (command.parameters[1] == "enim" && command.linkParamter == int.MaxValue)
        {
            Debug.LogError("door command's link parameter not set although it is a enemy-door: " + command.target.name);
            return false;
        }
        return true;
    }
    private static bool TestChestCommand(ImportCommand command)
    {
        // check param count
        if (command.parameters.Count != 2)
        {            
            Debug.LogError("chest command does not have two parameters: " + command.target.name);
            return false;
        }

        // check 1st Param
        if (command.parameters[0] != "small")
        {
            Debug.LogError("chest command ( " + command.target.name +") unknown size Parameter: " + command.parameters[0]);
            return false;
        }

        // check 2nd Param
        if (!(command.parameters[1] == "key" || command.parameters[1] == "r1" || command.parameters[1] == "r5"))
        {
            Debug.LogError("chest command ( " + command.target.name +") unknown loot Parameter: " + command.parameters[1]);
            return false;
        }

        // check link Param
        if (command.linkParamter != int.MaxValue)
        {
            Debug.LogError("chest command does have a linkParamter which is prohibited: " + command.target.name);
            return false;
        }

        return true;
    }
    private static bool TestDoorCollCommand(ImportCommand command)
    {
        // check param count
        if (!(command.parameters.Count == 0 || command.parameters[0] == ""))
        {
            Debug.LogError("doorcoll command does not have any parameters: " + command.target.name);
            return false;
        }

        // check link Param
        if (command.linkParamter != int.MaxValue)
        {
            Debug.LogError("doorcoll command does have a linkParamter which is prohibited: " + command.target.name);
            return false;
        }

        return true;
    }
    private static bool TestSkeyCommand(ImportCommand command)
    {
        // check param count
        if (!(command.parameters.Count == 1))
        {
            Debug.LogError("skey command does not have two parameters: " + command.target.name);
            return false;
        }

        // check 1st Param
        if (!(command.parameters[0] == "enim" || command.parameters[0] == "0x0"))
        {
            Debug.LogError("skey command ( " + command.target.name +") unknown spawn condition Parameter: " + command.parameters[0]);
            return false;
        }

        // check link Param
        if (command.parameters[0] == "enim" && command.linkParamter == int.MaxValue)
        {
            Debug.LogError("skey command's link parameter not set although it is a enim-smallkey: " + command.target.name);
            return false;
        }
        else if (command.parameters[0] == "0x0" && command.linkParamter != int.MaxValue)
        {
            Debug.LogError("skey command does have a linkParamter although it is a 0x0-smallkey " +
                "which is prohibited: " + command.target.name);
            return false;
        }

        return true;
    }

    // Execute Commands per Object
    private static bool ExecEnimCommand(ImportCommand command, Tiled2Unity.TiledMap Tilemap)
    {
        Vector3 tileOffset = new Vector3( ((Tilemap.TileWidth / 2) * Tilemap.ExportScale),
                                        - ((Tilemap.TileWidth / 2) * Tilemap.ExportScale),
                                        0.0f);
        // Exec 1st Param
        GameObject newObj = null;
        switch (command.parameters[0])
        {
            case "darkfairy":
                {
                    newObj = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/DarkFairy"));
                    newObj.transform.parent = command.target;
                    newObj.transform.position = command.target.position + tileOffset;
                    command.target.GetComponent<BoxCollider2D>().enabled = false;
                }
                break;
            default:
                {
                    Debug.LogError("Executing Enim Command: Unknown Enemytype Parameter: " + command.parameters[0]);
                    Debug.LogError("Current Command: " + command.target.name);
                    return false;
                }
        }

        // Exec 2nd Param
        if (command.parameters[1] == "0x0")
        {
            
        }
        else if (command.parameters[1].Contains("r"))
        {
            string[] splitter = command.parameters[1].Split('r');
            if(splitter.Length != 2)
            {
                Debug.LogError("Executing Enim Command: Rupee Loot Parameter, wrong syntax: " + command.parameters[1]);
                Debug.LogError("Current Command: " + command.target);
                return false;
            }
            int rupeeAmount = int.Parse(splitter[1]);
            if (rupeeAmount == 1)
            {
                foreach (Transform child in command.target)
                {
                    if(child.name.ToLower().Contains(command.parameters[0]))
                    {
                        child.GetComponent<Enemy>().drops.Add(
                            new Enemy.EnemyDrop(Enemy.DropTypes.rupee1, int.Parse(splitter[0])));
                    }
                }
            }
            else if (rupeeAmount == 5)
            {
                foreach (Transform child in command.target)
                {
                    if(child.name.ToLower().Contains(command.parameters[0]))
                    {
                        child.GetComponent<Enemy>().drops.Add(
                            new Enemy.EnemyDrop(Enemy.DropTypes.rupee5, int.Parse(splitter[0])));
                    }
                }
            }
            else
            {
                Debug.LogError("Executing Enim Command: Rupee Loot Parameter, wrong amount: " + command.parameters[1]);
                Debug.LogError("Current Command: " + command.target);
                return false;
            }       
        }
        else
        {
            Debug.LogError("Executing Enim Command: Unknown Loot Parameter: " + command.parameters[1]);
            Debug.LogError("Current Command: " + command.target);
            return false;
        }
        return true;
    }
    private static bool ExecRupCommand(ImportCommand command, Tiled2Unity.TiledMap Tilemap)
    {
        Vector3 tileOffset = new Vector3( ((Tilemap.TileWidth / 2) * Tilemap.ExportScale),
            - ((Tilemap.TileWidth / 2) * Tilemap.ExportScale),
            0.0f);
        // Exec 1st Param
        switch (command.parameters[0])
        {
            case "1":
                {
                    GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Rupee1"), 
                        command.target.position + tileOffset, Quaternion.identity, command.target);
                    command.target.GetComponent<BoxCollider2D>().enabled = false;
                }break;
            case "5":
                {
                    GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Rupee5"), 
                        command.target.position + tileOffset, Quaternion.identity, command.target);
                    command.target.GetComponent<BoxCollider2D>().enabled = false;
                }break;
            default:
                {
                    Debug.LogError("Executing Rup Command: Unknown Amount Parameter: " + command.parameters[0]);
                    Debug.LogError("Current Command: " + command.target.name);
                    return false;
                }
        }
        return true;
    }
    private static bool ExecSwitchCommand(ImportCommand command, Tiled2Unity.TiledMap Tilemap)
    {
        // Exec 1st Param
        switch (command.parameters[0])
        {
            case "d":
                {
                    
                }break;
            default:
                {
                    Debug.LogError("Executing Switch Command: Unknown Connection Parameter: " + command.parameters[1]);
                    return false;
                }
        }

        // General Settings
        if (command.target.GetComponent<Switch>() == null)
        {
            command.target.gameObject.AddComponent<Switch>();
        }
        command.target.GetComponent<BoxCollider2D>().isTrigger = true;

        return true;
    }
    private static bool ExecDoorCommand(ImportCommand command, Tiled2Unity.TiledMap Tilemap)
    {
        // Query all the necessary Sprites.
        UnityEngine.Object[] sprites = Resources.LoadAll("Sprites/DungeonTileset");
        Sprite DoorN = null;
        Sprite DoorE = null;
        Sprite DoorS = null;
        Sprite DoorW = null;
        Sprite DoorNKClosed = null;
        Sprite DoorEKClosed = null;
        Sprite DoorSKClosed = null;
        Sprite DoorWKClosed = null;
        foreach (var sprite in sprites)
        {
            if (sprite.name == "Door_N")
            {
                DoorN = (Sprite)sprite;
            }
            else if (sprite.name == "Door_E")
            {
                DoorE = (Sprite)sprite;
            }
            else if (sprite.name == "Door_S")
            {
                DoorS = (Sprite)sprite;
            }
            else if (sprite.name == "Door_W")
            {
                DoorW = (Sprite)sprite;
            }
            else if (sprite.name == "Door_NKClosed")
            {
                DoorNKClosed = (Sprite)sprite;
            }
            else if (sprite.name == "Door_EKClosed")
            {
                DoorEKClosed = (Sprite)sprite;
            }
            else if (sprite.name == "Door_SKClosed")
            {
                DoorSKClosed = (Sprite)sprite;
            }
            else if (sprite.name == "Door_WKClosed")
            {
                DoorWKClosed = (Sprite)sprite;
            }
        }

        if (DoorN == null || DoorE == null || DoorS == null || DoorW == null
            || DoorNKClosed == null || DoorEKClosed == null || DoorSKClosed == null || DoorWKClosed == null)
        {
            Debug.LogError("Executing Door Command: One of the DoorSprites could not be found!");
            Debug.LogError(DoorN + "; " + DoorE + "; " + DoorS + "; " + DoorW + "; " + 
                DoorNKClosed + "; " + DoorEKClosed + "; " + DoorSKClosed + "; " + DoorWKClosed + "; ");
            return false;
        }

        // Exec 1st Param
        if (command.target.GetComponent<SpriteRenderer>() == null)
        {
            // Additional changes:
            Vector3 tileOffset = new Vector3( ((Tilemap.TileWidth / 2) * Tilemap.ExportScale),
                - ((Tilemap.TileWidth / 2) * Tilemap.ExportScale),
                0.0f);
            command.target.position += 2* tileOffset;
            command.target.GetComponent<BoxCollider2D>().offset = Vector2.zero;


            SpriteRenderer renderer = command.target.gameObject.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 1;

            switch (command.parameters[0])
            {
                case "n":
                    {
                        if(command.parameters[1] == "s" || command.parameters[1] == "enim") renderer.sprite = DoorN;
                        else if(command.parameters[1] == "sk") renderer.sprite = DoorNKClosed;
                    }break;
                case "e":
                    {
                        if(command.parameters[1] == "s" || command.parameters[1] == "enim") renderer.sprite = DoorE;
                        else if(command.parameters[1] == "sk") renderer.sprite = DoorEKClosed;
                    }break;
                case "s":
                    {
                        if(command.parameters[1] == "s" || command.parameters[1] == "enim") renderer.sprite = DoorS;
                        else if(command.parameters[1] == "sk") renderer.sprite = DoorSKClosed;
                    }break;
                case "w":
                    {
                        if(command.parameters[1] == "s" || command.parameters[1] == "enim") renderer.sprite = DoorW;
                        else if(command.parameters[1] == "sk") renderer.sprite = DoorWKClosed;
                    }break;
                default:
                    {
                        Debug.LogError("Executing Door Command: Unknown direction Parameter: " + command.parameters[0]);
                        return false;
                    }
            }
        }

        // Exec 2nd Param
        switch (command.parameters[1])
        {
            case "sk":
                {
                    KeyDoor door = command.target.gameObject.GetComponent<KeyDoor>();
                    if (door == null)
                    {
                        door = command.target.gameObject.AddComponent<KeyDoor>();
                    }
                }break;
            case "s":
                {
                    
                }break;
            case "enim":
                {
                    if (command.target.gameObject.GetComponent<EnemyDoor>() == null)
                    {
                        command.target.gameObject.AddComponent<EnemyDoor>();
                    }
                }break;
            default:
                {
                    Debug.LogError("Executing Door Command: Unknown interactSource Parameter: " + command.parameters[1]);
                    return false;
                }
        }
        return true;
    }
    private static bool ExecChestCommand(ImportCommand command, Tiled2Unity.TiledMap Tilemap)   
    {
        // TODO: May have to change the collider too if i change the graphic to be a large Chest!
        // Exec 1st Param
        switch (command.parameters[0])
        {
            case "small":
                {
                    if (command.target.GetComponent<SpriteRenderer>() == null)
                    {
                        // Additional changes:

                        Vector3 tileOffset = new Vector3( ((Tilemap.TileWidth / 2) * Tilemap.ExportScale),
                            - ((Tilemap.TileWidth / 2) * Tilemap.ExportScale),
                            0.0f);
                        command.target.position += tileOffset;
                        command.target.GetComponent<BoxCollider2D>().offset = Vector2.zero;


                        SpriteRenderer renderer = command.target.gameObject.AddComponent<SpriteRenderer>();
                        renderer.sortingOrder = 1;

                        UnityEngine.Object[] sprites = Resources.LoadAll("Sprites/DungeonTileset");
                        foreach (var sprite in sprites)
                        {
                            if (sprite.name == "SChest_Closed")
                            {
                                renderer.sprite = (Sprite)sprite;
                            }
                        }
                    }
                }break;
            default:
                {
                    Debug.LogError("Executing Chest Command: Unknown size Parameter: " + command.parameters[0]);
                    return false;
                }
        }

        // Exec 2nd Param
        Chest chestScript = command.target.gameObject.AddComponent<Chest>();
        switch (command.parameters[1])
        {
            case "key":
                {
                    chestScript.loot = Chest.Loot.SmallKey;
                }break;
            case "r5":
                {
                    chestScript.loot = Chest.Loot.Rupee;
                    chestScript.amount = 5;
                }
                break;
            case "r1":
                {
                    chestScript.loot = Chest.Loot.Rupee;
                    chestScript.amount = 1;
                }
                break;
            default:
                {
                    Debug.LogError("Executing Chest Command: Unknown loot Parameter: " + command.parameters[1]);
                    return false;
                }
        }
        return true;
    }
    private static bool ExecDoorCollCommand(ImportCommand command, Tiled2Unity.TiledMap Tilemap)
    {
        if (command.target.GetComponent<DoorColliderEnemy>() == null)
        {
            command.target.gameObject.AddComponent<DoorColliderEnemy>();
        }

        return true;
    }
    private static bool ExecSkeyCommand(ImportCommand command, Tiled2Unity.TiledMap Tilemap)
    {
        if (command.target.GetComponent<KeySpawner>() == null)
        {
            command.target.gameObject.AddComponent<KeySpawner>();
        }
        return true;
    }

    // Linking
    private static bool doLinking(List<ImportCommand> linkPartners)
    {
        // 1st: Try to find the LinkSource.
        ImportCommand linkSource = null;
        foreach (var partner in linkPartners)
        {
            if (partner.function == "switch")
            {
                // This is the source
                if(linkPartners.Count != 2)
                {
                    Debug.LogError("Linking Error: Link Source is a switch, " +
                        "but you have no partner: " + partner.target.name);
                    return false;
                }

                linkSource = partner;
                break;
            }
            else if (partner.function == "door")
            {
                // This is the source
                if(linkPartners.Count <= 2)
                {
                    Debug.LogError("Linking Error: Link Source is a door, " +
                        "but you have no partner: " + partner.target.name);
                    return false;
                }

                linkSource = partner;
                break;
            }
            else if (partner.function == "skey")
            {
                // This is the source
                if(linkPartners.Count <= 2)
                {
                    Debug.LogError("Linking Error: Link Source is a door, " +
                        "but you have no partner: " + partner.target.name);
                    return false;
                }

                linkSource = partner;
                break;
            }

        }

        if (linkSource == null)
        {
            Debug.LogError("Linking Error: Couldn't find the list source in this linkPartner List!");
            String concat = "";
            foreach (var partner in linkPartners)
            {
                concat += partner.target.name + " ";
            }
            Debug.LogError("linkPartners: " + concat);

            return false;
        }

        // 2nd: Switch based on the function of the source.
        switch (linkSource.function)
        {
            case "switch":
                {
                    linkPartners.Remove(linkSource);
                    if (!ExecSwitchLink(linkSource, linkPartners[0])) return false;
                }
                break;
            case "door":
                {
                    linkPartners.Remove(linkSource);
                    if (!ExecEnemyDoorLink(linkSource, linkPartners)) return false;
                }
                break;
            case "skey":
                {
                    linkPartners.Remove(linkSource);
                    if (!ExecSkeyLink(linkSource, linkPartners)) return false;
                }
                break;
            default:
                {
                    Debug.LogError("Linking Error: Unknown Link Source: " + linkSource.target.name);
                    return false;
                }
        }

        return true;
    }

    private static bool ExecSwitchLink(ImportCommand switchCommand, ImportCommand target)
    {
        if(!(target.parameters[1] == "s" && switchCommand.parameters[0] == "d"))
        {
            Debug.LogError("Error Linking Switch: You tried to link a switch to a " +
                "door that is not a s-door or the switch is not a d-switch: " + target.target.name
                + " " + switchCommand.target.name);
            return false;
        }

        switchCommand.target.GetComponent<Switch>().target = target.target.gameObject;
        return true;
    }
    private static bool ExecEnemyDoorLink(ImportCommand doorCommand, List<ImportCommand> targets)
    {
        if(!(doorCommand.parameters[1] == "enim"))
        {
            String concat = "";
            foreach (var target in targets)
            {
                concat += target.target.name + " ";
            }
            Debug.LogError("Error Linking Door: You tried to link enemis to a" +
                "door that is not a enim-door: " + concat
                + " " + doorCommand.target.name);
            return false;
        }

        doorCommand.target.GetComponent<EnemyDoor>().numEnemies = targets.Count;
        foreach (var enemy in targets)
        {
            enemy.target.GetChild(0).GetComponent<Enemy>().callback = doorCommand.target.gameObject;
        }
        return true;
    }
    private static bool ExecSkeyLink(ImportCommand skeyCommand, List<ImportCommand> targets)
    {
        if(!(skeyCommand.parameters[0] == "enim"))
        {
            String concat = "";
            foreach (var target in targets)
            {
                concat += target.target.name + " ";
            }
            Debug.LogError("Error Linking Door: You tried to link enemis to a" +
                "door that is not a enim-door: " + concat
                + " " + skeyCommand.target.name);
            return false;
        }

        skeyCommand.target.GetComponent<KeySpawner>().numEnemies = targets.Count;
        foreach (var enemy in targets)
        {
            enemy.target.GetChild(0).GetComponent<Enemy>().callback = skeyCommand.target.gameObject;
        }
        return true;
    }

}