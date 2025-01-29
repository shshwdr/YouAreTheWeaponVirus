using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class TemporaryBlockArea : MonoBehaviour
{
    public Collider2D blockArea; // 需要阻止的区域

    private List<AIPath> affectedAgents = new List<AIPath>();
    private GraphUpdateObject guo; // 记录用于阻止的 GraphUpdateObject

    void Start()
    {
        // 1. 创建 GraphUpdateObject 并阻止 AI 进入
        guo = new GraphUpdateObject(blockArea.bounds);
        guo.modifyWalkability = true;
        guo.setWalkability = false; // 让该区域不可通行
        guo.updatePhysics = true;
        AstarPath.active.UpdateGraphs(guo); // 立即更新路径

        // 2. 让区域内的 NPC 重新寻路
        EvacuateCharacters();
    }

    void EvacuateCharacters()
    {
        var allAgents = FindObjectsOfType<AIPath>(); // 获取所有 AI
        foreach (var agent in allAgents)
        {
            if (blockArea.bounds.Contains(agent.transform.position)) // 如果 NPC 在禁行区域
            {
                Vector3 escapePoint = FindNearestEscapePoint(agent.transform.position);
                agent.destination = escapePoint; // 让 NPC 逃离
                agent.SearchPath(); // 重新计算路径

                affectedAgents.Add(agent);
            }
        }
    }

    Vector3 FindNearestEscapePoint(Vector3 currentPosition)
    {
        NNConstraint constraint = NNConstraint.Default;
        GraphNode nearestNode = AstarPath.active.GetNearest(currentPosition, constraint).node;

        if (nearestNode != null && nearestNode.Walkable)
        {
            Vector3 escapePoint = (Vector3)nearestNode.position;
            if (PathUtilities.IsPathPossible(AstarPath.active.GetNearest(currentPosition).node, nearestNode))
            {
                return escapePoint;
            }
        }
        
        return currentPosition; // 如果找不到有效点，就暂时不移动
    }

    void RemoveBlockArea()
    {
        if (guo != null)
        {
            // 3. 解除禁行区域（恢复可行走区域）
            GraphUpdateObject restoreGuo = new GraphUpdateObject(blockArea.bounds);
            restoreGuo.modifyWalkability = true;
            restoreGuo.setWalkability = true; // 重新允许通行
            restoreGuo.updatePhysics = true;
            AstarPath.active.UpdateGraphs(restoreGuo);
        }
    }

    void OnDestroy()
    {
        RemoveBlockArea(); // **提前在销毁前更新路径**
    }
}
