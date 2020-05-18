using System;
using System.Text;
using System.Collections.Generic;

/**
 * Grab the pellets as fast as you can!
 **/
class Player
{
    const int SUPER_PELLET_VALUE = 10;
    const int NUM_PARTITIONS = 6;
    const int STATUS_AVAILABLE = -1;
    const int STATUS_BUSY = 0;
    const int STATUS_COMPLETE = 1;
    const int BLOCKED_COOLDOWN = 5;
    // todo: this could be cooldown time -> distance
    const int SWITCH_DISTANCE = 5;
    const int TYPE_ROCK = 1; const int TYPE_PAPER = 2; const int TYPE_SCISSORS = 4;
    // const int COMPARE_ROCK_PAPER = TYPE_ROCK-TYPE_PAPER;
    const int COMPARE_ROCK_SCISSORS = TYPE_ROCK-TYPE_SCISSORS;
    // const int COMPARE_PAPER_ROCK = TYPE_PAPER-TYPE_ROCK;
    // const int COMPARE_PAPER_SCISSORS = TYPE_PAPER-TYPE_SCISSORS;
    // const int COMPARE_SCISSORS_ROCK = TYPE_SCISSORS-TYPE_ROCK;
    // const int COMPARE_SCISSORS_PAPER = TYPE_SCISSORS-TYPE_PAPER;
    const int ENEMY_VALUE_POSITIVE = 15;
    const int ENEMY_VALUE_NEUTRAL = 0;
    const int ENEMY_VALUE_NEGATIVE = -15;
    const int ACTION_NONE = 0; const int ACTION_MOVE = 1; const int ACTION_BOOST = 2; const int ACTION_SWITCH = 3;

    static Random rGen = new Random();
    static int[] MapDimensions = new int[2];
    static List<string> Map = new List<string>();
    static int[,] PartitionMap;
    static bool[,] PelletFoundMap;
    static Dictionary<int, int[]> PacMen = new Dictionary<int, int[]>();
    static Dictionary<int, int[]> PacMenTargets = new Dictionary<int, int[]>();
    static Dictionary<int, int[]> PacMenStartLocs = new Dictionary<int, int[]>();
    static Dictionary<int, int[]> PacMenPrevLocs = new Dictionary<int, int[]>();
    static Dictionary<int, int[]> PacMenBlockLocs = new Dictionary<int, int[]>();
    static Dictionary<int, int[]> PacMenNearEnemy = new Dictionary<int, int[]>();
    static Dictionary<int, int> PacMenLastMove = new Dictionary<int, int>();
    static List<int> PacMenBlocked = new List<int>();

    static Dictionary<int, int[]> EnemyPacMen = new Dictionary<int, int[]>();
    static Dictionary<int, Dictionary<string, int[]>> Pellets = new Dictionary<int, Dictionary<string, int[]>>();
    static List<int> PartitionStatus = new List<int>();

    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int width = int.Parse(inputs[0]); // size of the grid
        int height = int.Parse(inputs[1]); // top left corner is (x=0, y=0)
        MapDimensions = new int[2]{width, height};
        PartitionMap = new int[height, width];

        // initialize pellets found map defaulted false 
        PelletFoundMap = new bool[height, width];

        for (int y = 0; y < height; y++)
        {
             Map.Add(Console.ReadLine());  // one line of the grid: space " " is floor, pound "#" is wall
            
            var segmentWidth = width/3;
            for(var x = 0; x < width; x++) {
                var subParX = x < segmentWidth ? 0: x < segmentWidth*2 ? 1:2; 
                var subParY = y < height/2 ? 0:3;
                var subPartition = subParX + subParY;
                
                PartitionMap[y,x] = subPartition;
            }
        }

        // initialize Pellet Map with empty Lists for pellet location
        // and set all partitions to available
        for(var i = 0; i < 6; i++) {
            Pellets.Add(i, new Dictionary<string, int[]>());
            PartitionStatus.Add(STATUS_AVAILABLE);
        }

        var initialized = false;

