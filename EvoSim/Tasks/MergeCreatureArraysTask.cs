using EvoSim.ThreadingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EvoNet.Objects;

namespace EvoSim.Tasks
{
    class MergeCreatureArraysTask : SimulationTask
    {
        ThreadTaskGroup creatureActTaskGroup;

        public MergeCreatureArraysTask(Simulation sim, ThreadTaskGroup actTaskGroup) :
            base(sim)
        {
            creatureActTaskGroup = actTaskGroup;
        }

        protected override void Run(float time)
        {
            foreach (ThreadTask task in creatureActTaskGroup.Tasks)
            {
                CreatureTask creatureTask = task as CreatureTask;
                if (creatureTask != null)
                {
                    sim.CreatureManager.numberOfDeaths += creatureTask.CreaturesToKill.Count;
                    foreach (Creature creature in creatureTask.CreaturesToKill)
                    {
                        sim.CreatureManager.Graveyard.Add(creature);
                        sim.CreatureManager.Creatures.Remove(creature);
                    }
                    creatureTask.CreaturesToKill.Clear();
                    foreach (Creature creature in creatureTask.CreaturesToSpawn)
                    {
                        sim.CreatureManager.Creatures.Add(creature);
                    }
                    creatureTask.CreaturesToSpawn.Clear();
                }

            }
        }
    }
}
