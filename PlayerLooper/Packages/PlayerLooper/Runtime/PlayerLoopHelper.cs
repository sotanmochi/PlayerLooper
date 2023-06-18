using System;
using UnityEngine.LowLevel;

namespace PlayerLooper
{
    public static class PlayerLoopHelper
    {
        public static PlayerLoopSystem CreateLoopSystem<T>(PlayerLoopSystem.UpdateFunction updateDelegate)
        {
            return new PlayerLoopSystem()
            {
                type = typeof(T),
                updateDelegate = updateDelegate,
            };
        }

        public static int FindLoopSystemIndex(Type targetSystemType, PlayerLoopSystem[] playerLoopList)
        {
            int index = -1;

            for (int i = 0; i < playerLoopList.Length; i++)
            {
                if (playerLoopList[i].type == targetSystemType)
                {
                    index = i;
                }
            }

            return index;
        }

        public static void InsertSubSystem(ref PlayerLoopSystem currentSystem, int insertIndex, ref PlayerLoopSystem newSubSystem)
        {
            var newSubSystemList = new PlayerLoopSystem[currentSystem.subSystemList.Length + 1];

            for (int i = 0; i < newSubSystemList.Length; i++)
            {
                if (i < insertIndex)
                {
                    newSubSystemList[i] = currentSystem.subSystemList[i];
                }
                else if (i == insertIndex)
                {
                    newSubSystemList[i] = newSubSystem;
                }
                else
                {
                    newSubSystemList[i] = currentSystem.subSystemList[i - 1];
                }
            }

            currentSystem.subSystemList = newSubSystemList;
        }

        public static void InsertSubSystem(ref PlayerLoopSystem currentSystem, Type targetSubSystemType, ref PlayerLoopSystem newSubSystem)
        {
            int insertIndex = -1;

            if (targetSubSystemType == null)
            {
                insertIndex = 0;
            }
            else
            {
                for (int i = 0; i < currentSystem.subSystemList.Length; i++)
                {
                    if (currentSystem.subSystemList[i].type == targetSubSystemType)
                    {
                        insertIndex = i;
                    }
                }
            }

            if (insertIndex < 0)
            {
                throw new ArgumentException($"{targetSubSystemType.FullName} has not found in current system.");
            }

            InsertSubSystem(ref currentSystem, insertIndex, ref newSubSystem);
        }

        public static void AddSubSystem(ref PlayerLoopSystem currentSystem, ref PlayerLoopSystem newSubSystem)
        {
            var newSubSystemList = new PlayerLoopSystem[currentSystem.subSystemList.Length + 1];

            int index;
            for (index = 0; index < currentSystem.subSystemList.Length; index++)
            {
                newSubSystemList[index] = currentSystem.subSystemList[index];
            }
            newSubSystemList[index] = newSubSystem;

            currentSystem.subSystemList = newSubSystemList;
        }
    }
}