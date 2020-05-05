using System;
using System.Collections.Generic;

namespace ConsoleApp_Sandbox.AI_Bots.GhostInTheCell
{
    class GhostInTheCellAI
    {
        static void _Main(string[] args)
        {
            string[] inputs;
            int factoryCount = int.Parse(Console.ReadLine()); // the number of factories
            int linkCount = int.Parse(Console.ReadLine()); // the number of links between factories
            var ignoreFactoryIdCache = new List<int>();

            for (int i = 0; i < linkCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int factory1 = int.Parse(inputs[0]);
                int factory2 = int.Parse(inputs[1]);
                int distance = int.Parse(inputs[2]);
            }

            // game loop
            while (true)
            {
                int entityCount = int.Parse(Console.ReadLine()); // the number of entities (e.g. factories and troops)
                int myFactoryId = -1;
                int myFactoryValue = Int32.MinValue;
                int enemyFactoryId = -1;
                int enemyFactoryValue = Int32.MinValue;
                int troopsNb = 0;
                for (int i = 0; i < entityCount; i++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    int entityId = int.Parse(inputs[0]);
                    if (ignoreFactoryIdCache.Contains(entityId))
                    {
                        continue;
                    }

                    string entityType = inputs[1];
                    int arg1 = int.Parse(inputs[2]);
                    int arg2 = int.Parse(inputs[3]);
                    int arg3 = int.Parse(inputs[4]);
                    int arg4 = int.Parse(inputs[5]);
                    int arg5 = int.Parse(inputs[6]);

                    Console.Error.WriteLine(string.Join(",", inputs));
                    if (entityType.Equals("FACTORY"))
                    {
                        if (arg3 == 0)
                        {
                            ignoreFactoryIdCache.Add(entityId);
                        }

                        //Console.Error.WriteLine(string.Format("myFactory: {0} - {1} : compareFactory: {2} - {3}", myFactoryId, myFactoryValue, entityId, arg2));
                        if (arg1 == 1)
                        {
                            if (arg2 > myFactoryValue)
                            {
                                myFactoryId = entityId;
                                myFactoryValue = arg2;
                                troopsNb = arg2;
                            }
                        }
                        else
                        {
                            if ((arg3 - arg2) > enemyFactoryValue)
                            {
                                enemyFactoryId = entityId;
                                enemyFactoryValue = (arg3 - arg2);
                            }
                        }
                    }
                }

                if (myFactoryId != -1 && enemyFactoryId != -1)
                {
                    Console.WriteLine(string.Format("MOVE {0} {1} {2}", myFactoryId, enemyFactoryId, troopsNb));
                }
                else
                {
                    // Any valid action, such as "WAIT" or "MOVE source destination cyborgs"
                    Console.WriteLine("WAIT");
                }
            }
        }
    }
}