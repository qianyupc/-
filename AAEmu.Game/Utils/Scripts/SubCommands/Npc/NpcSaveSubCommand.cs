﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using AAEmu.Commons.IO;
using AAEmu.Commons.Utils;
using AAEmu.Game.Core.Managers.World;
using AAEmu.Game.Models.Game.Char;
using AAEmu.Game.Models.Game.World;
using AAEmu.Game.Models.Json;
using AAEmu.Game.Utils.Converters;
using Newtonsoft.Json;

namespace AAEmu.Game.Utils.Scripts.SubCommands
{
    public class NpcSaveSubCommand : SubCommandBase
    {
        public NpcSaveSubCommand()
        {
            Prefix = "[Npc Save]";
            Description = "Save one or all npc positions in the current character world to the world npc spawns file";
            CallExample = "/npc save all||/npc save <ObjId>";
        }

        public override void Execute(ICharacter character, string triggerArgument, string[] args)
        {
            var firstArgument = args.FirstOrDefault();
            if (firstArgument is null)
            {
                character.SendMessage(CallExample);
                return;
            }

            if (firstArgument == "all")
            {
                SaveAll(character);
                return;
            }

            if (!uint.TryParse(firstArgument, out var npcObjId))
            {
                SendMessage(character, "Invalid <ObjId> for Npc, please use a number");
                return;
            }
            
            SaveById(character, npcObjId);
        }

        private void SaveAll(ICharacter character)
        {
            var currentWorld = WorldManager.Instance.GetWorld(((Character)character).Transform.WorldId);
            var allNpcs = WorldManager.Instance.GetAllNpcs();
            var npcsInWorld = WorldManager.Instance.GetAllNpcsFromWorld(currentWorld.Id);
            var npcSpawnersFromFile = LoadNpcsFromFileByWorld(currentWorld);
            var npcSpawnersToFile = npcSpawnersFromFile.ToList();

            foreach(var npc in npcsInWorld.Where(n => n.Spawner is not null))
            {
                switch (npc.Spawner.Id)
                {
                    // spawned into the game manually
                    case 0:
                        {
                            var pos = npc.Transform.World;
                            var newNpcSpawn = new JsonNpcSpawns
                            {
                                Id = allNpcs.Last().ObjId++,
                                UnitId = npc.TemplateId,
                                Position = new JsonPosition
                                {
                                    X = pos.Position.X,
                                    Y = pos.Position.Y,
                                    Z = pos.Position.Z,
                                    Roll = pos.Rotation.X.RadToDeg(),
                                    Pitch = pos.Rotation.Y.RadToDeg(),
                                    Yaw = pos.Rotation.Z.RadToDeg()
                                }
                            };

                            npcSpawnersToFile.Add(newNpcSpawn);
                            break;
                        }

                    // removed from the game manually
                    case 0xffffffff: //(uint)-1
                        {
                            // Do not add to the output of a manually remote Npc
                            var npcSpawnsToRemove = new List<JsonNpcSpawns>();

                            foreach (var npcSpawnerToFile in npcSpawnersToFile
                                .Where(n => n.UnitId == npc.TemplateId))
                            {
                                // If the position changed don't mark to be removed
                                if (!npc.Transform.World.Position.Equals(npcSpawnerToFile.Position.AsVector3()))
                                {
                                    continue;
                                }

                                npcSpawnsToRemove.Add(npcSpawnerToFile);
                                break;
                            }

                            foreach (var npcSpawn in npcSpawnsToRemove)
                            {
                                npcSpawnersToFile.Remove(npcSpawn);
                            }
                            break;
                        }
                }
            }

            var jsonPathOut = Path.Combine(FileManager.AppPath, "Data", "Worlds", currentWorld.Name, "npc_spawns_new.json");
            var json = JsonConvert.SerializeObject(npcSpawnersToFile.ToArray(), Formatting.Indented, new JsonModelsConverter());
            File.WriteAllText(jsonPathOut, json);
            SendMessage(character, "All npcs have been saved!");
        }

        private void SaveById(ICharacter character, uint npcObjId)
        {
            var spawners = new List<JsonNpcSpawns>();
            var npc = WorldManager.Instance.GetNpc(npcObjId);
            if (npc is null)
            {
                SendColorMessage(character, Color.Red, "Npc with objId {0} Does not exist |r", npcObjId);
                return;
            }
            
            var world = WorldManager.Instance.GetWorld(npc.Transform.WorldId);
            if (world is null)
            {
                SendColorMessage(character, Color.Red, "Could not find the worldId {0} |r", npc.Transform.WorldId);
                return;
            }

            var spawn = new JsonNpcSpawns
            {
                Id = npc.ObjId,
                UnitId = npc.TemplateId,
                Position = new JsonPosition
                {
                    X = npc.Transform.Local.Position.X,
                    Y = npc.Transform.Local.Position.Y,
                    Z = npc.Transform.Local.Position.Z,
                    Roll = npc.Transform.Local.Rotation.X.RadToDeg(),
                    Pitch = npc.Transform.Local.Rotation.Y.RadToDeg(),
                    Yaw = npc.Transform.Local.Rotation.Z.RadToDeg()
                }
            };

            try
            {
                var spawnersFromFile = LoadNpcsFromFileByWorld(world).ToDictionary(x => x.Id, x => x);

                if (spawnersFromFile.ContainsKey(spawn.Id))
                {
                    spawnersFromFile[spawn.Id] = spawn;
                }
                else
                {
                    spawnersFromFile.Add(spawn.Id, spawn);
                }

                var jsonPathOut = Path.Combine(FileManager.AppPath, "Data", "Worlds", world.Name, "npc_spawns_new.json");
                var json = JsonConvert.SerializeObject(spawnersFromFile.Values.ToArray(), Formatting.Indented, new JsonModelsConverter());
                File.WriteAllText(jsonPathOut, json);
                SendMessage(character, "All npcs have been saved with added npc ObjId:{0}, TemplateId:{1}", npc.ObjId, npc.TemplateId);

            }
            catch (Exception e)
            {
                character.SendMessage(e.Message);
                _log.Warn(e);
            }
        }

        private List<JsonNpcSpawns> LoadNpcsFromFileByWorld(World world)
        {
            var jsonPathIn = Path.Combine(FileManager.AppPath, "Data", "Worlds", world.Name, "npc_spawns.json");
            if (!File.Exists(jsonPathIn))
            {
                throw new ApplicationException($"File {jsonPathIn} doesn't exists.");
            }

            var contents = FileManager.GetFileContents(jsonPathIn);
            _log.Info($"Loading spawns from file {jsonPathIn} ...");

            if (string.IsNullOrWhiteSpace(contents))
            {
                return new List<JsonNpcSpawns>();
            }
            else
            {
                return JsonHelper.DeserializeObject<List<JsonNpcSpawns>>(contents, new JsonModelsConverter());
            }
        }
    }
}