        // game loop
        while (true)
        {
            PacMenBlocked.Clear();
            PacMenBlockLocs.Clear();
            PacMenNearEnemy.Clear();
            PacMenLastMove.Clear();

            inputs = Console.ReadLine().Split(' ');
            int myScore = int.Parse(inputs[0]);
            int opponentScore = int.Parse(inputs[1]);
            int visiblePacCount = int.Parse(Console.ReadLine()); // all your pacs and enemy pacs in sight
            for (int i = 0; i < visiblePacCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int pacId = int.Parse(inputs[0]); // pac number (unique within a team)
                bool mine = inputs[1] != "0"; // true if this pac is yours
                int x = int.Parse(inputs[2]); // position in the grid
                int y = int.Parse(inputs[3]); // position in the grid
                string typeId = inputs[4]; 
                int speedTurnsLeft = int.Parse(inputs[5]); 
                int abilityCooldown = int.Parse(inputs[6]); 
            
                if(!initialized) {
                    if(mine) {
                        PacMen.Add(pacId, new int[6]{x, y, STATUS_AVAILABLE, getTypeId(typeId), speedTurnsLeft, abilityCooldown});
                        PacMenStartLocs.Add(pacId, new int[6]{x, y, STATUS_AVAILABLE, getTypeId(typeId), speedTurnsLeft, abilityCooldown});
                        PacMenPrevLocs.Add(pacId, new int[6]{x-1, y-1, STATUS_AVAILABLE, getTypeId(typeId), speedTurnsLeft, abilityCooldown});
                        PacMenTargets.Add(pacId, null);
                        PacMenLastMove.Add(pacId, ACTION_NONE);
                    } else {
                        EnemyPacMen.Add(pacId, new int[3]{x, y, getTypeId(typeId)});
                    }
                } else {
                    if(mine && PacMen.ContainsKey(pacId)) {
                        PacMen[pacId][0] = x;
                        PacMen[pacId][1] = y;
                        PacMen[pacId][3] = getTypeId(typeId);
                        PacMen[pacId][4] = speedTurnsLeft;
                        PacMen[pacId][5] = abilityCooldown;

                        if(PacMenLastMove.ContainsKey(pacId) && PacMenLastMove[pacId] == ACTION_MOVE
                            && PacMenPrevLocs.ContainsKey(pacId) 
                            && PacMen[pacId][2] != STATUS_AVAILABLE
                            && x == PacMenPrevLocs[pacId][0] && y == PacMenPrevLocs[pacId][1]){
                            Console.Error.WriteLine(string.Format("Detected Block {0}: [{1},{2}]",pacId,x,y));
                            PacMenBlocked.Add(pacId);
                            PacMenBlockLocs.Add(pacId, getRandomLoc());
                        }
                    } else {
                        if(EnemyPacMen.ContainsKey(pacId)) {
                            EnemyPacMen[pacId][0] = x;
                            EnemyPacMen[pacId][1] = y;
                        }
                    }
                }

                 if(mine & PacMen.ContainsKey(pacId) && PacMen[pacId][2] == STATUS_AVAILABLE) {
                    Console.Error.WriteLine(string.Format("Getting Partition for: {0}", pacId));
                    PacMen[pacId][2] = getAvailablePartition();
                }

                 if(initialized) {
                    // drop target if hit
                    if(mine && PacMenTargets.ContainsKey(pacId) && PacMenTargets[pacId] != null 
                        && PacMen.ContainsKey(pacId) && PacMen[pacId][0] == PacMenTargets[pacId][0] 
                        && PacMen[pacId][1] == PacMenTargets[pacId][1]) {
                        Console.Error.WriteLine(string.Format("Target Hit: [{0},{1}]-Id: {2}", PacMen[pacId][0],PacMen[pacId][1],pacId));
                        PacMenTargets[pacId] = null;
                    }

                    // remove target on enemy hit
                    if(!mine && EnemyPacMen.ContainsKey(pacId)){
                        var subPartition = PartitionMap[y,x];
                        var pelletHash = getPelletHash(x,y);
                        if(Pellets.ContainsKey(subPartition) && Pellets[subPartition].ContainsKey(pelletHash)){
                            Pellets[subPartition].Remove(pelletHash);
                        }
                    }

                    // set target if empty
                    if(mine && PacMenTargets.ContainsKey(pacId) && PacMenTargets[pacId] == null) {
                        Console.Error.WriteLine("Getting Pellet "+pacId);
                        PacMenTargets[pacId] = getClosestSubpartitionPellet(PacMen[pacId]);
                        // var pelletFound = getClosestSubpartitionPellet(PacMen[pacId]);
                        if(PacMenTargets[pacId] == null) {
                            Console.Error.WriteLine(string.Format("Partition Complete: {0}", PacMen[pacId][2]));
                            if(PacMen[pacId][2] >= 0) {
                                PartitionStatus[PacMen[pacId][2]] = STATUS_COMPLETE;
                            }
                            PacMen[pacId][2] = STATUS_AVAILABLE;
                        }
                        Console.Error.WriteLine(string.Format("Closest Pellet Found: {0}", PacMenTargets[pacId] != null ? PacMenTargets[pacId][0].ToString():"Error"));
                    } 

                    if(PacMenPrevLocs.ContainsKey(pacId) && PacMen.ContainsKey(pacId)) {
                        PacMenPrevLocs[pacId][0] = PacMen[pacId][0];
                        PacMenPrevLocs[pacId][1] = PacMen[pacId][1];
                        PacMenPrevLocs[pacId][2] = PacMen[pacId][2];
                    }
                    
                }
            }
            int visiblePelletCount = int.Parse(Console.ReadLine()); // all pellets in sight
            var pelletPartition = 0;
            for (int i = 0; i < visiblePelletCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int x = int.Parse(inputs[0]);
                int y = int.Parse(inputs[1]);
                int value = int.Parse(inputs[2]); // amount of points this pellet is worth
            
                if(!PelletFoundMap[y,x]) {
                    pelletPartition = PartitionMap[y,x];
                    if(Pellets.ContainsKey(pelletPartition)) {
                        PelletFoundMap[y,x] = true;
                        Pellets[pelletPartition].Add(getPelletHash(x,y), new int[3]{x, y, value == 1 ? 1 : SUPER_PELLET_VALUE});
                        
                        // reinitialize closed partition if new Pellets found
                        if(PartitionStatus[pelletPartition] == STATUS_COMPLETE) {
                            PartitionStatus[pelletPartition] = STATUS_AVAILABLE;
                        }
                        // Console.Error.WriteLine(string.Format("Adding Pellet[{0}]: [{1},{2}]: {3}", pelletPartition, x, y, value));
                    }
                }   
            }
            initialized = true;

            var action = getActions();
            action = string.IsNullOrEmpty(action)? "MOVE 0 15 10" : action.TrimEnd('|');
            // Console.Error.WriteLine(string.Format("Sending Action: {0}", action));
            Console.WriteLine(action);

        }
    }

    static string getActions() {
        StringBuilder actions = new StringBuilder();
        string switchAction = null;
        string boostAction = null;
        string moveAction = null;

        foreach(var pacMan in PacMen) {
            switchAction = getSwitchAction(pacMan);
            if(!string.IsNullOrEmpty(switchAction)) {
                actions.Append(switchAction);
                PacMenLastMove[pacMan.Key] = ACTION_SWITCH;
                continue;
            } 

            boostAction = getBoostAction(pacMan);
            if(!string.IsNullOrEmpty(boostAction)) {
                actions.Append(boostAction);
                PacMenLastMove[pacMan.Key] = ACTION_BOOST;
                continue;
            }

            updateBlockedPacMenTargets();
            moveAction = getMoveAction(pacMan);
            actions.Append(moveAction);
            PacMenLastMove[pacMan.Key] = ACTION_MOVE;
        }

        return actions.ToString();
    }

     static int getAvailablePartition() {
        for(var i = 0; i < NUM_PARTITIONS; i++) {
            if(PartitionStatus[i] == STATUS_AVAILABLE) {
                PartitionStatus[i] = STATUS_BUSY;
                return i;
            }
        }
        return STATUS_AVAILABLE;
    }

    static int[] getClosestSubpartitionPellet(int[] pacMan) {
        // Console.Error.WriteLine(string.Format("Getting Pellet Location: [{0},{1},{2}]", pacMan[0],pacMan[1],pacMan[2]));
        
        if(pacMan[2] == STATUS_AVAILABLE) {
            return null;
        }
        
        var maxValue = int.MinValue;
        var currentValue = int.MinValue;
        var pelletFound = false;
        KeyValuePair<string, int[]>? maxPellet = null;
        foreach(var pellet in Pellets[pacMan[2]]) {
            
            currentValue = getLocationValue(pacMan, pellet.Value);
            // Console.Error.WriteLine(string.Format("Max: {0} => Compare Pellet: [{1},{2},{3}] - {4} ",maxValue,pellet[0],pellet[1],pellet[2],currentValue));
            if(currentValue > maxValue) {
                maxValue = currentValue;
                maxPellet =  pellet;
                // Console.Error.WriteLine(string.Format("Max Pellet updated: [{0},{1}]-{2}", maxPellet[0], maxPellet[1], maxValue));
            }
        }

        if(Pellets.ContainsKey(pacMan[2]) && maxPellet.HasValue){
            Pellets[pacMan[2]].Remove(maxPellet.Value.Key);
        }
        
        return maxPellet.HasValue ? maxPellet.Value.Value : null;
    }

    static int getLocationValue(int[] pacMan, int[] pellet) {
        // already adjusted for Super Pellet Weight, use directly
        return pellet[2] - distance(pacMan, pellet);
    }

    static int distance(int[] pacMan, int[] target) {
        return Math.Abs(target[0]-pacMan[0]) + Math.Abs(target[1]-pacMan[1]);
    }

    static string getSwitchAction(KeyValuePair<int, int[]> pacMan) {
        int closestEnemyDistance = int.MaxValue;
        int[] closestEnemy = null;
        foreach(var enemyPacMan in EnemyPacMen) {
            if(pacMan.Value[5] == 0 && getEnemyValue(pacMan.Value, enemyPacMan.Value) < 0) {
                var enemyDistance = distance(pacMan.Value, enemyPacMan.Value);
                if(enemyDistance < SWITCH_DISTANCE && enemyDistance < closestEnemyDistance) {
                    closestEnemyDistance = enemyDistance;
                    closestEnemy = enemyPacMan.Value;
                }
            }
        }

        return closestEnemy != null ? string.Format("SWITCH {0} {1}|", pacMan.Key, getCounterAction(closestEnemy[2])) : null;
    }

    static int getEnemyLocationValue(int[] pacMan, int[] enemy) {
        return  getEnemyValue(pacMan, enemy) - distance(pacMan, enemy);
    }

    static int getEnemyValue(int[] pacMan, int[] enemy) {
       //  ROCK = 1; PAPER = 2; SCISSORS = 4;
        var comparerDelta = pacMan[3]-enemy[2];
        return (comparerDelta > 0 || comparerDelta == COMPARE_ROCK_SCISSORS) ? ENEMY_VALUE_POSITIVE 
                                : comparerDelta == 0 ?  ENEMY_VALUE_NEUTRAL : ENEMY_VALUE_NEGATIVE;
    }

    static string getCounterAction(int enemyType) {
       return enemyType == TYPE_ROCK ? "PAPER" : enemyType == TYPE_PAPER ? "SCISSORS" : "ROCK";
    }

    static string getBoostAction(KeyValuePair<int, int[]> pacMan) {
        Console.Error.WriteLine(string.Format("Checking Speed PacMan {0} - Speed: {1} - Cooldown: {2}", pacMan.Key, pacMan.Value[4], pacMan.Value[5]));
        return pacMan.Value[4] == 0 && pacMan.Value[5] == 0 ? string.Format("SPEED {0}|", pacMan.Key) : null;
    }

    static int getTypeId(string rawType) {
        return rawType.Equals("ROCK")?TYPE_ROCK:(rawType.Equals("PAPER")?TYPE_PAPER:TYPE_SCISSORS);
    }

    static void updateBlockedPacMenTargets() {
        foreach(var blockedPacMan in PacMenBlockLocs) {
            if(PacMenTargets.ContainsKey(blockedPacMan.Key)){
                Console.Error.WriteLine(string.Format("Setting block action: {0} [{1},{2}]",blockedPacMan.Key,PacMenStartLocs[blockedPacMan.Key][0],PacMenStartLocs[blockedPacMan.Key][1]));
                PacMenTargets[blockedPacMan.Key] = blockedPacMan.Value;
            }
        }
    }

    static string getMoveAction(KeyValuePair<int, int[]> pacMan) {
        Console.Error.WriteLine(string.Format("PacMan {0} Speed available: {1} - Has target: {2}",pacMan.Key, pacMan.Value[4], PacMenTargets != null));
        return pacMan.Value[4] > 0 && PacMenTargets[pacMan.Key] != null 
                ? string.Format("MOVE {0} {1} {2}|", pacMan.Key, PacMenTargets[pacMan.Key][0], PacMenTargets[pacMan.Key][1]) 
                : getRandomMoveAction(pacMan.Key);
    }

     static string getRandomMoveAction(int pacManId) {
        Console.Error.WriteLine(string.Format("{0} Used Random Move", pacManId));
        var randLoc = getRandomLoc();
        return string.Format("MOVE {0} {1} {2}|", pacManId, randLoc[0], randLoc[1]);
    }

     static int[] getRandomLoc() {
        var currentChar = '#';
        var randLoc = new int[3];
        while (currentChar == '#') {
            randLoc[0] = rGen.Next(0, MapDimensions[0]);
            randLoc[1] = rGen.Next(0, MapDimensions[1]);
            randLoc[2] = BLOCKED_COOLDOWN;
            currentChar = Map[randLoc[1]][randLoc[0]];
        }
        return randLoc;
    }

    static string getPelletHash(int x, int y) {
        return string.Format("{0}{1}", x, y);
    }
}