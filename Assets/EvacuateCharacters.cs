// using UnityEngine;
// using System.Collections.Generic;
// using Pathfinding;
//
// public class EvacuateCharacters : MonoBehaviour
// {
//     public Collider2D dangerZone; // 这个区域中的人物需要撤离
//     public Transform safePoint;   // 最近的安全点（可以是区域边缘）
//
//     private List<AIDestinationSetter> affectedAgents = new List<AIDestinationSetter>();
//
//     void Start()
//     {
//         // 找到所有 AI 角色
//         var allAgents = FindObjectsOfType<AIDestinationSetter>();
//
//         foreach (var agent in allAgents)
//         {
//             if (dangerZone.bounds.Contains(agent.transform.position))
//             {
//                 // 让他们移动到安全点
//                 agent.target = safePoint;
//                 affectedAgents.Add(agent);
//             }
//         }
//     }
//
//     void OnDestroy()
//     {
//         // 让 NPC 恢复正常寻路目标
//         foreach (var agent in affectedAgents)
//         {
//             if (agent != null)
//             {
//                 agent.target = null; // 恢复自由行动
//             }
//         }
//     }
// }