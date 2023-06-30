using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.LowLevel;

namespace UnityPlayerLooper.Utility
{
    public class PlayerLoopInfoExporter
    {
        public static void Export()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Unity {Application.unityVersion}");
            sb.AppendLine();

            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            
            foreach (var header in playerLoop.subSystemList)
            {
                sb.AppendLine($"---------- {header.type.Name} ----------");
                
                foreach (var subSystem in header.subSystemList)
                {
                    sb.AppendLine($"{header.type.Name}.{subSystem.type.Name}");
                    
                    if (subSystem.subSystemList != null)
                    {
                        Debug.LogError($"{header.type.Name}.{subSystem.type.Name} has more subsystems: {subSystem.subSystemList.Length}");
                    }
                }
            }
            
            var filePath = Path.Combine(Application.dataPath, $"PlayerLoopInfo_{Application.unityVersion}.txt");
            File.WriteAllText(filePath, sb.ToString());
        }
    }
}